using ECommerceApp.Commands;
using ECommerceApp.Services;
using ECommerceApp.Stores;
using System.Windows.Input;

namespace ECommerceApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Login view.
    /// Handles user authentication and navigation based on role.
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private string _username;
        private string _password;
        private string _errorMessage;
        private bool _isLoading;

        public LoginViewModel()
        {
            _userService = new UserService();
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    ErrorMessage = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    ErrorMessage = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the error message to display.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        /// <summary>
        /// Gets or sets whether the login is in progress.
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        /// <summary>
        /// Gets the login command.
        /// </summary>
        public ICommand LoginCommand { get; }

        private bool CanExecuteLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !IsLoading;
        }

        private void ExecuteLogin(object parameter)
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var user = _userService.Authenticate(Username, Password);

                if (user == null)
                {
                    ErrorMessage = "Invalid username or password.";
                    return;
                }

                if (!user.IsActive)
                {
                    ErrorMessage = "This account has been deactivated.";
                    return;
                }

                // Store the current user
                CurrentUserStore.Instance.CurrentUser = user;

                // Navigate based on role
                NavigateBasedOnRole(user.UserRole);
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void NavigateBasedOnRole(string role)
        {
            ViewModelBase targetViewModel;

            switch (role)
            {
                case UserRoles.StoreOwner:
                    targetViewModel = new StoreOwnerDashboardViewModel();
                    break;

                case UserRoles.Customer:
                    targetViewModel = new CustomerShopViewModel();
                    break;

                case UserRoles.CustomerService:
                    targetViewModel = new CustomerServiceViewModel();
                    break;

                default:
                    ErrorMessage = "Unknown user role.";
                    return;
            }

            NavigationStore.Instance.Navigate(targetViewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userService is System.IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
