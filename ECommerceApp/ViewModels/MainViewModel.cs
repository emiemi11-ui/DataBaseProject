using ECommerceApp.Commands;
using ECommerceApp.Stores;
using System.Windows.Input;

namespace ECommerceApp.ViewModels
{
    /// <summary>
    /// Main ViewModel that handles navigation and hosts the current view.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;
        private readonly CurrentUserStore _currentUserStore;

        public MainViewModel()
        {
            _navigationStore = NavigationStore.Instance;
            _currentUserStore = CurrentUserStore.Instance;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _currentUserStore.CurrentUserChanged += OnCurrentUserChanged;

            LogoutCommand = new RelayCommand(ExecuteLogout, CanExecuteLogout);

            // Start with login view
            _navigationStore.Navigate(new LoginViewModel());
        }

        /// <summary>
        /// Gets the current ViewModel being displayed.
        /// </summary>
        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;

        /// <summary>
        /// Gets whether a user is currently logged in.
        /// </summary>
        public bool IsLoggedIn => _currentUserStore.IsLoggedIn;

        /// <summary>
        /// Gets the current user's name.
        /// </summary>
        public string CurrentUserName => _currentUserStore.CurrentUser?.Username ?? string.Empty;

        /// <summary>
        /// Gets the current user's role.
        /// </summary>
        public string CurrentUserRole => _currentUserStore.UserRole ?? string.Empty;

        /// <summary>
        /// Gets the logout command.
        /// </summary>
        public ICommand LogoutCommand { get; }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        private void OnCurrentUserChanged()
        {
            OnPropertiesChanged(nameof(IsLoggedIn), nameof(CurrentUserName), nameof(CurrentUserRole));
        }

        private bool CanExecuteLogout(object parameter)
        {
            return IsLoggedIn;
        }

        private void ExecuteLogout(object parameter)
        {
            _currentUserStore.Logout();
            _navigationStore.Navigate(new LoginViewModel());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _navigationStore.CurrentViewModelChanged -= OnCurrentViewModelChanged;
                _currentUserStore.CurrentUserChanged -= OnCurrentUserChanged;
            }
            base.Dispose(disposing);
        }
    }
}
