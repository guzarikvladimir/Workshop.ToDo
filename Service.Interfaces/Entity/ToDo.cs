
namespace Service.Interfaces.Entity
{
    public class ToDo
    {
        public int ToDoId { get; set; }

        public int UserId { get; set; }

        public bool IsCompleted { get; set; }

        public string Name { get; set; }

        public bool IsUploaded { get; set; }
    }
}
