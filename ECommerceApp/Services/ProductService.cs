using ECommerceApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Service for product-related operations.
    /// Uses ECommerceEntities (DB First - EDMX generated context)
    /// Implements LINQ to Entities queries - Curs 10, pag. 11-14
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

        /// <summary>
        /// Gets all products with Eager Loading (Curs 10, pag. 12)
        /// Uses Include() for navigation properties
        /// </summary>
        public List<Product> GetAllProducts()
        {
            return _context.Products
                .Include(p => p.Category)        // Eager loading
                .Include(p => p.Inventory)       // Eager loading
                .Include(p => p.StoreOwner)      // Eager loading
                .Include(p => p.Tags)            // Many-to-Many
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        /// <summary>
        /// Gets active products using LINQ Where clause
        /// </summary>
        public List<Product> GetActiveProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Include(p => p.Tags)
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        /// <summary>
        /// Gets product by ID with Explicit Loading (Curs 10, pag. 13)
        /// Uses Entry().Reference().Load() and Entry().Collection().Load()
        /// </summary>
        public Product GetProductById(int productId)
        {
            var product = _context.Products.Find(productId);

            if (product != null)
            {
                // Explicit Loading - Conform Curs 10
                _context.Entry(product).Reference(p => p.Category).Load();
                _context.Entry(product).Reference(p => p.Inventory).Load();
                _context.Entry(product).Reference(p => p.StoreOwner).Load();
                _context.Entry(product).Collection(p => p.Reviews).Load();
                _context.Entry(product).Collection(p => p.Tags).Load();
                _context.Entry(product).Collection(p => p.OrderDetails).Load();
            }

            return product;
        }

        /// <summary>
        /// Gets products by category with filtering
        /// </summary>
        public List<Product> GetProductsByCategory(int categoryId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Include(p => p.Tags)
                .Where(p => p.CategoryID == categoryId && p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        /// <summary>
        /// Gets products by store owner
        /// </summary>
        public List<Product> GetProductsByStoreOwner(int storeOwnerId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Include(p => p.Tags)
                .Where(p => p.StoreOwnerID == storeOwnerId)
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        /// <summary>
        /// Searches products - Complex LINQ query
        /// </summary>
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
                .Include(p => p.Tags)
                .Where(p => p.IsActive &&
                    (p.ProductName.ToLower().Contains(searchTerm) ||
                     p.Description.ToLower().Contains(searchTerm) ||
                     p.Category.CategoryName.ToLower().Contains(searchTerm)))
                .OrderBy(p => p.ProductName)
                .ToList();
        }

        /// <summary>
        /// Adds a new product with automatic inventory creation
        /// </summary>
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

        /// <summary>
        /// Updates an existing product
        /// </summary>
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

        /// <summary>
        /// Soft delete - marks product as inactive
        /// </summary>
        public bool DeleteProduct(int productId)
        {
            try
            {
                var product = _context.Products.Find(productId);
                if (product == null)
                {
                    return false;
                }

                product.IsActive = false;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets all categories
        /// </summary>
        public List<Category> GetAllCategories()
        {
            return _context.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.CategoryName)
                .ToList();
        }

        /// <summary>
        /// Gets category by ID
        /// </summary>
        public Category GetCategoryById(int categoryId)
        {
            return _context.Categories.Find(categoryId);
        }

        // =====================================================
        // Tag Management Methods (Many-to-Many) - Curs 10
        // =====================================================

        /// <summary>
        /// Gets all tags
        /// </summary>
        public List<Tag> GetAllTags()
        {
            return _context.Tags
                .OrderBy(t => t.TagName)
                .ToList();
        }

        /// <summary>
        /// Gets tags for a specific product
        /// </summary>
        public List<Tag> GetProductTags(int productId)
        {
            var product = _context.Products
                .Include(p => p.Tags)
                .FirstOrDefault(p => p.ProductID == productId);

            return product?.Tags.ToList() ?? new List<Tag>();
        }

        /// <summary>
        /// Adds a tag to a product (Many-to-Many relationship)
        /// </summary>
        public bool AddTagToProduct(int productId, int tagId)
        {
            try
            {
                var product = _context.Products
                    .Include(p => p.Tags)
                    .FirstOrDefault(p => p.ProductID == productId);

                var tag = _context.Tags.Find(tagId);

                if (product == null || tag == null) return false;

                if (!product.Tags.Contains(tag))
                {
                    product.Tags.Add(tag);
                    _context.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Removes a tag from a product
        /// </summary>
        public bool RemoveTagFromProduct(int productId, int tagId)
        {
            try
            {
                var product = _context.Products
                    .Include(p => p.Tags)
                    .FirstOrDefault(p => p.ProductID == productId);

                var tag = product?.Tags.FirstOrDefault(t => t.TagID == tagId);

                if (product == null || tag == null) return false;

                product.Tags.Remove(tag);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
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
