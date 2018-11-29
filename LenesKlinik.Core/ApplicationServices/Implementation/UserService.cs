using System;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;
using LenesKlinik.Core.Entities.DTO;

namespace LenesKlinik.Core.ApplicationServices.Implementation
{
    public class UserService : IUserService
    {
        private IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public User CreateUser(UserCreateInput ucInput)
        {
            if(string.IsNullOrEmpty(ucInput.Firstname)) throw new ArgumentException("Firstname null or empty!");
            User user = new User
            {
                Address = ucInput.Address,
                Firstname = ucInput.Firstname,
                Lastname = ucInput.Lastname,
                Email = ucInput.Email,
                SecretNumber = ucInput.SecretNumber,
            };
            return _repo.CreateUser(user);
        }
    }
}