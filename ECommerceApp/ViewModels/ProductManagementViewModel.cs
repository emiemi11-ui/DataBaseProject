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
    /// ViewModel for Product Management.
    /// Handles CRUD operations for products.
    /// </summary>
    public class ProductManagementViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private ObservableCollection<Product> _products;
        private ObservableCollection<Category> _categories;
        private Product _selectedProduct;
        private string _searchText;
        private bool _isEditing;
        private string _errorMessage;
        private string _successMessage;

        // Edit form fields
        private string _editProductName;
        private string _editDescription;
        private decimal _editPrice;
        private Category _editCategory;
        private string _editImageURL;

        public ProductManagementViewModel()
        {
            _productService = new ProductService();
            Products = new ObservableCollection<Product>();
            Categories = new ObservableCollection<Category>();

            AddProductCommand = new RelayCommand(ExecuteAddProduct, CanExecuteAddProduct);
            EditProductCommand = new RelayCommand(ExecuteEditProduct, CanExecuteEditProduct);
            DeleteProductCommand = new RelayCommand(ExecuteDeleteProduct, CanExecuteDeleteProduct);
            SaveProductCommand = new RelayCommand(ExecuteSaveProduct, CanExecuteSaveProduct);
            CancelEditCommand = new RelayCommand(ExecuteCancelEdit);
            SearchCommand = new RelayCommand(ExecuteSearch);
            RefreshCommand = new RelayCommand(ExecuteRefresh);

            LoadData();
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
        /// Gets or sets the selected product.
        /// </summary>
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
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
        /// Gets or sets whether the edit form is visible.
        /// </summary>
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
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

        // Edit form properties
        public string EditProductName
        {
            get => _editProductName;
            set => SetProperty(ref _editProductName, value);
        }

        public string EditDescription
        {
            get => _editDescription;
            set => SetProperty(ref _editDescription, value);
        }

        public decimal EditPrice
        {
            get => _editPrice;
            set => SetProperty(ref _editPrice, value);
        }

        public Category EditCategory
        {
            get => _editCategory;
            set => SetProperty(ref _editCategory, value);
        }

        public string EditImageURL
        {
            get => _editImageURL;
            set => SetProperty(ref _editImageURL, value);
        }

        public bool IsNewProduct { get; private set; }

        // Commands
        public ICommand AddProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand SaveProductCommand { get; }
        public ICommand CancelEditCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }

        private void LoadData()
        {
            try
            {
                ClearMessages();

                var products = _productService.GetAllProducts();
                Products = new ObservableCollection<Product>(products);

                var categories = _productService.GetAllCategories();
                Categories = new ObservableCollection<Category>(categories);
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Failed to load data: {ex.Message}";
            }
        }

        private void ClearMessages()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        private bool CanExecuteAddProduct(object parameter) => !IsEditing;

        private void ExecuteAddProduct(object parameter)
        {
            ClearMessages();
            IsNewProduct = true;
            IsEditing = true;

            EditProductName = string.Empty;
            EditDescription = string.Empty;
            EditPrice = 0;
            EditCategory = Categories.FirstOrDefault();
            EditImageURL = string.Empty;
        }

        private bool CanExecuteEditProduct(object parameter) => SelectedProduct != null && !IsEditing;

        private void ExecuteEditProduct(object parameter)
        {
            if (SelectedProduct == null) return;

            ClearMessages();
            IsNewProduct = false;
            IsEditing = true;

            EditProductName = SelectedProduct.ProductName;
            EditDescription = SelectedProduct.Description;
            EditPrice = SelectedProduct.Price;
            EditCategory = Categories.FirstOrDefault(c => c.CategoryID == SelectedProduct.CategoryID);
            EditImageURL = SelectedProduct.ImageURL;
        }

        private bool CanExecuteDeleteProduct(object parameter) => SelectedProduct != null && !IsEditing;

        private void ExecuteDeleteProduct(object parameter)
        {
            if (SelectedProduct == null) return;

            ClearMessages();

            try
            {
                if (_productService.DeleteProduct(SelectedProduct.ProductID))
                {
                    Products.Remove(SelectedProduct);
                    SelectedProduct = null;
                    SuccessMessage = "Product deleted successfully.";
                }
                else
                {
                    ErrorMessage = "Failed to delete product.";
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error deleting product: {ex.Message}";
            }
        }

        private bool CanExecuteSaveProduct(object parameter)
        {
            return IsEditing &&
                   !string.IsNullOrWhiteSpace(EditProductName) &&
                   EditPrice >= 0 &&
                   EditCategory != null;
        }

        private void ExecuteSaveProduct(object parameter)
        {
            ClearMessages();

            try
            {
                if (IsNewProduct)
                {
                    var newProduct = new Product
                    {
                        ProductName = EditProductName,
                        Description = EditDescription,
                        Price = EditPrice,
                        CategoryID = EditCategory.CategoryID,
                        StoreOwnerID = CurrentUserStore.Instance.CurrentUser.UserID,
                        ImageURL = EditImageURL,
                        IsActive = true
                    };

                    if (_productService.AddProduct(newProduct))
                    {
                        LoadData();
                        SuccessMessage = "Product added successfully.";
                    }
                    else
                    {
                        ErrorMessage = "Failed to add product.";
                    }
                }
                else
                {
                    SelectedProduct.ProductName = EditProductName;
                    SelectedProduct.Description = EditDescription;
                    SelectedProduct.Price = EditPrice;
                    SelectedProduct.CategoryID = EditCategory.CategoryID;
                    SelectedProduct.ImageURL = EditImageURL;

                    if (_productService.UpdateProduct(SelectedProduct))
                    {
                        LoadData();
                        SuccessMessage = "Product updated successfully.";
                    }
                    else
                    {
                        ErrorMessage = "Failed to update product.";
                    }
                }

                IsEditing = false;
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Error saving product: {ex.Message}";
            }
        }

        private void ExecuteCancelEdit(object parameter)
        {
            IsEditing = false;
            ClearMessages();
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
                    var results = _productService.SearchProducts(SearchText);
                    Products = new ObservableCollection<Product>(results);
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Search failed: {ex.Message}";
            }
        }

        private void ExecuteRefresh(object parameter)
        {
            SearchText = string.Empty;
            LoadData();
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
