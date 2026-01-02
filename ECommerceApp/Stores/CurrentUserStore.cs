using ECommerceApp.Models;
using System;

namespace ECommerceApp.Stores
{
    /// <summary>
    /// Stores the currently logged-in user information.
    /// Acts as a singleton for sharing user state across ViewModels.
    /// </summary>
    public class CurrentUserStore
    {
        private static CurrentUserStore _instance;
        private static readonly object _lock = new object();

        private User _currentUser;

        /// <summary>
        /// Gets the singleton instance of the CurrentUserStore.
        /// </summary>
        public static CurrentUserStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CurrentUserStore();
                        }
                    }
                }
                return _instance;
            }
        }

        private CurrentUserStore() { }

        /// <summary>
        /// Gets or sets the currently logged-in user.
        /// </summary>
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                CurrentUserChanged?.Invoke();
            }
        }

        /// <summary>
        /// Gets whether a user is currently logged in.
        /// </summary>
        public bool IsLoggedIn => _currentUser != null;

        /// <summary>
        /// Gets the current user's role.
        /// </summary>
        public string UserRole => _currentUser?.UserRole;

        /// <summary>
        /// Event raised when the current user changes.
        /// </summary>
        public event Action CurrentUserChanged;

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        public void Logout()
        {
            CurrentUser = null;
        }
    }

    /// <summary>
    /// User role constants.
    /// </summary>
    public static class UserRoles
    {
        public const string StoreOwner = "StoreOwner";
        public const string Customer = "Customer";
        public const string CustomerService = "CustomerService";
    }
}
