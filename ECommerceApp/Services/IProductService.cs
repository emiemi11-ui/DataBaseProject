using ECommerceApp.Models;
using System.Collections.Generic;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Interface for product-related operations.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Gets all products with their categories and inventory.
        /// </summary>
        List<Product> GetAllProducts();

        /// <summary>
        /// Gets all active products.
        /// </summary>
        List<Product> GetActiveProducts();

        /// <summary>
        /// Gets a product by its ID.
        /// </summary>
        Product GetProductById(int productId);

        /// <summary>
        /// Gets products by category.
        /// </summary>
        List<Product> GetProductsByCategory(int categoryId);

        /// <summary>
        /// Gets products by store owner.
        /// </summary>
        List<Product> GetProductsByStoreOwner(int storeOwnerId);

        /// <summary>
        /// Searches products by name or description.
        /// </summary>
        List<Product> SearchProducts(string searchTerm);

        /// <summary>
        /// Adds a new product.
        /// </summary>
        bool AddProduct(Product product);

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        bool UpdateProduct(Product product);

        /// <summary>
        /// Deletes a product (soft delete - sets IsActive to false).
        /// </summary>
        bool DeleteProduct(int productId);

        /// <summary>
        /// Gets all categories.
        /// </summary>
        List<Category> GetAllCategories();

        /// <summary>
        /// Gets a category by its ID.
        /// </summary>
        Category GetCategoryById(int categoryId);
    }
}
