using System.Security.Cryptography.X509Certificates;
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

            modelBuilder.Entity<Customer>()
                .HasKey(cust => cust.Id);

            modelBuilder.Entity<User>()
                .HasKey(user => user.Id);

            modelBuilder.Entity<User>()
                .HasOne(user => user.Customer)
                .WithOne(cust => cust.User)
                .HasForeignKey<User>(user => user.Id);

            modelBuilder.Entity<Work>()
                .HasKey(work => work.Id);

            modelBuilder.Entity<Booking>()
                .HasOne(book => book.Customer)
                .WithMany(cust => cust.Bookings);

            modelBuilder.Entity<Booking>()
                .HasOne(book => book.Work)
                .WithMany(work => work.Bookings)
                .IsRequired(false);
        }

        public DbSet<Work> Work { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}