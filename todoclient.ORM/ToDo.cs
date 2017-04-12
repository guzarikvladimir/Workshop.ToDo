namespace todoclient.ORM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ToDos")]
    public partial class ToDo
    {
        public int Id { get; set; }

        public int ToDoId { get; set; }

        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsNeedToDelete { get; set; }
    }
}
