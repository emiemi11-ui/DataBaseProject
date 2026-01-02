using System.Collections.Generic;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Represents a product category in the E-Commerce system.
    /// </summary>
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }

        // Navigation properties
        public virtual ICollection<Product> Products { get; set; }
    }
}
