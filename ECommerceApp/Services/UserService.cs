using ECommerceApp.Helpers;
using ECommerceApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Service for user-related operations.
    /// Uses ECommerceEntities (DB First - EDMX generated context)
    /// </summary>
    public class UserService : IUserService, IDisposable
    {
        private readonly ECommerceEntities _context;
        private bool _disposed;

        public UserService()
        {
            _context = new ECommerceEntities();
        }

        public UserService(ECommerceEntities context)
        {
            _context = context;
        }

        /// <summary>
        /// Authenticates a user with username/email and password
        /// </summary>
        public User Authenticate(string usernameOrEmail, string password)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            string hashedPassword = PasswordHelper.ComputeSHA256Hash(password);

            var user = _context.Users
                .FirstOrDefault(u =>
                    (u.Username == usernameOrEmail || u.Email == usernameOrEmail) &&
                    u.HashedPassword == hashedPassword &&
                    u.IsActive);

            return user;
        }

        /// <summary>
        /// Gets user by ID with Explicit Loading
        /// </summary>
        public User GetUserById(int userId)
        {
            var user = _context.Users.Find(userId);

            if (user != null)
            {
                // Explicit Loading - Curs 10
                _context.Entry(user).Collection(u => u.Orders).Load();
                _context.Entry(user).Collection(u => u.CustomerTickets).Load();
                _context.Entry(user).Collection(u => u.Reviews).Load();
            }

            return user;
        }

        /// <summary>
        /// Gets user by username
        /// </summary>
        public User GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        public List<User> GetAllUsers()
        {
            return _context.Users
                .OrderBy(u => u.Username)
                .ToList();
        }

        /// <summary>
        /// Gets users by role
        /// </summary>
        public List<User> GetUsersByRole(string role)
        {
            return _context.Users
                .Where(u => u.UserRole == role && u.IsActive)
                .OrderBy(u => u.Username)
                .ToList();
        }

        /// <summary>
        /// Gets all customers
        /// </summary>
        public List<User> GetAllCustomers()
        {
            return GetUsersByRole("Customer");
        }

        /// <summary>
        /// Gets all customer service agents
        /// </summary>
        public List<User> GetAllSupportAgents()
        {
            return GetUsersByRole("CustomerService");
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        public bool CreateUser(User user, string password)
        {
            try
            {
                if (UsernameExists(user.Username))
                {
                    return false;
                }

                if (EmailExists(user.Email))
                {
                    return false;
                }

                user.HashedPassword = PasswordHelper.ComputeSHA256Hash(password);
                user.CreatedDate = DateTime.Now;
                user.IsActive = true;

                _context.Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        public bool UpdateUser(User user)
        {
            try
            {
                var existingUser = _context.Users.Find(user.UserID);
                if (existingUser == null)
                {
                    return false;
                }

                existingUser.Username = user.Username;
                existingUser.Email = user.Email;
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Address = user.Address;
                existingUser.UserRole = user.UserRole;
                existingUser.IsActive = user.IsActive;

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Updates user password
        /// </summary>
        public bool UpdatePassword(int userId, string newPassword)
        {
            try
            {
                var user = _context.Users.Find(userId);
                if (user == null) return false;

                user.HashedPassword = PasswordHelper.ComputeSHA256Hash(newPassword);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if username exists
        /// </summary>
        public bool UsernameExists(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }

        /// <summary>
        /// Checks if email exists
        /// </summary>
        public bool EmailExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
