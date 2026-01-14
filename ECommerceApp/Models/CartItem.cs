using System;

namespace ECommerceApp.Models
{
    /// <summary>
    /// CartItem - In-memory shopping cart item
    /// Not mapped to database, used for session-based cart
    /// </summary>
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ImageURL { get; set; }
        public string CategoryName { get; set; }

        // Calculated properties
        public decimal Subtotal => UnitPrice * Quantity;
        public string SubtotalFormatted => $"{Subtotal:N2} RON";
        public string UnitPriceFormatted => $"{UnitPrice:N2} RON";

        /// <summary>
        /// Creates a CartItem from a Product entity
        /// </summary>
        public static CartItem FromProduct(Product product, int quantity = 1)
        {
            return new CartItem
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                ProductDescription = product.Description,
                UnitPrice = product.Price,
                Quantity = quantity,
                ImageURL = product.ImageURL,
                CategoryName = product.Category?.CategoryName ?? "Uncategorized"
            };
        }

        /// <summary>
        /// Checks if there's enough stock for the requested quantity
        /// </summary>
        public bool HasSufficientStock(Product product)
        {
            if (product?.Inventory == null) return false;
            return product.Inventory.StockQuantity >= Quantity;
        }
    }
}
