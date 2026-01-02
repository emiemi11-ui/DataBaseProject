using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Represents a product review.
    /// </summary>
    [Table("Reviews")]
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; }

        // Navigation properties
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; }

        [ForeignKey("CustomerID")]
        public virtual User Customer { get; set; }

        public Review()
        {
            ReviewDate = DateTime.Now;
        }
    }
}
