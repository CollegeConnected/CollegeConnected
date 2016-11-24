using System.Data.Entity;

namespace CollegeConnected.Models
{
    public class CollegeConnectedDbContext : DbContext
    {
        public CollegeConnectedDbContext()
            : base("DefaultConnection")
        {
        }

        //public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<ImportResult> ImportResults { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Event> Events { get; set; }

        public static CollegeConnectedDbContext Create()
        {
            return new CollegeConnectedDbContext();
        }
    }
}