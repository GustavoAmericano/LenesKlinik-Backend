using System;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using LenesKlinik.Core.ApplicationServices;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LenesKlinik.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private DataContext _ctx;

        public UserRepository(DataContext ctx)
        {
            _ctx = ctx;
        }

        public User CreateUser(User user)
        {
            _ctx.Attach(user).State = EntityState.Added;
            _ctx.SaveChanges();
            return user;
        }

        public User ValidateUser(string email)
        {
            var userFromDb = _ctx.Users.Include(user => user.Customer).FirstOrDefault(u => u.Email.Equals(email));
            
            if(userFromDb == null) throw new InvalidDataException("User Not Found");

            return userFromDb;
        }

       
    }
}