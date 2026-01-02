using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Represents inventory for a product.
    /// </summary>
    [Table("Inventory")]
    public class Inventory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InventoryID { get; set; }

        [Required]
        [Index(IsUnique = true)]
        public int ProductID { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public int MinimumStock { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }

        // Navigation properties
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }

        /// <summary>
        /// Checks if the stock is below the minimum threshold
        /// </summary>
        [NotMapped]
        public bool IsLowStock => StockQuantity < MinimumStock;

        public Inventory()
        {
            LastUpdated = DateTime.Now;
            MinimumStock = 10;
        }
    }
}
