using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Represents a customer order.
    /// </summary>
    [Table("Orders")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string OrderStatus { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(500)]
        public string ShippingAddress { get; set; }

        public int? ProcessedBy { get; set; }

        // Navigation properties
        [ForeignKey("CustomerID")]
        public virtual User Customer { get; set; }

        [ForeignKey("ProcessedBy")]
        public virtual User Processor { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public Order()
        {
            OrderDate = DateTime.Now;
            OrderStatus = "Pending";
            OrderDetails = new HashSet<OrderDetail>();
        }
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
