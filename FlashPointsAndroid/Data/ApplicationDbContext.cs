using FlashPoints.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashPoints.Data
{
    public class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=tcp:flashpoints.database.windows.net,1433;Initial Catalog=flashpoints-database;Persist Security Info=False;User ID=flashpointsadmin;Password=Flashpoints!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }

        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<Prize> Prize { get; set; }
    }
}
