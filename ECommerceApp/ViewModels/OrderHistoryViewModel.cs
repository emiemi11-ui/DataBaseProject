using ECommerceApp.Commands;
using ECommerceApp.Models;
using ECommerceApp.Services;
using ECommerceApp.Stores;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ECommerceApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Order History view.
    /// Displays customer's past orders and their details.
    /// </summary>
    public class OrderHistoryViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        private ObservableCollection<Order> _orders;
        private Order _selectedOrder;
        private ObservableCollection<OrderDetail> _orderDetails;
        private string _errorMessage;
        private string _successMessage;

        public OrderHistoryViewModel()
        {
            _orderService = new OrderService();
            Orders = new ObservableCollection<Order>();
            OrderDetails = new ObservableCollection<OrderDetail>();

            ViewDetailsCommand = new RelayCommand(ExecuteViewDetails, CanExecuteViewDetails);
            CancelOrderCommand = new RelayCommand(ExecuteCancelOrder, CanExecuteCancelOrder);
            RefreshCommand = new RelayCommand(ExecuteRefresh);

            LoadData();
        }

        /// <summary>
        /// Gets or sets the collection of orders.
        /// </summary>
        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        /// <summary>
        /// Gets or sets the selected order.
        /// </summary>
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (SetProperty(ref _selectedOrder, value))
                {
                    LoadOrderDetails();
                }
            }
        }

        /// <summary>
        /// Gets or sets the order details for the selected order.
        /// </summary>
        public ObservableCollection<OrderDetail> OrderDetails
        {
            get => _orderDetails;
            set => SetProperty(ref _orderDetails, value);
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
        public ICommand ViewDetailsCommand { get; }
        public ICommand CancelOrderCommand { get; }
        public ICommand RefreshCommand { get; }

        private void LoadData()
        {
            try
            {
                ClearMessages();

                var currentUser = CurrentUserStore.Instance.CurrentUser;
                if (currentUser == null)
                {
                    ErrorMessage = "Please log in to view orders.";
                    return;
                }

                var orders = _orderService.GetOrdersByCustomer(currentUser.UserID);
                Orders = new ObservableCollection<Order>(orders);
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Failed to load orders: {ex.Message}";
            }
        }

        private void LoadOrderDetails()
        {
            if (SelectedOrder == null)
            {
                OrderDetails = new ObservableCollection<OrderDetail>();
                return;
            }

            try
            {
                var details = _orderService.GetOrderDetails(SelectedOrder.OrderID);
                OrderDetails = new ObservableCollection<OrderDetail>(details);
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Failed to load order details: {ex.Message}";
            }
        }

        private void ClearMessages()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        private bool CanExecuteViewDetails(object parameter) => SelectedOrder != null;

        private void ExecuteViewDetails(object parameter)
        {
            LoadOrderDetails();
        }

        private bool CanExecuteCancelOrder(object parameter)
        {
            return SelectedOrder != null &&
                   (SelectedOrder.OrderStatus == OrderStatuses.Pending ||
                    SelectedOrder.OrderStatus == OrderStatuses.Processing);
        }

        private void ExecuteCancelOrder(object parameter)
        {
            if (SelectedOrder == null) return;

            ClearMessages();

            try
            {
                if (_orderService.CancelOrder(SelectedOrder.OrderID))
                {
                    SuccessMessage = $"Order #{SelectedOrder.OrderID} has been cancelled.";
                    LoadData();
                }
                else
                {
                    ErrorMessage = "Failed to cancel order. It may have already been shipped.";
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error cancelling order: {ex.Message}";
            }
        }

        private void ExecuteRefresh(object parameter)
        {
            LoadData();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_orderService is System.IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
