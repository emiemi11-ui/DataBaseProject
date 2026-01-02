using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using ECommerceApp.Data;
using ECommerceApp.Helpers;
using ECommerceApp.Models;

namespace ECommerceApp.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ECommerceDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ECommerceDbContext context)
        {
            // Seed Users
            var users = new List<User>
            {
                new User
                {
                    UserID = 1,
                    Username = "admin",
                    Email = "admin@ecommerce.com",
                    HashedPassword = PasswordHelper.ComputeSHA256Hash("password123"),
                    UserRole = "StoreOwner",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new User
                {
                    UserID = 2,
                    Username = "john_doe",
                    Email = "john@example.com",
                    HashedPassword = PasswordHelper.ComputeSHA256Hash("password123"),
                    UserRole = "Customer",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new User
                {
                    UserID = 3,
                    Username = "jane_smith",
                    Email = "jane@example.com",
                    HashedPassword = PasswordHelper.ComputeSHA256Hash("password123"),
                    UserRole = "Customer",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new User
                {
                    UserID = 4,
                    Username = "support1",
                    Email = "support@ecommerce.com",
                    HashedPassword = PasswordHelper.ComputeSHA256Hash("password123"),
                    UserRole = "CustomerService",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                }
            };

            context.Users.AddOrUpdate(u => u.Username, users.ToArray());
            context.SaveChanges();

            // Seed Categories
            var categories = new List<Category>
            {
                new Category { CategoryID = 1, CategoryName = "Electronics", Description = "Electronic devices and gadgets" },
                new Category { CategoryID = 2, CategoryName = "Clothing", Description = "Apparel and fashion items" },
                new Category { CategoryID = 3, CategoryName = "Books", Description = "Books and publications" },
                new Category { CategoryID = 4, CategoryName = "Home & Garden", Description = "Home and garden products" }
            };

            context.Categories.AddOrUpdate(c => c.CategoryName, categories.ToArray());
            context.SaveChanges();

            // Seed Products
            var products = new List<Product>
            {
                new Product
                {
                    ProductID = 1,
                    ProductName = "Laptop Dell XPS 15",
                    Description = "High-performance laptop for professionals",
                    Price = 1299.99m,
                    CategoryID = 1,
                    StoreOwnerID = 1,
                    ImageURL = "https://via.placeholder.com/300x300?text=Laptop",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new Product
                {
                    ProductID = 2,
                    ProductName = "iPhone 14 Pro",
                    Description = "Latest iPhone with advanced camera",
                    Price = 999.99m,
                    CategoryID = 1,
                    StoreOwnerID = 1,
                    ImageURL = "https://via.placeholder.com/300x300?text=iPhone",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new Product
                {
                    ProductID = 3,
                    ProductName = "Sony Headphones WH-1000XM5",
                    Description = "Noise-cancelling wireless headphones",
                    Price = 399.99m,
                    CategoryID = 1,
                    StoreOwnerID = 1,
                    ImageURL = "https://via.placeholder.com/300x300?text=Headphones",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new Product
                {
                    ProductID = 4,
                    ProductName = "Men's T-Shirt",
                    Description = "Cotton t-shirt for everyday wear",
                    Price = 19.99m,
                    CategoryID = 2,
                    StoreOwnerID = 1,
                    ImageURL = "https://via.placeholder.com/300x300?text=T-Shirt",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new Product
                {
                    ProductID = 5,
                    ProductName = "Women's Jeans",
                    Description = "Comfortable denim jeans",
                    Price = 49.99m,
                    CategoryID = 2,
                    StoreOwnerID = 1,
                    ImageURL = "https://via.placeholder.com/300x300?text=Jeans",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new Product
                {
                    ProductID = 6,
                    ProductName = "The Great Gatsby",
                    Description = "Classic American novel",
                    Price = 12.99m,
                    CategoryID = 3,
                    StoreOwnerID = 1,
                    ImageURL = "https://via.placeholder.com/300x300?text=Book",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                },
                new Product
                {
                    ProductID = 7,
                    ProductName = "Garden Tools Set",
                    Description = "Complete gardening tools set",
                    Price = 79.99m,
                    CategoryID = 4,
                    StoreOwnerID = 1,
                    ImageURL = "https://via.placeholder.com/300x300?text=Tools",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                }
            };

            context.Products.AddOrUpdate(p => p.ProductName, products.ToArray());
            context.SaveChanges();

            // Seed Inventory
            var inventories = new List<Inventory>
            {
                new Inventory { InventoryID = 1, ProductID = 1, StockQuantity = 50, MinimumStock = 10, LastUpdated = DateTime.Now },
                new Inventory { InventoryID = 2, ProductID = 2, StockQuantity = 100, MinimumStock = 20, LastUpdated = DateTime.Now },
                new Inventory { InventoryID = 3, ProductID = 3, StockQuantity = 200, MinimumStock = 30, LastUpdated = DateTime.Now },
                new Inventory { InventoryID = 4, ProductID = 4, StockQuantity = 500, MinimumStock = 100, LastUpdated = DateTime.Now },
                new Inventory { InventoryID = 5, ProductID = 5, StockQuantity = 300, MinimumStock = 50, LastUpdated = DateTime.Now },
                new Inventory { InventoryID = 6, ProductID = 6, StockQuantity = 150, MinimumStock = 25, LastUpdated = DateTime.Now },
                new Inventory { InventoryID = 7, ProductID = 7, StockQuantity = 75, MinimumStock = 15, LastUpdated = DateTime.Now }
            };

            context.Inventories.AddOrUpdate(i => i.ProductID, inventories.ToArray());
            context.SaveChanges();

            // Seed Orders
            var orders = new List<Order>
            {
                new Order
                {
                    OrderID = 1,
                    CustomerID = 2,
                    OrderDate = DateTime.Now.AddDays(-5),
                    OrderStatus = "Delivered",
                    TotalAmount = 1719.98m,
                    ShippingAddress = "123 Main St, New York, NY 10001"
                },
                new Order
                {
                    OrderID = 2,
                    CustomerID = 3,
                    OrderDate = DateTime.Now.AddDays(-2),
                    OrderStatus = "Shipped",
                    TotalAmount = 449.98m,
                    ShippingAddress = "456 Oak Ave, Los Angeles, CA 90001"
                }
            };

            context.Orders.AddOrUpdate(o => o.OrderID, orders.ToArray());
            context.SaveChanges();

            // Seed Order Details
            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { OrderDetailID = 1, OrderID = 1, ProductID = 1, Quantity = 1, UnitPrice = 1299.99m },
                new OrderDetail { OrderDetailID = 2, OrderID = 1, ProductID = 3, Quantity = 1, UnitPrice = 399.99m },
                new OrderDetail { OrderDetailID = 3, OrderID = 1, ProductID = 4, Quantity = 1, UnitPrice = 19.99m },
                new OrderDetail { OrderDetailID = 4, OrderID = 2, ProductID = 3, Quantity = 1, UnitPrice = 399.99m },
                new OrderDetail { OrderDetailID = 5, OrderID = 2, ProductID = 5, Quantity = 1, UnitPrice = 49.99m }
            };

            context.OrderDetails.AddOrUpdate(od => od.OrderDetailID, orderDetails.ToArray());
            context.SaveChanges();

            // Seed Support Tickets
            var tickets = new List<SupportTicket>
            {
                new SupportTicket
                {
                    TicketID = 1,
                    CustomerID = 2,
                    Subject = "Product Delivery Issue",
                    Description = "My order hasn't arrived yet",
                    Status = "Open",
                    Priority = "High",
                    CreatedDate = DateTime.Now.AddDays(-1)
                },
                new SupportTicket
                {
                    TicketID = 2,
                    CustomerID = 3,
                    Subject = "Product Specification Question",
                    Description = "Can you provide more details about the laptop specs?",
                    Status = "Open",
                    Priority = "Medium",
                    CreatedDate = DateTime.Now
                }
            };

            context.SupportTickets.AddOrUpdate(t => t.TicketID, tickets.ToArray());
            context.SaveChanges();

            // Seed Reviews
            var reviews = new List<Review>
            {
                new Review
                {
                    ReviewID = 1,
                    ProductID = 1,
                    CustomerID = 2,
                    Rating = 5,
                    Comment = "Excellent laptop! Very fast and reliable.",
                    ReviewDate = DateTime.Now.AddDays(-3)
                },
                new Review
                {
                    ReviewID = 2,
                    ProductID = 3,
                    CustomerID = 3,
                    Rating = 4,
                    Comment = "Great sound quality, but a bit pricey.",
                    ReviewDate = DateTime.Now.AddDays(-1)
                }
            };

            context.Reviews.AddOrUpdate(r => r.ReviewID, reviews.ToArray());
            context.SaveChanges();
        }
    }
}
