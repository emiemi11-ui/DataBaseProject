using ECommerceApp.Data;
using ECommerceApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Service for support ticket operations.
    /// </summary>
    public class SupportService : ISupportService, IDisposable
    {
        private readonly ECommerceDbContext _context;
        private bool _disposed;

        public SupportService()
        {
            _context = new ECommerceDbContext();
        }

        public SupportService(ECommerceDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public List<SupportTicket> GetAllTickets()
        {
            return _context.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.AssignedTo)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        /// <inheritdoc/>
        public SupportTicket GetTicketById(int ticketId)
        {
            return _context.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.AssignedTo)
                .FirstOrDefault(t => t.TicketID == ticketId);
        }

        /// <inheritdoc/>
        public List<SupportTicket> GetTicketsByCustomer(int customerId)
        {
            return _context.SupportTickets
                .Include(t => t.AssignedTo)
                .Where(t => t.CustomerID == customerId)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        /// <inheritdoc/>
        public List<SupportTicket> GetTicketsByStatus(string status)
        {
            return _context.SupportTickets
                .Include(t => t.Customer)
                .Include(t => t.AssignedTo)
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        /// <inheritdoc/>
        public List<SupportTicket> GetTicketsByAssignee(int assigneeId)
        {
            return _context.SupportTickets
                .Include(t => t.Customer)
                .Where(t => t.AssignedToID == assigneeId)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        /// <inheritdoc/>
        public List<SupportTicket> GetUnassignedTickets()
        {
            return _context.SupportTickets
                .Include(t => t.Customer)
                .Where(t => t.AssignedToID == null && t.Status != TicketStatuses.Closed)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
