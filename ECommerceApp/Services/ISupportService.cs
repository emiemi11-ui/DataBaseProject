using ECommerceApp.Models;
using System.Collections.Generic;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Interface for support ticket operations.
    /// </summary>
    public interface ISupportService
    {
        /// <summary>
        /// Gets all support tickets.
        /// </summary>
        List<SupportTicket> GetAllTickets();

        /// <summary>
        /// Gets a ticket by its ID.
        /// </summary>
        SupportTicket GetTicketById(int ticketId);

        /// <summary>
        /// Gets tickets by customer.
        /// </summary>
        List<SupportTicket> GetTicketsByCustomer(int customerId);

        /// <summary>
        /// Gets tickets by status.
        /// </summary>
        List<SupportTicket> GetTicketsByStatus(string status);

        /// <summary>
        /// Gets tickets assigned to a specific support representative.
        /// </summary>
        List<SupportTicket> GetTicketsByAssignee(int assigneeId);

        /// <summary>
        /// Gets unassigned tickets.
        /// </summary>
        List<SupportTicket> GetUnassignedTickets();

        /// <summary>
        /// Creates a new support ticket.
        /// </summary>
        bool CreateTicket(SupportTicket ticket);

        /// <summary>
        /// Assigns a ticket to a support representative.
        /// </summary>
        bool AssignTicket(int ticketId, int assigneeId);

        /// <summary>
        /// Updates the status of a ticket.
        /// </summary>
        bool UpdateTicketStatus(int ticketId, string status);

        /// <summary>
        /// Resolves a ticket.
        /// </summary>
        bool ResolveTicket(int ticketId);

        /// <summary>
        /// Closes a ticket.
        /// </summary>
        bool CloseTicket(int ticketId);
    }
}
