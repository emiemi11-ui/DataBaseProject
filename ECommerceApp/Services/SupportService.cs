using ECommerceApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Ticket status constants
    /// </summary>
    public static class TicketStatuses
    {
        public const string Open = "Open";
        public const string InProgress = "InProgress";
        public const string Resolved = "Resolved";
        public const string Closed = "Closed";

        public static readonly string[] AllStatuses = { Open, InProgress, Resolved, Closed };
    }

    /// <summary>
    /// Ticket priority constants
    /// </summary>
    public static class TicketPriorities
    {
        public const string Low = "Low";
        public const string Medium = "Medium";
        public const string High = "High";

        public static readonly string[] AllPriorities = { Low, Medium, High };
    }

    /// <summary>
    /// Service for support ticket operations.
    /// Uses ECommerceEntities (DB First - EDMX generated context)
    /// </summary>
    public class SupportService : ISupportService, IDisposable
    {
        private readonly ECommerceEntities _context;
        private bool _disposed;

        public SupportService()
        {
            _context = new ECommerceEntities();
        }

        public SupportService(ECommerceEntities context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all tickets with Eager Loading
        /// </summary>
        public List<SupportTicket> GetAllTickets()
        {
            return _context.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.AssignedTo)
                .Include(t => t.TicketMessages)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        /// <summary>
        /// Gets ticket by ID with explicit loading
        /// </summary>
        public SupportTicket GetTicketById(int ticketId)
        {
            var ticket = _context.SupportTickets.Find(ticketId);

            if (ticket != null)
            {
                // Explicit Loading - Curs 10
                _context.Entry(ticket).Reference(t => t.Customer).Load();
                _context.Entry(ticket).Reference(t => t.AssignedTo).Load();
                _context.Entry(ticket).Collection(t => t.TicketMessages).Load();

                // Load user for each message
                foreach (var message in ticket.TicketMessages)
                {
                    _context.Entry(message).Reference(m => m.User).Load();
                }
            }

            return ticket;
        }

        /// <summary>
        /// Gets tickets by customer
        /// </summary>
        public List<SupportTicket> GetTicketsByCustomer(int customerId)
        {
            return _context.SupportTickets
                .Include(t => t.AssignedTo)
                .Include(t => t.TicketMessages)
                .Where(t => t.CustomerID == customerId)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        /// <summary>
        /// Gets tickets by status
        /// </summary>
        public List<SupportTicket> GetTicketsByStatus(string status)
        {
            return _context.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.AssignedTo)
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        /// <summary>
        /// Gets tickets by assignee
        /// </summary>
        public List<SupportTicket> GetTicketsByAssignee(int assigneeId)
        {
            return _context.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.TicketMessages)
                .Where(t => t.AssignedToID == assigneeId)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        /// <summary>
        /// Gets unassigned tickets
        /// </summary>
        public List<SupportTicket> GetUnassignedTickets()
        {
            return _context.SupportTickets
                .Include(t => t.Customer)
                .Where(t => t.AssignedToID == null && t.Status != TicketStatuses.Closed)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        /// <summary>
        /// Gets open tickets count
        /// </summary>
        public int GetOpenTicketsCount()
        {
            return _context.SupportTickets
                .Count(t => t.Status == TicketStatuses.Open || t.Status == TicketStatuses.InProgress);
        }

        /// <summary>
        /// Creates a new ticket
        /// </summary>
        public bool CreateTicket(SupportTicket ticket)
        {
            try
            {
                ticket.CreatedDate = DateTime.Now;
                ticket.Status = TicketStatuses.Open;

                _context.SupportTickets.Add(ticket);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Assigns ticket to support agent
        /// </summary>
        public bool AssignTicket(int ticketId, int assigneeId)
        {
            try
            {
                var ticket = _context.SupportTickets.Find(ticketId);
                if (ticket == null)
                {
                    return false;
                }

                ticket.AssignedToID = assigneeId;
                if (ticket.Status == TicketStatuses.Open)
                {
                    ticket.Status = TicketStatuses.InProgress;
                }

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Updates ticket status
        /// </summary>
        public bool UpdateTicketStatus(int ticketId, string status)
        {
            try
            {
                var ticket = _context.SupportTickets.Find(ticketId);
                if (ticket == null)
                {
                    return false;
                }

                ticket.Status = status;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Updates ticket priority
        /// </summary>
        public bool UpdateTicketPriority(int ticketId, string priority)
        {
            try
            {
                var ticket = _context.SupportTickets.Find(ticketId);
                if (ticket == null)
                {
                    return false;
                }

                ticket.Priority = priority;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Resolves a ticket
        /// </summary>
        public bool ResolveTicket(int ticketId)
        {
            try
            {
                var ticket = _context.SupportTickets.Find(ticketId);
                if (ticket == null)
                {
                    return false;
                }

                ticket.Status = TicketStatuses.Resolved;
                ticket.ResolvedDate = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Closes a ticket
        /// </summary>
        public bool CloseTicket(int ticketId)
        {
            try
            {
                var ticket = _context.SupportTickets.Find(ticketId);
                if (ticket == null)
                {
                    return false;
                }

                ticket.Status = TicketStatuses.Closed;
                if (ticket.ResolvedDate == null)
                {
                    ticket.ResolvedDate = DateTime.Now;
                }
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Adds a message to a ticket
        /// </summary>
        public bool AddTicketMessage(int ticketId, int userId, string messageText, bool isFromCustomer)
        {
            try
            {
                var message = new TicketMessage
                {
                    TicketID = ticketId,
                    UserID = userId,
                    MessageText = messageText,
                    MessageDate = DateTime.Now,
                    IsFromCustomer = isFromCustomer
                };

                _context.TicketMessages.Add(message);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets messages for a ticket
        /// </summary>
        public List<TicketMessage> GetTicketMessages(int ticketId)
        {
            return _context.TicketMessages
                .Include(m => m.User)
                .Where(m => m.TicketID == ticketId)
                .OrderBy(m => m.MessageDate)
                .ToList();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
