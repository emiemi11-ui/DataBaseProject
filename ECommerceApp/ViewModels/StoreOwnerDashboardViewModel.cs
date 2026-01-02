using ECommerceApp.Commands;
using ECommerceApp.Stores;
using System.Windows.Input;

namespace ECommerceApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Store Owner Dashboard.
    /// Provides navigation to product and inventory management.
    /// </summary>
    public class StoreOwnerDashboardViewModel : ViewModelBase
    {
        private ViewModelBase _currentContentViewModel;

        public StoreOwnerDashboardViewModel()
        {
            NavigateToProductsCommand = new RelayCommand(ExecuteNavigateToProducts);
            NavigateToInventoryCommand = new RelayCommand(ExecuteNavigateToInventory);
            NavigateToOrdersCommand = new RelayCommand(ExecuteNavigateToOrders);
            LogoutCommand = new RelayCommand(ExecuteLogout);

            // Default to product management
            CurrentContentViewModel = new ProductManagementViewModel();
        }

        /// <summary>
        /// Gets the current user's name.
        /// </summary>
        public string WelcomeMessage => $"Welcome, {CurrentUserStore.Instance.CurrentUser?.Username ?? "Store Owner"}!";

        /// <summary>
        /// Gets or sets the current content ViewModel.
        /// </summary>
        public ViewModelBase CurrentContentViewModel
        {
            get => _currentContentViewModel;
            set => SetProperty(ref _currentContentViewModel, value);
        }

        /// <summary>
        /// Command to navigate to product management.
        /// </summary>
        public ICommand NavigateToProductsCommand { get; }

        /// <summary>
        /// Command to navigate to inventory management.
        /// </summary>
        public ICommand NavigateToInventoryCommand { get; }

        /// <summary>
        /// Command to navigate to orders view.
        /// </summary>
        public ICommand NavigateToOrdersCommand { get; }

        /// <summary>
        /// Command to logout.
        /// </summary>
        public ICommand LogoutCommand { get; }

        private void ExecuteNavigateToProducts(object parameter)
        {
            CurrentContentViewModel = new ProductManagementViewModel();
        }

        private void ExecuteNavigateToInventory(object parameter)
        {
            CurrentContentViewModel = new InventoryManagementViewModel();
        }

        private void ExecuteNavigateToOrders(object parameter)
        {
            CurrentContentViewModel = new OrderManagementViewModel();
        }

        private void ExecuteLogout(object parameter)
        {
            CurrentUserStore.Instance.Logout();
            NavigationStore.Instance.Navigate(new LoginViewModel());
        }
    }
}
