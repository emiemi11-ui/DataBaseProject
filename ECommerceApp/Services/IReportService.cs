using ECommerceApp.Models;
using System;
using System.Collections.Generic;

namespace ECommerceApp.Services
{
    /// <summary>
    /// Interface for report-related operations using Raw SQL and Stored Procedures
    /// Conform Curs 10 - ADO.NET Entity Framework (pag. 13-14)
    /// </summary>
    public interface IReportService
    {
        // =====================================================
        // Stored Procedure Calls (Function Imports)
        // =====================================================

        /// <summary>
        /// Gets low stock products using stored procedure GetLowStockProducts
        /// </summary>
        List<LowStockProduct> GetLowStockProducts();

        /// <summary>
        /// Gets sales statistics using stored procedure GetSalesStatistics
        /// </summary>
        SalesStatistics GetSalesStatistics(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Gets popular products using stored procedure GetPopularProducts
        /// </summary>
        List<PopularProduct> GetPopularProducts(int topCount = 10);

        /// <summary>
        /// Gets customer order history using stored procedure GetCustomerOrderHistory
        /// </summary>
        List<CustomerOrderHistory> GetCustomerOrderHistory(int customerId);

        /// <summary>
        /// Gets dashboard statistics using stored procedure GetDashboardStatistics
        /// </summary>
        DashboardStatistics GetDashboardStatistics();

        // =====================================================
        // Raw SQL Queries using Database.SqlQuery<T>()
        // =====================================================

        /// <summary>
        /// Gets category sales summary using raw SQL
        /// </summary>
        List<CategorySalesDTO> GetCategorySales();

        /// <summary>
        /// Gets monthly sales trend using raw SQL
        /// </summary>
        List<MonthlySalesDTO> GetMonthlySalesTrend(int monthsBack = 6);

        /// <summary>
        /// Gets customer purchase summary using raw SQL
        /// </summary>
        List<CustomerPurchaseDTO> GetTopCustomers(int topCount = 10);

        // =====================================================
        // Raw SQL Commands using Database.ExecuteSqlCommand()
        // =====================================================

        /// <summary>
        /// Updates product prices by category using ExecuteSqlCommand
        /// </summary>
        int UpdateProductPricesByCategory(int categoryId, decimal percentageChange);

        /// <summary>
        /// Cleans up old closed tickets using ExecuteSqlCommand
        /// </summary>
        int CleanupOldTickets(int daysOld = 90);
    }

    // =====================================================
    // DTOs for Raw SQL Query Results
    // =====================================================

    public class CategorySalesDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public int TotalOrders { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class MonthlySalesDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class CustomerPurchaseDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastOrderDate { get; set; }
    }
}
