using System;
using System.Collections.Generic;
using System.Linq;
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
                    Birthdate = DateTime.Now.AddDays(-73).AddYears(22),
                    PhoneNumber = 51158200
                },
                Email = "Email@mail.com",
            };
            _mock.Setup(repo => repo.CheckEmailInUse(It.IsAny<string>()))
                .Returns<string>(email =>
                {
                    if (GetMockUsers().FirstOrDefault(user => user.Email.Equals(email)) != null) return true;
                    return false;
                });
            _mock.Setup(repo => repo.CreateUser(It.IsAny<User>())).Returns<User>(user =>
            {
                user.Id = 1337;
                return user;
            });
            _mock.Setup(repo => repo.UpdateUser(It.IsAny<User>())).Returns<User>(u => u);
            _mock.Setup(repo => repo.GetUserById(It.IsAny<int>())).Returns<int>(id => GetMockUsers().ToList()[id-1]);
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
        public void CreateUserNoLastNameExpectArgumentExceptionTest()
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

        [Fact]
        public void CreateUserEmailInUserExpectException()
        {
            _createUser.Email = "Admin@lk.dk";

            Exception e = Assert.Throws<ArgumentException>(() => _service.CreateUser(_createUser, _strongPass));
            Assert.Equal("Email already in use!" , e.Message);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(1234567)]
        public void CreateUserInvalidPhoneNumberExpectArgumentExceptionTest(int num)
        {
            _createUser.Customer.PhoneNumber = num;
            Exception e = Assert.Throws<ArgumentException>(() =>
                _service.CreateUser(_createUser, _strongPass));
            Assert.Equal("Invalid phone number!", e.Message);
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


        #region UPDATE
        [Fact]
        public void UpdateUserNoNewPassSuccessTest()
        {
            User u = GetMockUsers().ToList()[0];
            User returnUser = _service.UpdateUser(u, "4dm1n", null);
            Assert.Equal(1, returnUser.Id);
            _mock.Verify(repo => repo.UpdateUser(u), Times.Once);
        }

        [Fact]
        public void UpdateUserNewPassSuccessTest()
        {
            User u = GetMockUsers().ToList()[0];
            var hash = u.PasswordHash;
            User returnUser = _service.UpdateUser(u, "4dm1n", "NewStrongPass");
            Assert.Equal(1, returnUser.Id);
            Assert.NotEqual(hash, returnUser.PasswordHash);
            _mock.Verify(repo => repo.UpdateUser(u), Times.Once);
        }

        [Fact]
        public void UpdateUserBadNewPassExpectArgumentExceptionTest()
        {
            User u = GetMockUsers().ToList()[0];
            Exception e = Assert.Throws<ArgumentException>(() => _service.UpdateUser(u, "4dm1n", "weakpw"));
            _mock.Verify(repo => repo.UpdateUser(u), Times.Never);
            Assert.Equal("Password too weak!", e.Message);
        }

        [Fact]
        public void UpdateUserNoFirstNameExpectArgumentExceptionTest()
        {
            User u = GetMockUsers().ToList()[0];
            u.Customer.Firstname = null;
            Exception e = Assert.Throws<ArgumentException>(() => _service.UpdateUser(u, "4dm1n", null));
            _mock.Verify(repo => repo.UpdateUser(u), Times.Never);
            Assert.Equal("Firstname null or empty!", e.Message);
        }

        [Fact]
        public void UpdateUserNoLastNameExpectArgumentExceptionTest()
        {
            User u = GetMockUsers().ToList()[0];
            u.Customer.Lastname = null;
            Exception e = Assert.Throws<ArgumentException>(() => _service.UpdateUser(u, "4dm1n", null));
            _mock.Verify(repo => repo.UpdateUser(u), Times.Never);
            Assert.Equal("Lastname null or empty!", e.Message);
        }

        [Fact]
        public void UpdateUserNoAddressExpectArgumentExceptionTest()
        {
            User u = GetMockUsers().ToList()[0];
            u.Customer.Address = null;
            Exception e = Assert.Throws<ArgumentException>(() => _service.UpdateUser(u, "4dm1n", null));
            _mock.Verify(repo => repo.UpdateUser(u), Times.Never);
            Assert.Equal("Address null or empty!", e.Message);
        }

        [Theory]
        [InlineData(12)]
        [InlineData(1234567)]
        public void UpdateUserInvalidPhoneNumberExpectArgumentExceptionTest(int num)
        {
            User u = GetMockUsers().ToList()[0];
            u.Customer.PhoneNumber = num;
            Exception e = Assert.Throws<ArgumentException>(() => _service.UpdateUser(u, "4dm1n", null));
            _mock.Verify(repo => repo.UpdateUser(u), Times.Never);
            Assert.Equal("Invalid phone number!", e.Message);
        }


        [Theory]
        [InlineData(null)]
        [InlineData("Fakeemail@fakemail")]
        [InlineData("Wrongma@.com")]
        public void UpdateUserInvalidEmailExpectArgumentExceptionTest(string mail)
        {
            User u = GetMockUsers().ToList()[0];
            u.Email = null;
            Exception e = Assert.Throws<ArgumentException>(() => _service.UpdateUser(u, "4dm1n", null));
            _mock.Verify(repo => repo.UpdateUser(u), Times.Never);
            Assert.Equal("Email not accepted!", e.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void GetUserByIdSuccessTest(int id)
        {
            User user = _service.GetUserById(id);
            Assert.Equal(id, user.Id);
        }

        #endregion
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
                    Birthdate = new DateTime(1995,09,10),
                    PhoneNumber = 51158200,
                },
                IsAdmin = true
            };

            salt = GenerateSalt();
            hash = GenerateHash("us3r" + salt);
            var user2 = new User
            {
                Id = 2,
                Email = "user@lk.dk",
                PasswordSalt = salt,
                PasswordHash = hash,
                Customer = new Customer
                {
                    Id = 2,
                    Address = "Vejlevej 22",
                    Firstname = "Kenneth",
                    Lastname = "Pedersen",
                    Birthdate = new DateTime(1970,03,01),
                    PhoneNumber = 51928329
                },
                IsAdmin = true
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