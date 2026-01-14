using ECommerceApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Service for reports using Raw SQL and Stored Procedures
    /// Conform Curs 10 - ADO.NET Entity Framework (pag. 13-14)
    ///
    /// Demonstrates:
    /// - Database.SqlQuery<T>() for raw SQL queries
    /// - ExecuteSqlCommand() for update/delete operations
    /// - Function Imports for stored procedure calls
    /// </summary>
    public class ReportService : IReportService, IDisposable
    {
        private readonly ECommerceEntities _context;
        private bool _disposed;

        public ReportService()
        {
            _context = new ECommerceEntities();
        }

        public ReportService(ECommerceEntities context)
        {
            _context = context;
        }

        // =====================================================
        // STORED PROCEDURE CALLS (Function Imports)
        // Conform Curs 10, pag. 13-14
        // =====================================================

        /// <summary>
        /// GetLowStockProducts - Calls stored procedure via Function Import
        /// Defined in EDMX as Function Import
        /// </summary>
        public List<LowStockProduct> GetLowStockProducts()
        {
            // Using Function Import from EDMX
            return _context.GetLowStockProducts().ToList();
        }

        /// <summary>
        /// GetSalesStatistics - Calls stored procedure with parameters
        /// </summary>
        public SalesStatistics GetSalesStatistics(DateTime? startDate, DateTime? endDate)
        {
            // Using Function Import with parameters
            return _context.GetSalesStatistics(startDate, endDate).FirstOrDefault()
                ?? new SalesStatistics();
        }

        /// <summary>
        /// GetPopularProducts - Calls stored procedure with parameter
        /// </summary>
        public List<PopularProduct> GetPopularProducts(int topCount = 10)
        {
            // Using Function Import with parameter
            return _context.GetPopularProducts(topCount).ToList();
        }

        /// <summary>
        /// GetCustomerOrderHistory - Calls stored procedure for specific customer
        /// </summary>
        public List<CustomerOrderHistory> GetCustomerOrderHistory(int customerId)
        {
            // Using Function Import with customer ID parameter
            return _context.GetCustomerOrderHistory(customerId).ToList();
        }

        /// <summary>
        /// GetDashboardStatistics - Calls stored procedure for dashboard data
        /// </summary>
        public DashboardStatistics GetDashboardStatistics()
        {
            // Using Function Import
            return _context.GetDashboardStatistics().FirstOrDefault()
                ?? new DashboardStatistics();
        }

        // =====================================================
        // RAW SQL QUERIES - Database.SqlQuery<T>()
        // Conform Curs 10, pag. 14
        // =====================================================

        /// <summary>
        /// GetCategorySales - Complex raw SQL query for category analytics
        /// Uses Database.SqlQuery<T>() to execute raw SQL
        /// </summary>
        public List<CategorySalesDTO> GetCategorySales()
        {
            // Raw SQL query - Curs 10, pag. 14
            string sql = @"
                SELECT
                    c.CategoryID,
                    c.CategoryName,
                    COUNT(DISTINCT p.ProductID) AS ProductCount,
                    COUNT(DISTINCT od.OrderID) AS TotalOrders,
                    ISNULL(SUM(od.Quantity), 0) AS TotalQuantitySold,
                    ISNULL(SUM(od.Subtotal), 0) AS TotalRevenue
                FROM Categories c
                LEFT JOIN Products p ON c.CategoryID = p.CategoryID AND p.IsActive = 1
                LEFT JOIN OrderDetails od ON p.ProductID = od.ProductID
                LEFT JOIN Orders o ON od.OrderID = o.OrderID AND o.OrderStatus != 'Cancelled'
                GROUP BY c.CategoryID, c.CategoryName
                ORDER BY TotalRevenue DESC";

            return _context.Database.SqlQuery<CategorySalesDTO>(sql).ToList();
        }

        /// <summary>
        /// GetMonthlySalesTrend - Raw SQL query for monthly sales analysis
        /// </summary>
        public List<MonthlySalesDTO> GetMonthlySalesTrend(int monthsBack = 6)
        {
            // Raw SQL with parameter - Curs 10, pag. 14
            string sql = @"
                SELECT
                    YEAR(o.OrderDate) AS [Year],
                    MONTH(o.OrderDate) AS [Month],
                    DATENAME(MONTH, o.OrderDate) AS MonthName,
                    COUNT(o.OrderID) AS OrderCount,
                    ISNULL(SUM(o.TotalAmount), 0) AS TotalRevenue
                FROM Orders o
                WHERE o.OrderDate >= DATEADD(MONTH, -@MonthsBack, GETDATE())
                    AND o.OrderStatus != 'Cancelled'
                GROUP BY YEAR(o.OrderDate), MONTH(o.OrderDate), DATENAME(MONTH, o.OrderDate)
                ORDER BY [Year] DESC, [Month] DESC";

            var monthsParam = new SqlParameter("@MonthsBack", monthsBack);
            return _context.Database.SqlQuery<MonthlySalesDTO>(sql, monthsParam).ToList();
        }

        /// <summary>
        /// GetTopCustomers - Raw SQL query for top customers by purchases
        /// </summary>
        public List<CustomerPurchaseDTO> GetTopCustomers(int topCount = 10)
        {
            // Raw SQL query with TOP and parameter
            string sql = @"
                SELECT TOP (@TopCount)
                    u.UserID,
                    u.Username,
                    u.Email,
                    ISNULL(u.FirstName + ' ' + u.LastName, u.Username) AS FullName,
                    COUNT(o.OrderID) AS OrderCount,
                    ISNULL(SUM(o.TotalAmount), 0) AS TotalSpent,
                    MAX(o.OrderDate) AS LastOrderDate
                FROM Users u
                LEFT JOIN Orders o ON u.UserID = o.CustomerID AND o.OrderStatus != 'Cancelled'
                WHERE u.UserRole = 'Customer' AND u.IsActive = 1
                GROUP BY u.UserID, u.Username, u.Email, u.FirstName, u.LastName
                ORDER BY TotalSpent DESC";

            var topParam = new SqlParameter("@TopCount", topCount);
            return _context.Database.SqlQuery<CustomerPurchaseDTO>(sql, topParam).ToList();
        }

        // =====================================================
        // RAW SQL COMMANDS - Database.ExecuteSqlCommand()
        // Conform Curs 10, pag. 14
        // =====================================================

        /// <summary>
        /// UpdateProductPricesByCategory - Uses ExecuteSqlCommand for bulk update
        /// Returns number of affected rows
        /// </summary>
        public int UpdateProductPricesByCategory(int categoryId, decimal percentageChange)
        {
            // ExecuteSqlCommand - Curs 10, pag. 14
            // Note: percentageChange is positive for increase, negative for decrease
            string sql = @"
                UPDATE Products
                SET Price = Price * (1 + @Percentage / 100.0)
                WHERE CategoryID = @CategoryID
                    AND IsActive = 1";

            var categoryParam = new SqlParameter("@CategoryID", categoryId);
            var percentageParam = new SqlParameter("@Percentage", percentageChange);

            return _context.Database.ExecuteSqlCommand(sql, categoryParam, percentageParam);
        }

        /// <summary>
        /// CleanupOldTickets - Uses ExecuteSqlCommand to delete old closed tickets
        /// Returns number of affected rows
        /// </summary>
        public int CleanupOldTickets(int daysOld = 90)
        {
            // First delete messages from old closed tickets
            string deleteMessagesSql = @"
                DELETE FROM TicketMessages
                WHERE TicketID IN (
                    SELECT TicketID
                    FROM SupportTickets
                    WHERE Status = 'Closed'
                        AND ResolvedDate < DATEADD(DAY, -@DaysOld, GETDATE())
                )";

            var daysParam1 = new SqlParameter("@DaysOld", daysOld);
            _context.Database.ExecuteSqlCommand(deleteMessagesSql, daysParam1);

            // Then delete the tickets
            string deleteTicketsSql = @"
                DELETE FROM SupportTickets
                WHERE Status = 'Closed'
                    AND ResolvedDate < DATEADD(DAY, -@DaysOld, GETDATE())";

            var daysParam2 = new SqlParameter("@DaysOld", daysOld);
            return _context.Database.ExecuteSqlCommand(deleteTicketsSql, daysParam2);
        }

        // =====================================================
        // ADDITIONAL RAW SQL QUERIES FOR DASHBOARD
        // =====================================================

        /// <summary>
        /// Gets revenue for current month using raw SQL
        /// </summary>
        public decimal GetCurrentMonthRevenue()
        {
            string sql = @"
                SELECT ISNULL(SUM(TotalAmount), 0) AS Revenue
                FROM Orders
                WHERE MONTH(OrderDate) = MONTH(GETDATE())
                    AND YEAR(OrderDate) = YEAR(GETDATE())
                    AND OrderStatus != 'Cancelled'";

            return _context.Database.SqlQuery<decimal>(sql).FirstOrDefault();
        }

        /// <summary>
        /// Gets order count for today using raw SQL
        /// </summary>
        public int GetTodayOrderCount()
        {
            string sql = @"
                SELECT COUNT(*) AS OrderCount
                FROM Orders
                WHERE CAST(OrderDate AS DATE) = CAST(GETDATE() AS DATE)";

            return _context.Database.SqlQuery<int>(sql).FirstOrDefault();
        }

        /// <summary>
        /// Gets product performance summary using raw SQL
        /// </summary>
        public List<ProductPerformanceDTO> GetProductPerformance(int topCount = 20)
        {
            string sql = @"
                SELECT TOP (@TopCount)
                    p.ProductID,
                    p.ProductName,
                    c.CategoryName,
                    p.Price,
                    i.StockQuantity,
                    ISNULL(SUM(od.Quantity), 0) AS TotalSold,
                    ISNULL(SUM(od.Subtotal), 0) AS TotalRevenue,
                    COUNT(DISTINCT r.ReviewID) AS ReviewCount,
                    AVG(CAST(r.Rating AS FLOAT)) AS AverageRating
                FROM Products p
                INNER JOIN Categories c ON p.CategoryID = c.CategoryID
                LEFT JOIN Inventory i ON p.ProductID = i.ProductID
                LEFT JOIN OrderDetails od ON p.ProductID = od.ProductID
                LEFT JOIN Reviews r ON p.ProductID = r.ProductID
                WHERE p.IsActive = 1
                GROUP BY p.ProductID, p.ProductName, c.CategoryName, p.Price, i.StockQuantity
                ORDER BY TotalRevenue DESC";

            var topParam = new SqlParameter("@TopCount", topCount);
            return _context.Database.SqlQuery<ProductPerformanceDTO>(sql, topParam).ToList();
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

    /// <summary>
    /// DTO for product performance report
    /// </summary>
    public class ProductPerformanceDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public int? StockQuantity { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ReviewCount { get; set; }
        public double? AverageRating { get; set; }
    }
}
