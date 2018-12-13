using System;
using System.Collections.Generic;
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
            catch (Exception)
            {
                throw new Exception("An Error occured trying to save the user.");
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
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new Exception("An Error occured trying to save the user.");
            }
        }

        public User UpdateUser(User user, string clearPass, string newPass)
        {
            try
            {
                User storedUser = _repo.GetUserById(user.Id);
                if(storedUser == null) throw new ArgumentException($"No user found with ID: {user.Id}");
                if(storedUser.PasswordHash != GenerateHash(clearPass + storedUser.PasswordSalt)) throw new ArgumentException("Wrong password");
                if (!ValidateEmail(user.Email)) throw new ArgumentException("Email not accepted!");
                if(newPass != null)
                    if (!ValidatePassword(newPass)) throw new ArgumentException("Password too weak!");
                ValidateCustomerInformation(user.Customer);

                if (newPass == null)
                {
                    user.PasswordSalt = storedUser.PasswordSalt;
                    user.PasswordHash = GenerateHash(clearPass + user.PasswordSalt);
                }
                else
                {
                    user.PasswordSalt = GenerateSalt();
                    user.PasswordHash = GenerateHash(newPass + user.PasswordSalt);
                }
                

                return _repo.UpdateUser(user);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new Exception("An Error occured trying to save the user.");
            }
        }

        public User GetUserById(int id)
        {
            try
            {
                return _repo.GetUserById(id);
            }
            catch (Exception)
            {
                throw new Exception("An Error occured trying to fetch the user.");
            }
        }

        public List<User> GetAllUsers()
        {
            try
            {
                return _repo.GetAllUsers();
            }
            catch (Exception)
            {
                throw new Exception("An Error occured trying to fetch the users.");
            }
        }


        /// <summary>
        /// Validates that the User entities information is correct.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="clearPass"></param>
        private void ValidateUserInformation(User user, string clearPass)
        {
            if (!ValidateEmail(user.Email)) throw new ArgumentException("Email not accepted!");
            if (!ValidatePassword(clearPass)) throw new ArgumentException("Password too weak!");
            if (_repo.CheckEmailInUse(user.Email)) throw new ArgumentException("Email already in use!"); // This is last, because it sends a request to DB.
        }

        /// <summary>
        /// Validates that the password is within the rules for it.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool ValidatePassword(string password)
        {
            if (password.Length < 8) return false;
            return true;
            // Could add more checks here
        }


        /// <summary>
        /// Validates that the customer entity's information is correct.
        /// </summary>
        /// <param name="cust"></param>
        private void ValidateCustomerInformation(Customer cust)
        {
            if (string.IsNullOrEmpty(cust.Firstname)) throw new ArgumentException("Firstname null or empty!");
            if (string.IsNullOrEmpty(cust.Lastname)) throw new ArgumentException("Lastname null or empty!");
            if (string.IsNullOrEmpty(cust.Address)) throw new ArgumentException("Address null or empty!");
            if (cust.PhoneNumber.ToString().Length != 8) throw new ArgumentException("Invalid phone number!");
        }


        /// <summary>
        /// Validates that the email is an actual email.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        private bool ValidateEmail(string userEmail)
        {
            if (userEmail == null) return false;
            return CreateValidEmailRegex().IsMatch(userEmail);
        }

        /// <summary>
        /// Creates the regex used to validate an email address.
        /// </summary>
        /// <returns></returns>
        private static Regex CreateValidEmailRegex()
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                                       + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                                       + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
        }


        /// <summary>
        /// Generates a random 128bit string.
        /// </summary>
        /// <returns></returns>
        private static string GenerateSalt()
        {
            byte[] bytes = new byte[128 / 8];
            using (var keyGenerator = RandomNumberGenerator.Create())
            {
                keyGenerator.GetBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Generates a one-way SHA256 hash from the input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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