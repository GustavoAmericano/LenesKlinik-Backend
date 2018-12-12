using System;
using System.IO;
using System.Linq;
using LenesKlinik.Core.DomainServices;
using LenesKlinik.Core.Entities;
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

        public User GetUserByMail(string email)
        {
            var userFromDb = _ctx.Users.Include(user => user.Customer).FirstOrDefault(u => u.Email.Equals(email));
            
            if(userFromDb == null) throw new InvalidDataException("User Not Found");

            return userFromDb;
        }

        public bool CheckEmailInUse(string email)
        {
            if (_ctx.Users.FirstOrDefault(user => user.Email.Equals(email)) != null) return true;
            return false;
        }

        public User GetUserById(int userId)
        {
            try
            {
                User u =  _ctx.Users.FirstOrDefault(user => user.Id == userId);
                _ctx.Entry(u).State = EntityState.Detached;
                return u;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to fetch user from DB!");
            }
        }

        public User UpdateUser(User user)
        {
            try
            {
                _ctx.Attach(user).State = EntityState.Modified;
                _ctx.Attach(user.Customer).State = EntityState.Modified;
                _ctx.SaveChanges();
                return user;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to update user in DB!");
            }
        }
    }
}