using Services;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using todoclient.ORM;
using ToDoClient.Models;
using ToDoClient.Services;
using todoclient.Models;

namespace ToDoClient.Controllers
{
    /// <summary>
    /// Processes todo requests.
    /// </summary>
    public class ToDosController : ApiController
    {
        private readonly ToDoService todoService = new ToDoService();
        private readonly UserService userService = new UserService();
        private readonly ToDoDbService tododbService = new ToDoDbService();

        /// <summary>
        /// Returns all todo-items for the current user.
        /// </summary>
        /// <returns>The list of todo-items.</returns>
        public IEnumerable<ToDoItemViewModel> Get()
        {
            var userId = userService.GetOrCreateUser();
            return tododbService.GetByUserId(userId).ToList()
                .Select(todo => todo.ToMVC());
        }

        /// <summary>
        /// Updates the existing todo-item.
        /// </summary>
        /// <param name="todo">The todo-item to update.</param>
        public void Put(ToDoItemViewModel todo)
        {
            todo.UserId = userService.GetOrCreateUser();
            tododbService.Update(todo.ToORM());
            var item = tododbService.GetByUserId(todo.UserId).FirstOrDefault(x => x.Id == todo.Id);
            Task.Run(() => todoService.UpdateItem(item.ToCloud()));
        }

        /// <summary>
        /// Deletes the specified todo-item.
        /// </summary>
        /// <param name="id">The todo item identifier.</param>
        public void Delete(int id)
        {
            var userId = userService.GetOrCreateUser();
            var todo = tododbService.GetByUserId(userId).FirstOrDefault(x => x.Id == id);
            if (todo != null)
            {
                int todoId = todo.ToDoId;
                tododbService.Delete(id);
                Task.Run(() => todoService.DeleteItem(todoId));
            }
        }

        /// <summary>
        /// Creates a new todo-item.
        /// </summary>
        /// <param name="todo">The todo-item to create.</param>
        public void Post(ToDoItemViewModel todo)
        {   
            todo.UserId = userService.GetOrCreateUser();
            tododbService.Create(new ToDo()
            {
                Name = todo.Name,
                UserId = todo.UserId,
                IsCompleted = todo.IsCompleted
            });
            Task.Run(() => todoService.CreateItem(todo))
                .ContinueWith((unusedArg) => 
                {
                    var itemdb = tododbService.GetByUserId(todo.UserId).FirstOrDefault(x => x.Name == todo.Name);
                    var itemCloud = todoService.GetItems(todo.UserId).FirstOrDefault(x => x.Name.Contains(itemdb.Name));
                    if (itemCloud != null)
                    {
                        itemdb.ToDoId = itemCloud.ToDoId;
                        tododbService.UpdateTodoId(itemdb.Id, itemCloud.ToDoId);
                    }
                });
        }
    }
}
