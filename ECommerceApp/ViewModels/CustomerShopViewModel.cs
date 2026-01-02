using ECommerceApp.Commands;
using ECommerceApp.Models;
using ECommerceApp.Services;
using ECommerceApp.Stores;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ECommerceApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Customer Shop view.
    /// Handles product browsing, filtering, and adding to cart.
    /// </summary>
    public class CustomerShopViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private ObservableCollection<Product> _products;
        private ObservableCollection<Category> _categories;
        private ObservableCollection<CartItem> _cartItems;
        private Product _selectedProduct;
        private Category _selectedCategory;
        private string _searchText;
        private string _errorMessage;
        private string _successMessage;
        private ViewModelBase _currentContentViewModel;

        public CustomerShopViewModel()
        {
            _productService = new ProductService();
            Products = new ObservableCollection<Product>();
            Categories = new ObservableCollection<Category>();
            CartItems = new ObservableCollection<CartItem>();

            AddToCartCommand = new RelayCommand(ExecuteAddToCart, CanExecuteAddToCart);
            ViewCartCommand = new RelayCommand(ExecuteViewCart);
            ViewOrdersCommand = new RelayCommand(ExecuteViewOrders);
            FilterByCategoryCommand = new RelayCommand(ExecuteFilterByCategory);
            SearchCommand = new RelayCommand(ExecuteSearch);
            ClearFilterCommand = new RelayCommand(ExecuteClearFilter);
            LogoutCommand = new RelayCommand(ExecuteLogout);
            ViewShopCommand = new RelayCommand(ExecuteViewShop);

            LoadData();
        }

        /// <summary>
        /// Gets the welcome message.
        /// </summary>
        public string WelcomeMessage => $"Welcome, {CurrentUserStore.Instance.CurrentUser?.Username ?? "Customer"}!";

        /// <summary>
        /// Gets or sets the current content ViewModel.
        /// </summary>
        public ViewModelBase CurrentContentViewModel
        {
            get => _currentContentViewModel;
            set => SetProperty(ref _currentContentViewModel, value);
        }

        /// <summary>
        /// Gets or sets the collection of products.
        /// </summary>
        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        /// <summary>
        /// Gets or sets the collection of categories.
        /// </summary>
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        /// <summary>
        /// Gets or sets the shopping cart items.
        /// </summary>
        public ObservableCollection<CartItem> CartItems
        {
            get => _cartItems;
            set => SetProperty(ref _cartItems, value);
        }

        /// <summary>
        /// Gets the cart item count.
        /// </summary>
        public int CartItemCount => CartItems?.Sum(c => c.Quantity) ?? 0;

        /// <summary>
        /// Gets or sets the selected product.
        /// </summary>
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        /// <summary>
        /// Gets or sets the selected category filter.
        /// </summary>
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        /// <summary>
        /// Gets or sets the success message.
        /// </summary>
        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        // Commands
        public ICommand AddToCartCommand { get; }
        public ICommand ViewCartCommand { get; }
        public ICommand ViewOrdersCommand { get; }
        public ICommand FilterByCategoryCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearFilterCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ViewShopCommand { get; }

        private void LoadData()
        {
            try
            {
                ClearMessages();

                var products = _productService.GetActiveProducts()
                    .Where(p => p.Inventory != null && p.Inventory.StockQuantity > 0)
                    .ToList();
                Products = new ObservableCollection<Product>(products);

                var categories = _productService.GetAllCategories();
                Categories = new ObservableCollection<Category>(categories);
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Failed to load products: {ex.Message}";
            }
        }

        private void ClearMessages()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        private bool CanExecuteAddToCart(object parameter) => SelectedProduct != null;

        private void ExecuteAddToCart(object parameter)
        {
            if (SelectedProduct == null) return;

            ClearMessages();

            try
            {
                // Check stock
                if (SelectedProduct.Inventory == null || SelectedProduct.Inventory.StockQuantity == 0)
                {
                    ErrorMessage = "This product is out of stock.";
                    return;
                }

                // Check if already in cart
                var existingItem = CartItems.FirstOrDefault(c => c.ProductID == SelectedProduct.ProductID);
                if (existingItem != null)
                {
                    // Check if we can add more
                    if (existingItem.Quantity >= SelectedProduct.Inventory.StockQuantity)
                    {
                        ErrorMessage = "Cannot add more. Maximum stock reached.";
                        return;
                    }
                    existingItem.Quantity++;
                }
                else
                {
                    CartItems.Add(CartItem.FromProduct(SelectedProduct));
                }

                OnPropertyChanged(nameof(CartItemCount));
                SuccessMessage = $"{SelectedProduct.ProductName} added to cart.";
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error adding to cart: {ex.Message}";
            }
        }

        private void ExecuteViewCart(object parameter)
        {
            CurrentContentViewModel = new ShoppingCartViewModel(CartItems, this);
        }

        private void ExecuteViewOrders(object parameter)
        {
            CurrentContentViewModel = new OrderHistoryViewModel();
        }

        private void ExecuteViewShop(object parameter)
        {
            CurrentContentViewModel = null;
            LoadData();
        }

        private void ExecuteFilterByCategory(object parameter)
        {
            ClearMessages();

            try
            {
                if (SelectedCategory == null)
                {
                    LoadData();
                }
                else
                {
                    var products = _productService.GetProductsByCategory(SelectedCategory.CategoryID)
                        .Where(p => p.Inventory != null && p.Inventory.StockQuantity > 0)
                        .ToList();
                    Products = new ObservableCollection<Product>(products);
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Filter failed: {ex.Message}";
            }
        }

        private void ExecuteSearch(object parameter)
        {
            ClearMessages();

            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    LoadData();
                }
                else
                {
                    var results = _productService.SearchProducts(SearchText)
                        .Where(p => p.Inventory != null && p.Inventory.StockQuantity > 0)
                        .ToList();
                    Products = new ObservableCollection<Product>(results);
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Search failed: {ex.Message}";
            }
        }

        private void ExecuteClearFilter(object parameter)
        {
            SelectedCategory = null;
            SearchText = string.Empty;
            LoadData();
        }

        private void ExecuteLogout(object parameter)
        {
            CurrentUserStore.Instance.Logout();
            NavigationStore.Instance.Navigate(new LoginViewModel());
        }

        /// <summary>
        /// Clears the cart after successful order.
        /// </summary>
        public void ClearCart()
        {
            CartItems.Clear();
            OnPropertyChanged(nameof(CartItemCount));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_productService is System.IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
