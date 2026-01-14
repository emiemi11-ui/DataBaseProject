using System;
using System.Linq;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Partial class for Order - Business logic extensions
    /// Extends the auto-generated Order class from EDMX
    /// </summary>
    public partial class Order
    {
        /// <summary>
        /// Gets the formatted total amount with currency
        /// </summary>
        public string TotalAmountFormatted => $"{TotalAmount:N2} RON";

        /// <summary>
        /// Gets the formatted order date
        /// </summary>
        public string OrderDateFormatted => OrderDate.ToString("dd MMM yyyy HH:mm");

        /// <summary>
        /// Gets the short formatted order date
        /// </summary>
        public string OrderDateShort => OrderDate.ToString("dd/MM/yyyy");

        /// <summary>
        /// Gets the order status color
        /// </summary>
        public string StatusColor
        {
            get
            {
                switch (OrderStatus)
                {
                    case "Pending": return "#FF9800";
                    case "Processing": return "#2196F3";
                    case "Shipped": return "#9C27B0";
                    case "Delivered": return "#4CAF50";
                    case "Cancelled": return "#F44336";
                    default: return "#9E9E9E";
                }
            }
        }

        /// <summary>
        /// Gets the order status icon
        /// </summary>
        public string StatusIcon
        {
            get
            {
                switch (OrderStatus)
                {
                    case "Pending": return "M12 2C6.5 2 2 6.5 2 12S6.5 22 12 22 22 17.5 22 12 17.5 2 12 2M12 20C7.59 20 4 16.41 4 12S7.59 4 12 4 20 7.59 20 12 16.41 20 12 20M12.5 7V12.25L17 14.92L16.25 16.15L11 13V7H12.5Z"; // Clock
                    case "Processing": return "M12 4V1L8 5l4 4V6c3.31 0 6 2.69 6 6 0 1.01-.25 1.97-.7 2.8l1.46 1.46C19.54 15.03 20 13.57 20 12c0-4.42-3.58-8-8-8zm0 14c-3.31 0-6-2.69-6-6 0-1.01.25-1.97.7-2.8L5.24 7.74C4.46 8.97 4 10.43 4 12c0 4.42 3.58 8 8 8v3l4-4-4-4v3z"; // Sync
                    case "Shipped": return "M20 8h-3V4H3c-1.1 0-2 .9-2 2v11h2c0 1.66 1.34 3 3 3s3-1.34 3-3h6c0 1.66 1.34 3 3 3s3-1.34 3-3h2v-5l-3-4zM6 18.5c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5zm13.5-9l1.96 2.5H17V9.5h2.5zm-1.5 9c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5z"; // Truck
                    case "Delivered": return "M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z"; // Check
                    case "Cancelled": return "M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"; // Close
                    default: return "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2z";
                }
            }
        }

        /// <summary>
        /// Gets the number of items in the order
        /// </summary>
        public int ItemCount => OrderDetails?.Count ?? 0;

        /// <summary>
        /// Gets the total quantity of items
        /// </summary>
        public int TotalQuantity => OrderDetails?.Sum(od => od.Quantity) ?? 0;

        /// <summary>
        /// Gets the customer name (null-safe)
        /// </summary>
        public string CustomerName => Customer?.FullName ?? "Unknown";

        /// <summary>
        /// Gets the customer email (null-safe)
        /// </summary>
        public string CustomerEmail => Customer?.Email ?? "";

        /// <summary>
        /// Checks if order can be cancelled
        /// </summary>
        public bool CanBeCancelled => OrderStatus == "Pending" || OrderStatus == "Processing";

        /// <summary>
        /// Checks if order can be updated to next status
        /// </summary>
        public bool CanAdvanceStatus => OrderStatus != "Delivered" && OrderStatus != "Cancelled";

        /// <summary>
        /// Gets the next status in workflow
        /// </summary>
        public string NextStatus
        {
            get
            {
                switch (OrderStatus)
                {
                    case "Pending": return "Processing";
                    case "Processing": return "Shipped";
                    case "Shipped": return "Delivered";
                    default: return null;
                }
            }
        }

        /// <summary>
        /// Gets a display-friendly order reference
        /// </summary>
        public string OrderReference => $"ORD-{OrderID:D6}";

        /// <summary>
        /// Gets the payment method display name
        /// </summary>
        public string PaymentMethodDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(PaymentMethod)) return "Not specified";
                return PaymentMethod;
            }
        }
    }

    /// <summary>
    /// Partial class for OrderDetail - Business logic extensions
    /// </summary>
    public partial class OrderDetail
    {
        /// <summary>
        /// Gets the formatted unit price
        /// </summary>
        public string UnitPriceFormatted => $"{UnitPrice:N2} RON";

        /// <summary>
        /// Gets the formatted subtotal
        /// </summary>
        public string SubtotalFormatted => $"{(Subtotal ?? (Quantity * UnitPrice)):N2} RON";

        /// <summary>
        /// Gets the product name (null-safe)
        /// </summary>
        public string ProductName => Product?.ProductName ?? "Unknown Product";

        /// <summary>
        /// Calculates the line total if Subtotal is null
        /// </summary>
        public decimal LineTotal => Subtotal ?? (Quantity * UnitPrice);
    }
}
