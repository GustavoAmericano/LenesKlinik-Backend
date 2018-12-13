using System;
using System.Collections.Generic;
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


        /// <summary>
        /// Checks if a user with selected email already exists
        /// </summary>
        /// <param name="email"></param>
        /// <returns>True if user exists, False if not.</returns>
        bool CheckEmailInUse(string email);


        /// <summary>
        /// Fetches the user with the given Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        User GetUserById(int userId);

        User UpdateUser(User user);

        List<User> GetAllUsers();
    }
}