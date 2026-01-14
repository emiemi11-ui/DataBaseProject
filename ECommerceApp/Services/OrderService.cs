using ECommerceApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Order status constants
    /// </summary>
    public static class OrderStatuses
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Shipped = "Shipped";
        public const string Delivered = "Delivered";
        public const string Cancelled = "Cancelled";

        public static readonly string[] AllStatuses = { Pending, Processing, Shipped, Delivered, Cancelled };
    }

    /// <summary>
    /// Service for order-related operations.
    /// Uses ECommerceEntities (DB First - EDMX generated context)
    /// </summary>
    public class OrderService : IOrderService, IDisposable
    {
        private readonly ECommerceEntities _context;
        private bool _disposed;

        public OrderService()
        {
            _context = new ECommerceEntities();
        }

        public OrderService(ECommerceEntities context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all orders with Eager Loading
        /// </summary>
        public List<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .Include(o => o.OrderDetails.Select(od => od.Product))
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        /// <summary>
        /// Gets order by ID with related entities
        /// </summary>
        public Order GetOrderById(int orderId)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .Include(o => o.OrderDetails.Select(od => od.Product))
                .FirstOrDefault(o => o.OrderID == orderId);
        }

        /// <summary>
        /// Gets orders by customer
        /// </summary>
        public List<Order> GetOrdersByCustomer(int customerId)
        {
            return _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.OrderDetails.Select(od => od.Product))
                .Where(o => o.CustomerID == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        /// <summary>
        /// Gets orders by status
        /// </summary>
        public List<Order> GetOrdersByStatus(string status)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .Where(o => o.OrderStatus == status)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        /// <summary>
        /// Gets pending orders count
        /// </summary>
        public int GetPendingOrdersCount()
        {
            return _context.Orders.Count(o => o.OrderStatus == OrderStatuses.Pending);
        }

        /// <summary>
        /// Creates a new order with transaction support
        /// </summary>
        public Order CreateOrder(int customerId, string shippingAddress, List<CartItem> cartItems, string paymentMethod = null)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Calculate total amount
                    decimal totalAmount = cartItems.Sum(item => item.Subtotal);

                    // Create the order
                    var order = new Order
                    {
                        CustomerID = customerId,
                        OrderDate = DateTime.Now,
                        TotalAmount = totalAmount,
                        OrderStatus = OrderStatuses.Pending,
                        ShippingAddress = shippingAddress,
                        PaymentMethod = paymentMethod ?? "Credit Card"
                    };

                    _context.Orders.Add(order);
                    _context.SaveChanges();

                    // Create order details and update inventory
                    foreach (var item in cartItems)
                    {
                        // Create order detail
                        var orderDetail = new OrderDetail
                        {
                            OrderID = order.OrderID,
                            ProductID = item.ProductID,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice
                        };
                        _context.OrderDetails.Add(orderDetail);

                        // Update inventory
                        var inventory = _context.Inventories.FirstOrDefault(i => i.ProductID == item.ProductID);
                        if (inventory != null)
                        {
                            if (inventory.StockQuantity < item.Quantity)
                            {
                                throw new InvalidOperationException($"Insufficient stock for product: {item.ProductName}");
                            }
                            inventory.StockQuantity -= item.Quantity;
                            inventory.LastUpdated = DateTime.Now;
                        }
                    }

                    _context.SaveChanges();
                    transaction.Commit();

                    return order;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Updates order status
        /// </summary>
        public bool UpdateOrderStatus(int orderId, string status)
        {
            try
            {
                var order = _context.Orders.Find(orderId);
                if (order == null)
                {
                    return false;
                }

                order.OrderStatus = status;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Cancels an order and restores inventory
        /// </summary>
        public bool CancelOrder(int orderId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var order = _context.Orders
                        .Include(o => o.OrderDetails)
                        .FirstOrDefault(o => o.OrderID == orderId);

                    if (order == null)
                    {
                        return false;
                    }

                    // Can only cancel pending or processing orders
                    if (order.OrderStatus != OrderStatuses.Pending &&
                        order.OrderStatus != OrderStatuses.Processing)
                    {
                        return false;
                    }

                    // Restore inventory
                    foreach (var detail in order.OrderDetails)
                    {
                        var inventory = _context.Inventories.FirstOrDefault(i => i.ProductID == detail.ProductID);
                        if (inventory != null)
                        {
                            inventory.StockQuantity += detail.Quantity;
                            inventory.LastUpdated = DateTime.Now;
                        }
                    }

                    order.OrderStatus = OrderStatuses.Cancelled;
                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets order details for a specific order
        /// </summary>
        public List<OrderDetail> GetOrderDetails(int orderId)
        {
            return _context.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Product.Category)
                .Where(od => od.OrderID == orderId)
                .ToList();
        }

        /// <summary>
        /// Gets monthly revenue
        /// </summary>
        public decimal GetMonthlyRevenue()
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            return _context.Orders
                .Where(o => o.OrderDate >= startOfMonth && o.OrderStatus != OrderStatuses.Cancelled)
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;
        }

        /// <summary>
        /// Gets orders for date range
        /// </summary>
        public List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
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
