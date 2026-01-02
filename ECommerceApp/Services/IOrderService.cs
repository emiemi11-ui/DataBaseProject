using ECommerceApp.Models;
using System.Collections.Generic;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Interface for order-related operations.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Gets all orders with their details.
        /// </summary>
        List<Order> GetAllOrders();

        /// <summary>
        /// Gets an order by its ID.
        /// </summary>
        Order GetOrderById(int orderId);

        /// <summary>
        /// Gets all orders for a specific customer.
        /// </summary>
        List<Order> GetOrdersByCustomer(int customerId);

        /// <summary>
        /// Gets orders by status.
        /// </summary>
        List<Order> GetOrdersByStatus(string status);

        /// <summary>
        /// Creates a new order from cart items.
        /// </summary>
        Order CreateOrder(int customerId, string shippingAddress, List<CartItem> cartItems);

        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        bool UpdateOrderStatus(int orderId, string status);

        /// <summary>
        /// Cancels an order (only if still pending).
        /// </summary>
        bool CancelOrder(int orderId);

        /// <summary>
        /// Gets order details for a specific order.
        /// </summary>
        List<OrderDetail> GetOrderDetails(int orderId);
    }
}
