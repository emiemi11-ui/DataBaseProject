using ECommerceApp.Models;
using System.Collections.Generic;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Interface for inventory-related operations.
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Gets all inventory items with their products.
        /// </summary>
        List<Inventory> GetAllInventory();

        /// <summary>
        /// Gets inventory for a specific product.
        /// </summary>
        Inventory GetInventoryByProductId(int productId);

        /// <summary>
        /// Gets all low stock items (below minimum stock level).
        /// </summary>
        List<Inventory> GetLowStockItems();

        /// <summary>
        /// Updates the stock quantity for a product.
        /// </summary>
        bool UpdateStock(int productId, int quantity);

        /// <summary>
        /// Adds stock to a product.
        /// </summary>
        bool AddStock(int productId, int quantity);

        /// <summary>
        /// Removes stock from a product.
        /// </summary>
        bool RemoveStock(int productId, int quantity);

        /// <summary>
        /// Sets the minimum stock level for a product.
        /// </summary>
        bool SetMinimumStock(int productId, int minimumStock);

        /// <summary>
        /// Checks if there is sufficient stock for an order.
        /// </summary>
        bool HasSufficientStock(int productId, int requestedQuantity);
    }
}
