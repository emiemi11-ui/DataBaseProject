using System;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Represents inventory information for a product.
    /// </summary>
    public partial class Inventory
    {
        public int InventoryID { get; set; }
        public int ProductID { get; set; }
        public int StockQuantity { get; set; }
        public int MinimumStock { get; set; }
        public DateTime LastUpdated { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; }

        /// <summary>
        /// Checks if the stock is below the minimum threshold
        /// </summary>
        public bool IsLowStock => StockQuantity < MinimumStock;
    }
}
