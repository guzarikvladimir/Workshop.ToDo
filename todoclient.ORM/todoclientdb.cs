namespace todoclient.ORM
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class todoclientdb : DbContext
    {
        public todoclientdb()
            : base("name=todoclientdb")
        {
        }

        public virtual DbSet<ToDo> ToDos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
