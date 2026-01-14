using ECommerceApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Service for inventory-related operations.
    /// Uses ECommerceEntities (DB First - EDMX generated context)
    /// </summary>
    public class InventoryService : IInventoryService, IDisposable
    {
        private readonly ECommerceEntities _context;
        private bool _disposed;

        public InventoryService()
        {
            _context = new ECommerceEntities();
        }

        public InventoryService(ECommerceEntities context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all inventory with Eager Loading
        /// </summary>
        public List<Inventory> GetAllInventory()
        {
            return _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Product.Category)
                .OrderBy(i => i.Product.ProductName)
                .ToList();
        }

        /// <summary>
        /// Gets inventory by product ID
        /// </summary>
        public Inventory GetInventoryByProductId(int productId)
        {
            return _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Product.Category)
                .FirstOrDefault(i => i.ProductID == productId);
        }

        /// <summary>
        /// Gets low stock items
        /// </summary>
        public List<Inventory> GetLowStockItems()
        {
            return _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Product.Category)
                .Where(i => i.StockQuantity < i.MinimumStock && i.Product.IsActive)
                .OrderBy(i => i.StockQuantity)
                .ToList();
        }

        /// <summary>
        /// Gets low stock count
        /// </summary>
        public int GetLowStockCount()
        {
            return _context.Inventories
                .Count(i => i.StockQuantity < i.MinimumStock && i.Product.IsActive);
        }

        /// <summary>
        /// Gets out of stock items
        /// </summary>
        public List<Inventory> GetOutOfStockItems()
        {
            return _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Product.Category)
                .Where(i => i.StockQuantity <= 0 && i.Product.IsActive)
                .OrderBy(i => i.Product.ProductName)
                .ToList();
        }

        /// <summary>
        /// Updates stock quantity
        /// </summary>
        public bool UpdateStock(int productId, int quantity)
        {
            try
            {
                var inventory = _context.Inventories.FirstOrDefault(i => i.ProductID == productId);
                if (inventory == null)
                {
                    return false;
                }

                if (quantity < 0)
                {
                    return false;
                }

                inventory.StockQuantity = quantity;
                inventory.LastUpdated = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Adds stock to inventory
        /// </summary>
        public bool AddStock(int productId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    return false;
                }

                var inventory = _context.Inventories.FirstOrDefault(i => i.ProductID == productId);
                if (inventory == null)
                {
                    return false;
                }

                inventory.StockQuantity += quantity;
                inventory.LastUpdated = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Removes stock from inventory
        /// </summary>
        public bool RemoveStock(int productId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    return false;
                }

                var inventory = _context.Inventories.FirstOrDefault(i => i.ProductID == productId);
                if (inventory == null)
                {
                    return false;
                }

                if (inventory.StockQuantity < quantity)
                {
                    return false;
                }

                inventory.StockQuantity -= quantity;
                inventory.LastUpdated = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sets minimum stock threshold
        /// </summary>
        public bool SetMinimumStock(int productId, int minimumStock)
        {
            try
            {
                if (minimumStock < 0)
                {
                    return false;
                }

                var inventory = _context.Inventories.FirstOrDefault(i => i.ProductID == productId);
                if (inventory == null)
                {
                    return false;
                }

                inventory.MinimumStock = minimumStock;
                inventory.LastUpdated = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if there's sufficient stock
        /// </summary>
        public bool HasSufficientStock(int productId, int requestedQuantity)
        {
            var inventory = _context.Inventories.FirstOrDefault(i => i.ProductID == productId);
            return inventory != null && inventory.StockQuantity >= requestedQuantity;
        }

        /// <summary>
        /// Gets total inventory value
        /// </summary>
        public decimal GetTotalInventoryValue()
        {
            return _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.Product.IsActive)
                .Sum(i => (decimal?)i.StockQuantity * i.Product.Price) ?? 0;
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
