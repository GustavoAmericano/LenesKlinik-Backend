using LenesKlinik.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LenesKlinik.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Customer>()
            //    .HasOne(c => c.Type)
            //    .WithMany(ct => ct.Customers)
            //    .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasKey(user => user.Id);

            modelBuilder.Entity<Work>()
                .HasKey(work => work.Id);

            modelBuilder.Entity<Booking>()
                .HasOne(book => book.User)
                .WithMany(user => user.Bookings)
                .IsRequired();

            modelBuilder.Entity<Booking>()
                .HasOne(book => book.Work)
                .WithMany(work => work.Bookings);
        }

        public DbSet<Work> Work { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}