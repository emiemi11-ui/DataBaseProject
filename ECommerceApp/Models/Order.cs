using System;
using System.Collections.Generic;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Represents a customer order in the E-Commerce system.
    /// </summary>
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public string ShippingAddress { get; set; }

        // Navigation properties
        public virtual User Customer { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }

    /// <summary>
    /// Order status constants
    /// </summary>
    public static class OrderStatuses
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Shipped = "Shipped";
        public const string Delivered = "Delivered";
        public const string Cancelled = "Cancelled";

        public static string[] All => new[] { Pending, Processing, Shipped, Delivered, Cancelled };
    }
}
