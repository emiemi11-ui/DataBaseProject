using ECommerceApp.Data;
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
    /// </summary>
    public class UserService : IUserService, IDisposable
    {
        private readonly ECommerceDbContext _context;
        private bool _disposed;

        public UserService()
        {
            _context = new ECommerceDbContext();
        }

        public UserService(ECommerceDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public User GetUserById(int userId)
        {
            return _context.Users.Find(userId);
        }

        /// <inheritdoc/>
        public User GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        /// <inheritdoc/>
        public List<User> GetAllUsers()
        {
            return _context.Users
                .OrderBy(u => u.Username)
                .ToList();
        }

        /// <inheritdoc/>
        public List<User> GetUsersByRole(string role)
        {
            return _context.Users
                .Where(u => u.UserRole == role && u.IsActive)
                .OrderBy(u => u.Username)
                .ToList();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool UsernameExists(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }

        /// <inheritdoc/>
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
