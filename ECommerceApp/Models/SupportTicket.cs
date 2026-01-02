using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Represents a customer support ticket.
    /// </summary>
    [Table("SupportTickets")]
    public class SupportTicket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TicketID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        [Required]
        [MaxLength(20)]
        public string Priority { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public int? AssignedToID { get; set; }

        public DateTime? ResolvedDate { get; set; }

        // Navigation properties
        [ForeignKey("CustomerID")]
        public virtual User Customer { get; set; }

        [ForeignKey("AssignedToID")]
        public virtual User AssignedTo { get; set; }

        public SupportTicket()
        {
            CreatedDate = DateTime.Now;
            Status = "Open";
            Priority = "Medium";
        }
    }

    /// <summary>
    /// Ticket status constants
    /// </summary>
    public static class TicketStatuses
    {
        public const string Open = "Open";
        public const string InProgress = "InProgress";
        public const string Resolved = "Resolved";
        public const string Closed = "Closed";

        public static string[] All => new[] { Open, InProgress, Resolved, Closed };
    }

    /// <summary>
    /// Ticket priority constants
    /// </summary>
    public static class TicketPriorities
    {
        public const string Low = "Low";
        public const string Medium = "Medium";
        public const string High = "High";

        public static string[] All => new[] { Low, Medium, High };
    }
}
