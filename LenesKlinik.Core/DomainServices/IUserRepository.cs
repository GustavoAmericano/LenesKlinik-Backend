using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.DomainServices
{
    public interface IUserRepository
    {

        /// <summary>
        /// Saves a user and it's contained Customer entity to the DB.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>The saved entity.</returns>
        User CreateUser(User user);

        /// <summary>
        /// Fetches the first matching user with the given email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>First matching User entity.</returns>
        User GetUserByMail(string email);
    }
}