using LenesKlinik.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LenesKlinik.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Work>()
                .HasKey(work => work.Id);

            modelBuilder.Entity<User>()
                .HasKey(user => user.Id);

            modelBuilder.Entity<Booking>()
                .HasOne<User>()
                .WithMany(u => u.Bookings);

            modelBuilder.Entity<Booking>()
                .HasOne<Work>()
                .WithMany(wo => wo.Bookings);
        }

        public DbSet<Work> Work { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}