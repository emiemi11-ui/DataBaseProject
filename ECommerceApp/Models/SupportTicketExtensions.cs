using System;
using System.Linq;

namespace ECommerceApp.Models
{
    /// <summary>
    /// Partial class for SupportTicket - Business logic extensions
    /// Extends the auto-generated SupportTicket class from EDMX
    /// </summary>
    public partial class SupportTicket
    {
        /// <summary>
        /// Gets the formatted creation date
        /// </summary>
        public string CreatedDateFormatted => CreatedDate.ToString("dd MMM yyyy HH:mm");

        /// <summary>
        /// Gets the formatted resolved date
        /// </summary>
        public string ResolvedDateFormatted => ResolvedDate?.ToString("dd MMM yyyy HH:mm") ?? "Not resolved";

        /// <summary>
        /// Gets the status color
        /// </summary>
        public string StatusColor
        {
            get
            {
                switch (Status)
                {
                    case "Open": return "#2196F3";
                    case "InProgress": return "#FF9800";
                    case "Resolved": return "#4CAF50";
                    case "Closed": return "#9E9E9E";
                    default: return "#9E9E9E";
                }
            }
        }

        /// <summary>
        /// Gets the priority color
        /// </summary>
        public string PriorityColor
        {
            get
            {
                switch (Priority)
                {
                    case "Low": return "#4CAF50";
                    case "Medium": return "#FF9800";
                    case "High": return "#F44336";
                    default: return "#9E9E9E";
                }
            }
        }

        /// <summary>
        /// Gets the status display text
        /// </summary>
        public string StatusDisplay
        {
            get
            {
                switch (Status)
                {
                    case "InProgress": return "In Progress";
                    default: return Status;
                }
            }
        }

        /// <summary>
        /// Gets the customer name (null-safe)
        /// </summary>
        public string CustomerName => Customer?.FullName ?? "Unknown";

        /// <summary>
        /// Gets the customer email (null-safe)
        /// </summary>
        public string CustomerEmail => Customer?.Email ?? "";

        /// <summary>
        /// Gets the assigned agent name
        /// </summary>
        public string AssignedAgentName => AssignedTo?.FullName ?? "Unassigned";

        /// <summary>
        /// Checks if ticket is assigned
        /// </summary>
        public bool IsAssigned => AssignedToID.HasValue;

        /// <summary>
        /// Checks if ticket can be resolved
        /// </summary>
        public bool CanBeResolved => Status == "Open" || Status == "InProgress";

        /// <summary>
        /// Checks if ticket can be closed
        /// </summary>
        public bool CanBeClosed => Status == "Resolved";

        /// <summary>
        /// Gets the message count
        /// </summary>
        public int MessageCount => TicketMessages?.Count ?? 0;

        /// <summary>
        /// Gets the last message date
        /// </summary>
        public DateTime? LastMessageDate => TicketMessages?.OrderByDescending(m => m.MessageDate).FirstOrDefault()?.MessageDate;

        /// <summary>
        /// Gets the last message preview
        /// </summary>
        public string LastMessagePreview
        {
            get
            {
                var lastMessage = TicketMessages?.OrderByDescending(m => m.MessageDate).FirstOrDefault();
                if (lastMessage == null) return "";

                var text = lastMessage.MessageText;
                if (string.IsNullOrEmpty(text)) return "";

                return text.Length <= 50 ? text : text.Substring(0, 50) + "...";
            }
        }

        /// <summary>
        /// Gets a display-friendly ticket reference
        /// </summary>
        public string TicketReference => $"TKT-{TicketID:D6}";

        /// <summary>
        /// Gets the time since creation
        /// </summary>
        public string TimeSinceCreation
        {
            get
            {
                var timeSpan = DateTime.Now - CreatedDate;
                if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} min ago";
                if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hours ago";
                if (timeSpan.TotalDays < 30) return $"{(int)timeSpan.TotalDays} days ago";
                return CreatedDateFormatted;
            }
        }
    }

    /// <summary>
    /// Partial class for TicketMessage - Business logic extensions
    /// </summary>
    public partial class TicketMessage
    {
        /// <summary>
        /// Gets the formatted message date
        /// </summary>
        public string MessageDateFormatted => MessageDate.ToString("dd MMM yyyy HH:mm");

        /// <summary>
        /// Gets the sender name
        /// </summary>
        public string SenderName => User?.FullName ?? "Unknown";

        /// <summary>
        /// Gets the sender type label
        /// </summary>
        public string SenderLabel => IsFromCustomer ? "Customer" : "Support";

        /// <summary>
        /// Gets the message alignment (for chat display)
        /// </summary>
        public string MessageAlignment => IsFromCustomer ? "Left" : "Right";

        /// <summary>
        /// Gets the message background color
        /// </summary>
        public string MessageBackgroundColor => IsFromCustomer ? "#E3F2FD" : "#F3E5F5";
    }
}
