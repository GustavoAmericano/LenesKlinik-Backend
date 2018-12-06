using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using LenesKlinik.Core.ApplicationServices;
using LenesKlinik.Core.ApplicationServices.Implementation;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;
using Moq;
using Xunit;

namespace WorkTest
{
    public class UserServiceTest
    {   
        private IUserService _service;
        private Mock<IUserRepository> _mock;
        private List<User> _users;
        private User _createUser;
        private string _strongPass;

        public UserServiceTest()
        {
            _users = new List<User>(GetMockUsers());
            _mock = new Mock<IUserRepository>();
            _service = new UserService(_mock.Object);
            _strongPass = "Str0ngP4$$";
            _createUser = new User
            {
                Customer = new Customer
                {
                    Id = 1,
                    Address = "Fake Address",
                    Firstname = "First",
                    Lastname = "last",
                    SecretNumber = 1234567890,
                },
                Email = "Email@mail.com",

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
            User returnUser = _service.CreateUser(_createUser, _strongPass);
            Assert.Equal(1337,returnUser.Id);
            _mock.Verify(repo => repo.CreateUser(returnUser), Times.Once);
        }

        [Fact]
        public void CreateUserNoFirstNameExpectArgumentExceptionTest()
        {
            _createUser.Customer.Firstname = null;

            Exception e = Assert.Throws<ArgumentException>(() =>
                _service.CreateUser(_createUser, _strongPass));
            Assert.Equal("Firstname null or empty!", e.Message);
            _mock.Verify(repo => repo.CreateUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void CreateUserNolastNameExpectArgumentExceptionTest()
        {
            _createUser.Customer.Lastname = null;

            Exception e = Assert.Throws<ArgumentException>(() =>
                _service.CreateUser(_createUser, _strongPass));
            Assert.Equal("Lastname null or empty!", e.Message);
            _mock.Verify(repo => repo.CreateUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void CreateUserNoAddressExpectArgumentExceptionTest()
        {
            _createUser.Customer.Address = null;

            Exception e = Assert.Throws<ArgumentException>(() =>
                _service.CreateUser(_createUser, _strongPass));
            Assert.Equal("Address null or empty!", e.Message);
            _mock.Verify(repo => repo.CreateUser(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData(123456789)]
        [InlineData(12345)]
        public void CreateUserInvalidSecretNumberExpectArgumentExceptionTest(int secret)
        {
            _createUser.Customer.SecretNumber = secret;
            Exception e = Assert.Throws<ArgumentException>(() =>
                _service.CreateUser(_createUser, _strongPass));
            Assert.Equal("Invalid secret!", e.Message);
            _mock.Verify(repo => repo.CreateUser(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("Fakeemail@fakemail")]
        [InlineData("Wrongma@.com")]
        public void CreateUserInvalidEmailExpectArgumentExceptionTest(string mail)
        {
            _createUser.Email = mail;

            Exception e = Assert.Throws<ArgumentException>(() =>
                _service.CreateUser(_createUser, _strongPass));
            Assert.Equal("Email not accepted!", e.Message);
            _mock.Verify(repo => repo.CreateUser(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData("poor")]
        [InlineData("secret")]
        public void CreateUserInvalidPasswordNameExpectArgumentExceptionTest(string password)
        {

            Exception e = Assert.Throws<ArgumentException>(() =>
                _service.CreateUser(_createUser, password));
            Assert.Equal("Password too weak!", e.Message);
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
                Customer = new Customer
                {
                    Id = 1,
                    Address = "Vejlevej 22",
                    Firstname = "Kenneth",
                    Lastname = "Pedersen",
                    SecretNumber = 0910951337,
                },
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
                Customer = new Customer
                {
                    Id = 2,
                    Address = "Vejlevej 22",
                    Firstname = "Kenneth",
                    Lastname = "Pedersen",
                    SecretNumber = 0910951337,
                },
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