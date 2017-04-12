using System.Data.Entity;
using Services;
using System.Linq;
using ToDoClient.Services;
using ToDoClient.Models;
using todoclient.Models;
using todoclient.ORM;

namespace todoclient.Infrastructure
{
    public class ServiceSynchronizer
    {
        private readonly ToDoService todoService = new ToDoService();
        private readonly UserService userService = new UserService();
        private readonly ToDoDbService tododbService = new ToDoDbService();

        public void UploadMissing()
        {
            int userId = userService.GetOrCreateUser();
            var itemsCloud = todoService.GetItems(userId).ToList();
            var itemsDB = tododbService.GetByUserId(userId).ToList().Select(x => x.ToMVC());
            var tempResult = from itemCl in itemsCloud
                             from itemDB in itemsDB
                             where itemCl.ToDoId == itemDB.ToDoId
                             select new ToDoItemViewModel{
                                 Id = 0,
                                 ToDoId = itemCl.ToDoId,
                                 IsCompleted = itemCl.IsCompleted,
                                 Name = itemCl.Name,
                                 UserId = itemCl.UserId };
            foreach (var item in itemsCloud)
            {
                var task = tempResult.FirstOrDefault(x => x.ToDoId == item.ToDoId);
                if (task == null)
                    tododbService.Create(item.ToORM());
            }

            foreach (var item in itemsDB)
            {
                var task = tempResult.FirstOrDefault(x => x.ToDoId == item.ToDoId);
                if (task == null)
                    todoService.DeleteItem(item.ToDoId);
            }
        }

        public void DeleteMissing()
        {
            int userId = userService.GetOrCreateUser();
            var itemsCloud = todoService.GetItems(userId).ToList();
            var itemsDB = tododbService.GetByUserId(userId).ToList().Select(x => x.ToMVC());
            var tempResult = from itemDB in itemsDB
                             from itemCl in itemsCloud
                             where itemDB.ToDoId == itemCl.ToDoId
                             select new ToDoItemViewModel
                             {
                                 Id = 0,
                                 ToDoId = itemDB.ToDoId,
                                 IsCompleted = itemDB.IsCompleted,
                                 Name = itemDB.Name,
                                 UserId = itemDB.UserId
                             };
            foreach (var item in itemsDB)
            {
                var task = tempResult.FirstOrDefault(x => x.ToDoId == item.ToDoId);
                if (task == null)
                    todoService.CreateItem(item);
            }

            foreach (var item in itemsCloud)
            {
                var task = tempResult.FirstOrDefault(x => x.ToDoId == item.ToDoId);
                if (task == null)
                    todoService.DeleteItem(item.ToDoId);
            }
        }

        public void Synchronize()
        {
            var users = tododbService.GetUsers();
            foreach (int userId in users)
            {
                var dbCol = tododbService.GetByUserId(userId).ToList();
                var cloudCol = todoService.GetItems(userId);
                foreach (ToDo item in dbCol)
                {
                    if (!item.IsNeedToDelete && item.ToDoId == 0)
                    {
                        int todoId;
                        if (!cloudCol.Any(x => x.UserId == userId && x.Name.Contains(item.Name)))
                        {
                            todoService.CreateItem(item.ToCloud());
                            todoId = todoService.GetItems(userId).FirstOrDefault(x => x.Name.Contains(item.Name)).ToDoId;
                        }
                        else
                        {
                            todoId = cloudCol.FirstOrDefault(x => x.UserId == userId && x.Name.Contains(item.Name)).ToDoId;
                        }
                        tododbService.UpdateTodoId(item.Id, todoId);
                    }
                    else if (item.IsNeedToDelete)
                    {
                        if (item.ToDoId != 0)
                        {
                            todoService.DeleteItem(item.ToDoId);
                        }
                        else
                        {
                            if (cloudCol.Any(x => x.UserId == userId && x.Name.Contains(item.Name)))
                            {
                                int todoId = cloudCol.FirstOrDefault(x => x.UserId == userId && x.Name.Contains(item.Name)).ToDoId;
                                todoService.DeleteItem(todoId);
                            }
                        }
                        tododbService.Delete(item.Id);
                    }
                }
            }
        }
    }
}