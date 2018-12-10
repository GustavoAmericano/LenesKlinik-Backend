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
    }
}