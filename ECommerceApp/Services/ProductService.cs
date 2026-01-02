using ECommerceApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Service for product-related operations.
    /// </summary>
    public class ProductService : IProductService, IDisposable
    {
        private readonly ECommerceEntities _context;
        private bool _disposed;

        public ProductService()
        {
            _context = new ECommerceEntities();
        }

        public ProductService(ECommerceEntities context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public List<Product> GetAllProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Include(p => p.StoreOwner)
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        /// <inheritdoc/>
        public List<Product> GetActiveProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        /// <inheritdoc/>
        public Product GetProductById(int productId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Include(p => p.StoreOwner)
                .Include(p => p.Reviews)
                .FirstOrDefault(p => p.ProductID == productId);
        }

        /// <inheritdoc/>
        public List<Product> GetProductsByCategory(int categoryId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => p.CategoryID == categoryId && p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        /// <inheritdoc/>
        public List<Product> GetProductsByStoreOwner(int storeOwnerId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => p.StoreOwnerID == storeOwnerId)
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        /// <inheritdoc/>
        public List<Product> SearchProducts(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetActiveProducts();
            }

            searchTerm = searchTerm.ToLower();

            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => p.IsActive &&
                    (p.ProductName.ToLower().Contains(searchTerm) ||
                     p.Description.ToLower().Contains(searchTerm) ||
                     p.Category.CategoryName.ToLower().Contains(searchTerm)))
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        /// <inheritdoc/>
        public bool AddProduct(Product product)
        {
            try
            {
                product.CreatedDate = DateTime.Now;
                product.IsActive = true;

                _context.Products.Add(product);
                _context.SaveChanges();

                // Create inventory record
                var inventory = new Inventory
                {
                    ProductID = product.ProductID,
                    StockQuantity = 0,
                    MinimumStock = 5,
                    LastUpdated = DateTime.Now
                };
                _context.Inventories.Add(inventory);
                _context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool UpdateProduct(Product product)
        {
            try
            {
                var existingProduct = _context.Products.Find(product.ProductID);
                if (existingProduct == null)
                {
                    return false;
                }

                existingProduct.ProductName = product.ProductName;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.CategoryID = product.CategoryID;
                existingProduct.ImageURL = product.ImageURL;
                existingProduct.IsActive = product.IsActive;

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public bool DeleteProduct(int productId)
        {
            try
            {
                var product = _context.Products.Find(productId);
                if (product == null)
                {
                    return false;
                }

                // Soft delete - just mark as inactive
                product.IsActive = false;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public List<Category> GetAllCategories()
        {
            return _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToList();
        }

        /// <inheritdoc/>
        public Category GetCategoryById(int categoryId)
        {
            return _context.Categories.Find(categoryId);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
