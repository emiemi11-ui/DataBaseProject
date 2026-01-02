using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Represents a product in the E-Commerce system.
    /// </summary>
    [Table("Products")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [Required]
        public int StoreOwnerID { get; set; }

        [MaxLength(500)]
        public string ImageURL { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        // Navigation properties
        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }

        [ForeignKey("StoreOwnerID")]
        public virtual User StoreOwner { get; set; }

        public virtual Inventory Inventory { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        public Product()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
            OrderDetails = new HashSet<OrderDetail>();
            Reviews = new HashSet<Review>();
        }
    }
}
