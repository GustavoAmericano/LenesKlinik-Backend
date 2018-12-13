using System.Collections.Generic;
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
        User CreateUser(User user,string clearPass);

        /// <summary>
        /// Validates existance of user, and matches the password with the saved hashed format. 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>A User entity, if validation succeeds, null if not.</returns>
        User ValidateUser(string email, string password);

        /// <summary>
        /// Validates the given user and customer entities data and saves it.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="clearPass"></param>
        /// <param name="newPass"></param>
        /// <returns></returns>
        User UpdateUser(User user, string clearPass, string newPass);

        /// <summary>
        /// Retrieves the user with the given Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        User GetUserById(int id);

        /// <summary>
        /// Fetches all users in the Database.
        /// </summary>
        /// <returns></returns>
        List<User> GetAllUsers();
    }
}