using System;
using System.Security.Cryptography;
using System.Text;
using LenesKlinik.Core.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LenesKlinik.Data
{
    public class DBSeed
    {
        public static void SeedDB(DataContext ctx)
        {
            ctx.Database.EnsureDeleted(); // Delete ENTIRE Db!
            ctx.Database.EnsureCreated(); // Recreate Db

            var work1 = ctx.Work.Add(new Work
            {
                Id = 1,
                Title = "Massage",
                Description = "A nice massage",
                Duration = 30,
                Price = 299.99,
                ImageUrl = "http://chchacupuncture.co.nz/wp-content/uploads/2016/07/services5.jpg"
            }).Entity;

            var work2 = ctx.Work.Add(new Work
            {
                Id = 2,
                Title = "Raindrop Massage",
                Description = "A nice Raindrop massage",
                Duration = 90,
                Price = 799.99,
                ImageUrl = "http://chchacupuncture.co.nz/wp-content/uploads/2016/07/services5.jpg"
            }).Entity;


            var salt = GenerateSalt();
            var hash = GenerateHash("4dm1n" + salt);
            var user1 = ctx.Users.Add(new User
            {
                Id = 1,
                Email = "Admin@lk.dk",
                PasswordSalt = salt,
                PasswordHash = hash,
                IsAdmin = true
            }).Entity;



            salt = GenerateSalt();
            hash = GenerateHash("us3r" + salt);
            var user2 = ctx.Users.Add(new User
            {
                Id = 2,
                Email = "user@lk.dk",
                PasswordSalt = salt,
                PasswordHash = hash,
                IsAdmin = false
            }).Entity;


            var customer1 = ctx.Customers.Add(new Customer
            {
                Id = 1,
                Address = "Vejlevej 22",
                Firstname = "Kenneth",
                Lastname = "Pedersen",
                Birthdate = new DateTime(1995,10,09),
                PhoneNumber = 51158200,
                Bookings = null,
                User = user1
            }).Entity;

            var customer2 = ctx.Customers.Add(new Customer
            {
                Id = 2,
                Address = "Vejlevej 22",
                Firstname = "Kenneth",
                Lastname = "Pedersen",
                Birthdate = new DateTime(1970,03,01),
                PhoneNumber = 51928312,
                Bookings = null,
                User = user2
            }).Entity;
            ctx.SaveChanges();
        }

        private static string GenerateSalt()
        {
            byte[] bytes = new byte[128 / 8];
            using (var keyGenerator = RandomNumberGenerator.Create())
            {
                keyGenerator.GetBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private static string GenerateHash(string input)
        {
            using (var sha = SHA256Managed.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

                return BitConverter.ToString(bytes);
            }
        }
    }
}