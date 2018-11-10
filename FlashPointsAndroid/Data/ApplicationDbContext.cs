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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PrizeRedeemed>()
                .HasKey(pr => new { pr.UserID, pr.PrizeID });

            modelBuilder.Entity<PrizeRedeemed>()
                .HasOne(bc => bc.Prize)
                .WithMany(b => b.PrizesRedeemed)
                .HasForeignKey(bc => bc.PrizeID);

            modelBuilder.Entity<PrizeRedeemed>()
                .HasOne(bc => bc.User)
                .WithMany(c => c.PrizesRedeemed)
                .HasForeignKey(bc => bc.UserID);



            modelBuilder.Entity<EventAttended>()
                .HasKey(pr => new { pr.UserID, pr.EventID });

            modelBuilder.Entity<EventAttended>()
                .HasOne(bc => bc.Event)
                .WithMany(b => b.EventsAttended)
                .HasForeignKey(bc => bc.EventID);

            modelBuilder.Entity<EventAttended>()
                .HasOne(bc => bc.User)
                .WithMany(c => c.EventsAttended)
                .HasForeignKey(bc => bc.UserID);
        }
    }
}
