using ECommerceApp.Models;
using System.Collections.Generic;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Interface for user-related operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Authenticates a user with username/email and password.
        /// </summary>
        /// <param name="usernameOrEmail">Username or email</param>
        /// <param name="password">Plain text password</param>
        /// <returns>The authenticated user or null if authentication fails</returns>
        User Authenticate(string usernameOrEmail, string password);

        /// <summary>
        /// Gets a user by their ID.
        /// </summary>
        User GetUserById(int userId);

        /// <summary>
        /// Gets a user by their username.
        /// </summary>
        User GetUserByUsername(string username);

        /// <summary>
        /// Gets all users.
        /// </summary>
        List<User> GetAllUsers();

        /// <summary>
        /// Gets all users by role.
        /// </summary>
        List<User> GetUsersByRole(string role);

        /// <summary>
        /// Creates a new user.
        /// </summary>
        bool CreateUser(User user, string password);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        bool UpdateUser(User user);

        /// <summary>
        /// Checks if a username already exists.
        /// </summary>
        bool UsernameExists(string username);

        /// <summary>
        /// Checks if an email already exists.
        /// </summary>
        bool EmailExists(string email);
    }
}
