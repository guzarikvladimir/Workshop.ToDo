using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using todoclient.ORM;

namespace Services
{
    public class ToDoDbService
    {
        private readonly DbContext context = new todoclientdb();

        public IEnumerable<int> GetUsers()
        {
            return context.Set<ToDo>().Select(x => x.UserId).ToArray();
        }

        public IQueryable<ToDo> GetByUserId(int userId)
        {
            return context.Set<ToDo>().Where(x => x.UserId == userId).AsNoTracking();
        }

        public void Create(ToDo model)
        {
            context.Set<ToDo>().Add(model);
            context.SaveChanges();
        }

        public void Update(ToDo model)
        {
            var todo = context.Set<ToDo>().FirstOrDefault(x => x.Id == model.Id);
            todo.IsCompleted = model.IsCompleted;
            todo.Name = model.Name;
            context.SaveChanges();
        }

        public void Delete(int modelId)
        {
            var todo = context.Set<ToDo>().FirstOrDefault(x => x.Id == modelId);
            context.Set<ToDo>().Remove(todo);
            context.SaveChanges();
        }

        public void UpdateTodoId(int modelId, int todoId)
        {
            var todo = context.Set<ToDo>().FirstOrDefault(x => x.Id == modelId);
            todo.ToDoId = todoId;
            context.SaveChanges();
        }

        public void MarkAsNeedToDelete(int modelId)
        {
            var todo = context.Set<ToDo>().FirstOrDefault(x => x.Id == modelId);
            todo.IsNeedToDelete = true;
            context.SaveChanges();
        }
    }
}
