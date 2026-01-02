using System;
using System.Collections.Generic;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Represents a user in the E-Commerce system.
    /// Can be a Store Owner, Customer, or Customer Service Representative.
    /// </summary>
    public partial class User
    {
        public User()
        {
            Products = new HashSet<Product>();
            Orders = new HashSet<Order>();
            Reviews = new HashSet<Review>();
            CustomerTickets = new HashSet<SupportTicket>();
            AssignedTickets = new HashSet<SupportTicket>();
        }

        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string UserRole { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<SupportTicket> CustomerTickets { get; set; }
        public virtual ICollection<SupportTicket> AssignedTickets { get; set; }
    }
}
