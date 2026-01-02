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
    /// ViewModel for the Shopping Cart view.
    /// Handles cart management and order placement.
    /// </summary>
    public class ShoppingCartViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        private readonly CustomerShopViewModel _parentViewModel;
        private ObservableCollection<CartItem> _cartItems;
        private string _shippingAddress;
        private string _errorMessage;
        private string _successMessage;
        private bool _isPlacingOrder;

        public ShoppingCartViewModel(ObservableCollection<CartItem> cartItems, CustomerShopViewModel parentViewModel)
        {
            _orderService = new OrderService();
            _cartItems = cartItems;
            _parentViewModel = parentViewModel;

            UpdateQuantityCommand = new RelayCommand(ExecuteUpdateQuantity);
            RemoveFromCartCommand = new RelayCommand(ExecuteRemoveFromCart);
            PlaceOrderCommand = new RelayCommand(ExecutePlaceOrder, CanExecutePlaceOrder);
            ContinueShoppingCommand = new RelayCommand(ExecuteContinueShopping);
            ClearCartCommand = new RelayCommand(ExecuteClearCart, CanExecuteClearCart);

            // Subscribe to quantity changes
            foreach (var item in _cartItems)
            {
                item.PropertyChanged += CartItem_PropertyChanged;
            }
        }

        /// <summary>
        /// Gets or sets the cart items.
        /// </summary>
        public ObservableCollection<CartItem> CartItems
        {
            get => _cartItems;
            set => SetProperty(ref _cartItems, value);
        }

        /// <summary>
        /// Gets the total amount.
        /// </summary>
        public decimal TotalAmount => CartItems?.Sum(c => c.Subtotal) ?? 0;

        /// <summary>
        /// Gets the formatted total.
        /// </summary>
        public string FormattedTotal => TotalAmount.ToString("C2");

        /// <summary>
        /// Gets whether the cart is empty.
        /// </summary>
        public bool IsCartEmpty => CartItems == null || CartItems.Count == 0;

        /// <summary>
        /// Gets or sets the shipping address.
        /// </summary>
        public string ShippingAddress
        {
            get => _shippingAddress;
            set => SetProperty(ref _shippingAddress, value);
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

        /// <summary>
        /// Gets or sets whether an order is being placed.
        /// </summary>
        public bool IsPlacingOrder
        {
            get => _isPlacingOrder;
            set => SetProperty(ref _isPlacingOrder, value);
        }

        // Commands
        public ICommand UpdateQuantityCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand PlaceOrderCommand { get; }
        public ICommand ContinueShoppingCommand { get; }
        public ICommand ClearCartCommand { get; }

        private void CartItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartItem.Quantity) || e.PropertyName == nameof(CartItem.Subtotal))
            {
                OnPropertyChanged(nameof(TotalAmount));
                OnPropertyChanged(nameof(FormattedTotal));
            }
        }

        private void ClearMessages()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        private void ExecuteUpdateQuantity(object parameter)
        {
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(FormattedTotal));
        }

        private void ExecuteRemoveFromCart(object parameter)
        {
            if (parameter is CartItem item)
            {
                item.PropertyChanged -= CartItem_PropertyChanged;
                CartItems.Remove(item);
                OnPropertyChanged(nameof(TotalAmount));
                OnPropertyChanged(nameof(FormattedTotal));
                OnPropertyChanged(nameof(IsCartEmpty));
                _parentViewModel?.OnPropertyChanged("CartItemCount");
            }
        }

        private bool CanExecutePlaceOrder(object parameter)
        {
            return !IsCartEmpty &&
                   !string.IsNullOrWhiteSpace(ShippingAddress) &&
                   !IsPlacingOrder;
        }

        private void ExecutePlaceOrder(object parameter)
        {
            ClearMessages();
            IsPlacingOrder = true;

            try
            {
                var currentUser = CurrentUserStore.Instance.CurrentUser;
                if (currentUser == null)
                {
                    ErrorMessage = "Please log in to place an order.";
                    return;
                }

                var order = _orderService.CreateOrder(
                    currentUser.UserID,
                    ShippingAddress,
                    CartItems.ToList()
                );

                if (order != null)
                {
                    SuccessMessage = $"Order #{order.OrderID} placed successfully! Total: {TotalAmount:C2}";

                    // Clear the cart
                    foreach (var item in CartItems)
                    {
                        item.PropertyChanged -= CartItem_PropertyChanged;
                    }
                    CartItems.Clear();
                    _parentViewModel?.ClearCart();

                    OnPropertyChanged(nameof(TotalAmount));
                    OnPropertyChanged(nameof(FormattedTotal));
                    OnPropertyChanged(nameof(IsCartEmpty));
                }
                else
                {
                    ErrorMessage = "Failed to place order. Please try again.";
                }
            }
            catch (System.InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error placing order: {ex.Message}";
            }
            finally
            {
                IsPlacingOrder = false;
            }
        }

        private void ExecuteContinueShopping(object parameter)
        {
            _parentViewModel?.ViewShopCommand?.Execute(null);
        }

        private bool CanExecuteClearCart(object parameter) => !IsCartEmpty;

        private void ExecuteClearCart(object parameter)
        {
            foreach (var item in CartItems)
            {
                item.PropertyChanged -= CartItem_PropertyChanged;
            }
            CartItems.Clear();
            _parentViewModel?.ClearCart();

            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(FormattedTotal));
            OnPropertyChanged(nameof(IsCartEmpty));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var item in CartItems)
                {
                    item.PropertyChanged -= CartItem_PropertyChanged;
                }

                if (_orderService is System.IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
