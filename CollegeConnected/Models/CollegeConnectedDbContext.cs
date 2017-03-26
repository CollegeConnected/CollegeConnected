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
        public virtual DbSet<Constituent> Students { get; set; }

        public static CollegeConnectedDbContext Create()
        {
            return new CollegeConnectedDbContext();
        }
    }
}