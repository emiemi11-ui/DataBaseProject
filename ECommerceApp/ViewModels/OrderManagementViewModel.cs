using ECommerceApp.Commands;
using ECommerceApp.Models;
using ECommerceApp.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ECommerceApp.ViewModels
{
    /// <summary>
    /// ViewModel for Order Management (Store Owner view).
    /// Handles order status updates and viewing.
    /// </summary>
    public class OrderManagementViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        private ObservableCollection<Order> _orders;
        private Order _selectedOrder;
        private ObservableCollection<OrderDetail> _orderDetails;
        private string _selectedStatusFilter;
        private string _errorMessage;
        private string _successMessage;

        public OrderManagementViewModel()
        {
            _orderService = new OrderService();
            Orders = new ObservableCollection<Order>();
            OrderDetails = new ObservableCollection<OrderDetail>();
            StatusOptions = new ObservableCollection<string>(OrderStatuses.All);

            UpdateStatusCommand = new RelayCommand(ExecuteUpdateStatus, CanExecuteUpdateStatus);
            FilterByStatusCommand = new RelayCommand(ExecuteFilterByStatus);
            RefreshCommand = new RelayCommand(ExecuteRefresh);
            ViewDetailsCommand = new RelayCommand(ExecuteViewDetails, CanExecuteViewDetails);

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
        /// Gets the available status options.
        /// </summary>
        public ObservableCollection<string> StatusOptions { get; }

        /// <summary>
        /// Gets or sets the selected status filter.
        /// </summary>
        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set => SetProperty(ref _selectedStatusFilter, value);
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
        public ICommand UpdateStatusCommand { get; }
        public ICommand FilterByStatusCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ViewDetailsCommand { get; }

        private void LoadData()
        {
            try
            {
                ClearMessages();

                var orders = _orderService.GetAllOrders();
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

        private bool CanExecuteUpdateStatus(object parameter)
        {
            return SelectedOrder != null && parameter is string;
        }

        private void ExecuteUpdateStatus(object parameter)
        {
            if (SelectedOrder == null || !(parameter is string newStatus)) return;

            ClearMessages();

            try
            {
                if (_orderService.UpdateOrderStatus(SelectedOrder.OrderID, newStatus))
                {
                    SuccessMessage = $"Order status updated to {newStatus}.";
                    LoadData();
                }
                else
                {
                    ErrorMessage = "Failed to update order status.";
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error updating status: {ex.Message}";
            }
        }

        private void ExecuteFilterByStatus(object parameter)
        {
            ClearMessages();

            try
            {
                if (string.IsNullOrEmpty(SelectedStatusFilter) || SelectedStatusFilter == "All")
                {
                    LoadData();
                }
                else
                {
                    var filtered = _orderService.GetOrdersByStatus(SelectedStatusFilter);
                    Orders = new ObservableCollection<Order>(filtered);
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Filter failed: {ex.Message}";
            }
        }

        private void ExecuteRefresh(object parameter)
        {
            SelectedStatusFilter = null;
            LoadData();
        }

        private bool CanExecuteViewDetails(object parameter) => SelectedOrder != null;

        private void ExecuteViewDetails(object parameter)
        {
            LoadOrderDetails();
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
