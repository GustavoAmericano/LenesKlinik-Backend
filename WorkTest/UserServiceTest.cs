using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using LenesKlinik.Core.ApplicationServices;
using LenesKlinik.Core.ApplicationServices.Implementation;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;
using LenesKlinik.Core.Entities.DTO;
using Moq;
using Xunit;

namespace WorkTest
{
    public class UserServiceTest
    {   
        private IUserService _service;
        private Mock<IUserRepository> _mock;
        private List<User> _users;
        private UserCreateInput _inputUser;

        public UserServiceTest()
        {
            _users = new List<User>(GetMockUsers());
            _mock = new Mock<IUserRepository>();
            _service = new UserService(_mock.Object);
            _inputUser = new UserCreateInput
            {
                SecretNumber = 1234567890,
                Address = "Fake Address",
                Firstname = "First",
                Lastname = "Name",
                Email = "Mail@Mail.mail",
                clearPassword = "password"
            };

            _mock.Setup(repo => repo.CreateUser(It.IsAny<User>())).Returns<User>(user =>
            {
                user.Id = 1337;
                return user;
            });
        }

        [Fact]
        public void CreateUserSuccessTest()
        {
            User returnUser = _service.CreateUser(_inputUser);

            Assert.Equal(1337,returnUser.Id);
            _mock.Verify(repo => repo.CreateUser(returnUser), Times.Once);
        }

        [Fact]
        public void CreateUserNoFirstNameExpectArgumentExceptionTest()
        {
            _inputUser.Firstname = null;
            User returnUser = _service.CreateUser(_inputUser);

            Exception e = Assert.Throws<ArgumentException>(() => _service.CreateUser(_inputUser));
            Assert.Equal("Firstname null or empty!", e.Message);
            _mock.Verify(repo => repo.CreateUser(It.IsAny<User>()), Times.Never);
        }

        #region Other
        // Creates two users with random generated salt.
        private IEnumerable<User> GetMockUsers()
        {
            var salt = GenerateSalt();
            var hash = GenerateHash("4dm1n" + salt);
            var user1 = new User
            {
                Id = 1,
                Email = "Admin@lk.dk",
                PasswordSalt = salt,
                PasswordHash = hash,
                Address = "Vejlevej 22",
                Firstname = "Kenneth",
                Lastname = "Pedersen",
                SecretNumber = 0910951337,
                isAdmin = true
            };

            salt = GenerateSalt();
            hash = GenerateHash("us3r" + salt);
            var user2 = new User
            {
                Id = 1,
                Email = "Admin@lk.dk",
                PasswordSalt = salt,
                PasswordHash = hash,
                Address = "Vejlevej 22",
                Firstname = "Kenneth",
                Lastname = "Pedersen",
                SecretNumber = 0910951337,
                isAdmin = true
            };

            return new List<User>() { user1, user2 };
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
        #endregion
    }
}