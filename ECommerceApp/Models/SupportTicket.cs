using System;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Represents a customer support ticket.
    /// </summary>
    public partial class SupportTicket
    {
        public int TicketID { get; set; }
        public int CustomerID { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? AssignedToID { get; set; }
        public DateTime? ResolvedDate { get; set; }

        // Navigation properties
        public virtual User Customer { get; set; }
        public virtual User AssignedTo { get; set; }
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
