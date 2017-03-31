using System.Data.Entity;

namespace CollegeConnected.Models
{
    public class CollegeConnectedDbContext : DbContext
    {
        public CollegeConnectedDbContext()
            : base("DefaultConnection")
        {
        }

        public virtual DbSet<ImportResult> ImportResults { get; set; }
        public virtual DbSet<Constituent> Constituents { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventAttendance> EventAttendants { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }

        public static CollegeConnectedDbContext Create()
        {
            return new CollegeConnectedDbContext();
        }
    }
}