using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Represents a user in the E-Commerce system.
    /// Can be a Store Owner, Customer, or Customer Service Representative.
    /// </summary>
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [Required]
        [MaxLength(50)]
        [Index(IsUnique = true)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(256)]
        public string HashedPassword { get; set; }

        [Required]
        [MaxLength(20)]
        public string UserRole { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<SupportTicket> CustomerTickets { get; set; }
        public virtual ICollection<SupportTicket> AssignedTickets { get; set; }

        public User()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
            Products = new HashSet<Product>();
            Orders = new HashSet<Order>();
            Reviews = new HashSet<Review>();
            CustomerTickets = new HashSet<SupportTicket>();
            AssignedTickets = new HashSet<SupportTicket>();
        }
    }
}
