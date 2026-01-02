using System;
using System.Collections.Generic;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Represents a product in the E-Commerce system.
    /// </summary>
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Reviews = new HashSet<Review>();
        }

        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryID { get; set; }
        public int StoreOwnerID { get; set; }
        public string ImageURL { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual Category Category { get; set; }
        public virtual User StoreOwner { get; set; }
        public virtual Inventory Inventory { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
