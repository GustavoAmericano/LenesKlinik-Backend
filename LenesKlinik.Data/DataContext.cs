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
            //modelBuilder.Entity<User>()
            //   .HasKey(u => u.Id);

            //modelBuilder.Entity<Pet>()
            //  .HasOne(p => p.Owner)
            //  .WithMany(o => o.Pets)
            //  .OnDelete(DeleteBehavior.SetNull);
        }

        //public DbSet<User> Users { get; set; }
        public DbSet<Work> Work { get; set; }
        public DbSet<User> Users { get; set; }


    }
}