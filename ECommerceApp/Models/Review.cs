using System;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Represents a product review from a customer.
    /// </summary>
    public partial class Review
    {
        public int ReviewID { get; set; }
        public int ProductID { get; set; }
        public int CustomerID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }

        // Navigation properties
        public virtual Product Product { get; set; }
        public virtual User Customer { get; set; }
    }
}
