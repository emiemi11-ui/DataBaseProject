using ECommerceApp.Commands;
using ECommerceApp.Models;
using ECommerceApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ECommerceApp.ViewModels
{
    /// <summary>
    /// ViewModel for Inventory Management.
    /// Handles stock updates and low stock alerts.
    /// </summary>
    public class InventoryManagementViewModel : ViewModelBase
    {
        private readonly IInventoryService _inventoryService;
        private ObservableCollection<Inventory> _inventoryItems;
        private ObservableCollection<Inventory> _lowStockItems;
        private Inventory _selectedInventory;
        private int _newStockQuantity;
        private string _errorMessage;
        private string _successMessage;
        private bool _showLowStockOnly;

        public InventoryManagementViewModel()
        {
            _inventoryService = new InventoryService();
            InventoryItems = new ObservableCollection<Inventory>();
            LowStockItems = new ObservableCollection<Inventory>();

            UpdateStockCommand = new RelayCommand(ExecuteUpdateStock, CanExecuteUpdateStock);
            AddStockCommand = new RelayCommand(ExecuteAddStock, CanExecuteAddStock);
            RemoveStockCommand = new RelayCommand(ExecuteRemoveStock, CanExecuteRemoveStock);
            RefreshCommand = new RelayCommand(ExecuteRefresh);
            ToggleLowStockFilterCommand = new RelayCommand(ExecuteToggleLowStockFilter);

            LoadData();
        }

        /// <summary>
        /// Gets or sets the collection of inventory items.
        /// </summary>
        public ObservableCollection<Inventory> InventoryItems
        {
            get => _inventoryItems;
            set => SetProperty(ref _inventoryItems, value);
        }

        /// <summary>
        /// Gets or sets the collection of low stock items.
        /// </summary>
        public ObservableCollection<Inventory> LowStockItems
        {
            get => _lowStockItems;
            set => SetProperty(ref _lowStockItems, value);
        }

        /// <summary>
        /// Gets or sets the selected inventory item.
        /// </summary>
        public Inventory SelectedInventory
        {
            get => _selectedInventory;
            set
            {
                if (SetProperty(ref _selectedInventory, value))
                {
                    NewStockQuantity = value?.StockQuantity ?? 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets the new stock quantity for updates.
        /// </summary>
        public int NewStockQuantity
        {
            get => _newStockQuantity;
            set => SetProperty(ref _newStockQuantity, value);
        }

        /// <summary>
        /// Gets or sets whether to show only low stock items.
        /// </summary>
        public bool ShowLowStockOnly
        {
            get => _showLowStockOnly;
            set => SetProperty(ref _showLowStockOnly, value);
        }

        /// <summary>
        /// Gets the low stock count.
        /// </summary>
        public int LowStockCount => LowStockItems?.Count ?? 0;

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
        public ICommand UpdateStockCommand { get; }
        public ICommand AddStockCommand { get; }
        public ICommand RemoveStockCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ToggleLowStockFilterCommand { get; }

        private void LoadData()
        {
            try
            {
                ClearMessages();

                var items = _inventoryService.GetAllInventory();
                InventoryItems = new ObservableCollection<Inventory>(items);

                var lowStock = _inventoryService.GetLowStockItems();
                LowStockItems = new ObservableCollection<Inventory>(lowStock);

                OnPropertyChanged(nameof(LowStockCount));
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Failed to load inventory: {ex.Message}";
            }
        }

        private void ClearMessages()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        private bool CanExecuteUpdateStock(object parameter)
        {
            return SelectedInventory != null && NewStockQuantity >= 0;
        }

        private void ExecuteUpdateStock(object parameter)
        {
            if (SelectedInventory == null) return;

            ClearMessages();

            try
            {
                if (_inventoryService.UpdateStock(SelectedInventory.ProductID, NewStockQuantity))
                {
                    SuccessMessage = $"Stock updated to {NewStockQuantity} units.";
                    LoadData();
                }
                else
                {
                    ErrorMessage = "Failed to update stock.";
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error updating stock: {ex.Message}";
            }
        }

        private bool CanExecuteAddStock(object parameter)
        {
            return SelectedInventory != null && NewStockQuantity > 0;
        }

        private void ExecuteAddStock(object parameter)
        {
            if (SelectedInventory == null) return;

            ClearMessages();

            try
            {
                if (_inventoryService.AddStock(SelectedInventory.ProductID, NewStockQuantity))
                {
                    SuccessMessage = $"Added {NewStockQuantity} units to stock.";
                    LoadData();
                }
                else
                {
                    ErrorMessage = "Failed to add stock.";
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error adding stock: {ex.Message}";
            }
        }

        private bool CanExecuteRemoveStock(object parameter)
        {
            return SelectedInventory != null &&
                   NewStockQuantity > 0 &&
                   SelectedInventory.StockQuantity >= NewStockQuantity;
        }

        private void ExecuteRemoveStock(object parameter)
        {
            if (SelectedInventory == null) return;

            ClearMessages();

            try
            {
                if (_inventoryService.RemoveStock(SelectedInventory.ProductID, NewStockQuantity))
                {
                    SuccessMessage = $"Removed {NewStockQuantity} units from stock.";
                    LoadData();
                }
                else
                {
                    ErrorMessage = "Failed to remove stock. Check available quantity.";
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error removing stock: {ex.Message}";
            }
        }

        private void ExecuteRefresh(object parameter)
        {
            LoadData();
        }

        private void ExecuteToggleLowStockFilter(object parameter)
        {
            ShowLowStockOnly = !ShowLowStockOnly;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_inventoryService is System.IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
