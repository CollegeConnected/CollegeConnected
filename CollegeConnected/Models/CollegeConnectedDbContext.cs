using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CollegeConnected.Models
{
    public class CollegeConnectedDbContext : IdentityDbContext<ApplicationUser>
    {
        public CollegeConnectedDbContext()
            : base("DefaultConnection", false)
        {
        }

        //public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<ImportResult> ImportResults { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        //public virtual DbSet<User> Users { get; set;}
      //  public virtual DbSet<Event> Events { get; set; }

        public static CollegeConnectedDbContext Create()
        {
            return new CollegeConnectedDbContext();
        }
    }
}