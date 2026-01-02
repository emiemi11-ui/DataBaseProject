using ECommerceApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Service for inventory-related operations.
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

        /// <inheritdoc/>
        public List<Inventory> GetAllInventory()
        {
            return _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Product.Category)
                .OrderBy(i => i.Product.ProductName)
                .ToList();
        }

        /// <inheritdoc/>
        public Inventory GetInventoryByProductId(int productId)
        {
            return _context.Inventories
                .Include(i => i.Product)
                .FirstOrDefault(i => i.ProductID == productId);
        }

        /// <inheritdoc/>
        public List<Inventory> GetLowStockItems()
        {
            return _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Product.Category)
                .Where(i => i.StockQuantity < i.MinimumStock)
                .OrderBy(i => i.StockQuantity)
                .ToList();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool HasSufficientStock(int productId, int requestedQuantity)
        {
            var inventory = _context.Inventories.FirstOrDefault(i => i.ProductID == productId);
            return inventory != null && inventory.StockQuantity >= requestedQuantity;
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
