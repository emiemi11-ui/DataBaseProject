using System;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Partial class for Inventory - Business logic extensions
    /// Extends the auto-generated Inventory class from EDMX
    /// </summary>
    public partial class Inventory
    {
        /// <summary>
        /// Checks if stock is below minimum threshold
        /// </summary>
        public bool IsLowStock => StockQuantity < MinimumStock;

        /// <summary>
        /// Checks if out of stock
        /// </summary>
        public bool IsOutOfStock => StockQuantity <= 0;

        /// <summary>
        /// Gets the stock status text
        /// </summary>
        public string StockStatus
        {
            get
            {
                if (StockQuantity <= 0) return "Out of Stock";
                if (IsLowStock) return "Low Stock";
                return "In Stock";
            }
        }

        /// <summary>
        /// Gets the stock status color
        /// </summary>
        public string StockStatusColor
        {
            get
            {
                if (StockQuantity <= 0) return "#F44336";
                if (IsLowStock) return "#FF9800";
                return "#4CAF50";
            }
        }

        /// <summary>
        /// Gets the amount needed to reach minimum stock
        /// </summary>
        public int StockNeeded
        {
            get
            {
                if (!IsLowStock) return 0;
                return MinimumStock - StockQuantity;
            }
        }

        /// <summary>
        /// Gets the stock level as a percentage
        /// </summary>
        public double StockPercentage
        {
            get
            {
                if (MinimumStock <= 0) return 100;
                return Math.Min(100, (double)StockQuantity / MinimumStock * 100);
            }
        }

        /// <summary>
        /// Gets the stock level percentage formatted
        /// </summary>
        public string StockPercentageFormatted => $"{StockPercentage:F0}%";

        /// <summary>
        /// Gets the formatted last updated date
        /// </summary>
        public string LastUpdatedFormatted => LastUpdated.ToString("dd MMM yyyy HH:mm");

        /// <summary>
        /// Gets the product name (null-safe)
        /// </summary>
        public string ProductName => Product?.ProductName ?? "Unknown";

        /// <summary>
        /// Gets the product price (null-safe)
        /// </summary>
        public decimal ProductPrice => Product?.Price ?? 0;

        /// <summary>
        /// Gets the product price formatted
        /// </summary>
        public string ProductPriceFormatted => $"{ProductPrice:N2} RON";

        /// <summary>
        /// Gets the stock value (quantity * price)
        /// </summary>
        public decimal StockValue => StockQuantity * ProductPrice;

        /// <summary>
        /// Gets the stock value formatted
        /// </summary>
        public string StockValueFormatted => $"{StockValue:N2} RON";

        /// <summary>
        /// Gets the category name (null-safe)
        /// </summary>
        public string CategoryName => Product?.Category?.CategoryName ?? "Uncategorized";
    }
}
