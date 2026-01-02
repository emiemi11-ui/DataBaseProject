namespace ECommerceApp.Models
{
    /// <summary>
    /// Represents a line item in an order.
    /// </summary>
    public partial class OrderDetail
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Computed subtotal (Quantity * UnitPrice)
        /// </summary>
        public decimal Subtotal => Quantity * UnitPrice;

        // Navigation properties
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
