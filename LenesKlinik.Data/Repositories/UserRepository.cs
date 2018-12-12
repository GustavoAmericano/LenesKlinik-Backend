using System;
using System.Collections.Generic;
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
            var userFromDb = _ctx.Users.Include(user => user.Customer)
                .FirstOrDefault(u => u.Email.ToLower().Equals(email.ToLower()));
            
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
                User u =  _ctx.Users.Include(user => user.Customer).FirstOrDefault(user => user.Id == userId);
                _ctx.Entry(u.Customer).State = EntityState.Detached;
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
                //if (_ctx.ChangeTracker.Entries<User>().Any(u => u.Entity.Id == user.Id))
                //    _ctx.Entry(user).State = EntityState.Detached;

                //if (_ctx.ChangeTracker.Entries<Customer>().Any(cust => cust.Entity.Id == user.Customer.Id))
                //    _ctx.Entry(user.Customer).State = EntityState.Detached;
                _ctx.Attach(user).State = EntityState.Modified;
                _ctx.Customers.Attach(user.Customer).State = EntityState.Modified;
                _ctx.SaveChanges();
                return user;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to update user in DB!");
            }
        }

        public List<User> GetAllUsers()
        {
            return _ctx.Users.Include(user => user.Customer).ToList();
        }
    }
}