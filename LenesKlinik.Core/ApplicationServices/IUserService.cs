using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.ApplicationServices
{
    public interface IUserService
    {
        /// <summary>
        /// Validates and saves the user + customer entity within to the database.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="clearPass"></param>
        /// <returns>The saved User entity.</returns>
        void CreateUser(User user,string clearPass);

        /// <summary>
        /// Validates existance of user, and matches the password with the saved hashed format. 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>A User entity, if validation succeeds, null if not.</returns>
        User ValidateUser(string email, string password);

        void UpdateUser(User user, string clearPass, string newPass);
    }
}