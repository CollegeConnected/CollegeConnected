using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CollegeConnected.Models
{
    public class CollegeConnectedDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Event> Events { get; set; }
    }
}