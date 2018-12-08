using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.ApplicationServices.Implementation
{
    public class UserService : IUserService
    {
        private IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public User CreateUser(User user, string clearPass)
        {
            try
            {
                ValidateUserInformation(user, clearPass);
                ValidateCustomerInformation(user.Customer);
                user.PasswordSalt = GenerateSalt();
                user.PasswordHash = GenerateHash(clearPass + user.PasswordSalt);
                return _repo.CreateUser(user);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public User ValidateUser(string email, string password)
        {
            try
            {
                var user = _repo.GetUserByMail(email);
                if (user.PasswordHash != GenerateHash(password + user.PasswordSalt) ) throw new ArgumentException("Wrong password");
                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void ValidateUserInformation(User user, string clearPass)
        {
            if (!ValidateEmail(user.Email)) throw new ArgumentException("Email not accepted!");
            if (clearPass.Length < 8) throw new ArgumentException("Password too weak!");
        }

        private void ValidateCustomerInformation(Customer cust)
        {
            if (string.IsNullOrEmpty(cust.Firstname)) throw new ArgumentException("Firstname null or empty!");
            if (string.IsNullOrEmpty(cust.Lastname)) throw new ArgumentException("Lastname null or empty!");
            if (string.IsNullOrEmpty(cust.Address)) throw new ArgumentException("Address null or empty!");
            if (cust.SecretNumber.ToString().Length != 10) throw new ArgumentException("Invalid secret!");
        }

        private bool ValidateEmail(string userEmail)
        {
            if (userEmail == null) return false;
            return CreateValidEmailRegex().IsMatch(userEmail);
        }

        private static Regex CreateValidEmailRegex()
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                                       + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                                       + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
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