using System;
using System.Security.Cryptography;
using System.Text;

namespace LenesKlinik.Data
{
    public class DBSeed
    {
        public static void SeedDB(DataContext ctx)
        {
            ctx.Database.EnsureDeleted(); // Delete ENTIRE Db!
            ctx.Database.EnsureCreated(); // Recreate Db

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