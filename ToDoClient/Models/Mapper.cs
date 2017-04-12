using ToDoClient.Models;
using todoclient.ORM;

namespace todoclient.Models
{
    public static class Mapper
    {
        public static ToDoItemViewModel ToMVC(this ToDo todo)
        {
            return new ToDoItemViewModel()
            {
                Id = todo.Id,
                ToDoId = todo.ToDoId,
                UserId = todo.UserId,
                Name = todo.Name,
                IsCompleted = todo.IsCompleted,
                IsNeedToDelete = todo.IsNeedToDelete
            };
        }

        public static ToDo ToORM(this ToDoItemViewModel todo)
        {
            return new ToDo()
            {
                Id = todo.Id,
                ToDoId = todo.ToDoId,
                UserId = todo.UserId,
                Name = todo.Name,
                IsCompleted = todo.IsCompleted
            };
        }

        public static ToDoItemViewModel ToCloud(this ToDo todo)
        {
            return new ToDoItemViewModel()
            {
                ToDoId = todo.ToDoId,
                UserId = todo.UserId,
                Name = todo.Name,
                IsCompleted = todo.IsCompleted,
            };
        }
    }
}