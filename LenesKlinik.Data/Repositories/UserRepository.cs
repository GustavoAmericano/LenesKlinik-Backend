using LenesKlinik.Core.ApplicationServices;
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
    }
}