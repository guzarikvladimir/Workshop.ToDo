using Services;
using System.Linq;
using ToDoClient.Services;
using ToDoClient.Models;
using todoclient.Models;

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

        
    }
}