using ECommerceApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Service for order-related operations.
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

        /// <inheritdoc/>
        public List<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .Include(o => o.OrderDetails.Select(od => od.Product))
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        /// <inheritdoc/>
        public Order GetOrderById(int orderId)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .Include(o => o.OrderDetails.Select(od => od.Product))
                .FirstOrDefault(o => o.OrderID == orderId);
        }

        /// <inheritdoc/>
        public List<Order> GetOrdersByCustomer(int customerId)
        {
            return _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.OrderDetails.Select(od => od.Product))
                .Where(o => o.CustomerID == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        /// <inheritdoc/>
        public List<Order> GetOrdersByStatus(string status)
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .Where(o => o.OrderStatus == status)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        /// <inheritdoc/>
        public Order CreateOrder(int customerId, string shippingAddress, List<CartItem> cartItems)
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
                        ShippingAddress = shippingAddress
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public List<OrderDetail> GetOrderDetails(int orderId)
        {
            return _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderID == orderId)
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
