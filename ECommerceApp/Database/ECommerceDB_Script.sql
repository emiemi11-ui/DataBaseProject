-- =====================================================
-- E-COMMERCE DATABASE - DB FIRST APPROACH
-- Conform Curs 10 - ADO.NET Entity Framework
-- =====================================================

USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'ECommerceDB')
BEGIN
    ALTER DATABASE ECommerceDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ECommerceDB;
END
GO

-- Create Database
CREATE DATABASE ECommerceDB;
GO

USE ECommerceDB;
GO

-- =====================================================
-- TABELE PRINCIPALE
-- =====================================================

-- Tabel Users (Store Owners, Customers, Customer Service)
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    HashedPassword NVARCHAR(255) NOT NULL,
    UserRole NVARCHAR(20) NOT NULL CHECK (UserRole IN ('StoreOwner', 'Customer', 'CustomerService')),
    FirstName NVARCHAR(50) NULL,
    LastName NVARCHAR(50) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Address NVARCHAR(500) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);

-- Tabel Categories
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL,
    IconCode NVARCHAR(10) NULL  -- Unicode pentru iconițe
);

-- Tabel Products
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Price DECIMAL(18,2) NOT NULL CHECK (Price >= 0),
    CategoryID INT NOT NULL,
    StoreOwnerID INT NOT NULL,
    ImageURL NVARCHAR(500) NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    CONSTRAINT FK_Products_Users FOREIGN KEY (StoreOwnerID) REFERENCES Users(UserID)
);

-- Tabel Inventory (relație one-to-one cu Products)
CREATE TABLE Inventory (
    InventoryID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL UNIQUE,  -- UNIQUE pentru one-to-one
    StockQuantity INT NOT NULL DEFAULT 0 CHECK (StockQuantity >= 0),
    MinimumStock INT NOT NULL DEFAULT 5 CHECK (MinimumStock >= 0),
    LastUpdated DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Inventory_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE
);

-- Tabel Orders
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (TotalAmount >= 0),
    OrderStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending'
        CHECK (OrderStatus IN ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled')),
    ShippingAddress NVARCHAR(500) NULL,
    PaymentMethod NVARCHAR(50) NULL,
    CONSTRAINT FK_Orders_Users FOREIGN KEY (CustomerID) REFERENCES Users(UserID)
);

-- Tabel OrderDetails (relație one-to-many cu Orders)
CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(18,2) NOT NULL CHECK (UnitPrice >= 0),
    Subtotal AS (Quantity * UnitPrice) PERSISTED,
    CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE,
    CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

-- Tabel SupportTickets
CREATE TABLE SupportTickets (
    TicketID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT NOT NULL,
    Subject NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Open'
        CHECK (Status IN ('Open', 'InProgress', 'Resolved', 'Closed')),
    Priority NVARCHAR(20) NOT NULL DEFAULT 'Medium'
        CHECK (Priority IN ('Low', 'Medium', 'High')),
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    AssignedToID INT NULL,
    ResolvedDate DATETIME NULL,
    CONSTRAINT FK_SupportTickets_Customer FOREIGN KEY (CustomerID) REFERENCES Users(UserID),
    CONSTRAINT FK_SupportTickets_Assigned FOREIGN KEY (AssignedToID) REFERENCES Users(UserID)
);

-- Tabel TicketMessages (pentru conversații în tickets)
CREATE TABLE TicketMessages (
    MessageID INT IDENTITY(1,1) PRIMARY KEY,
    TicketID INT NOT NULL,
    UserID INT NOT NULL,
    MessageText NVARCHAR(MAX) NOT NULL,
    MessageDate DATETIME NOT NULL DEFAULT GETDATE(),
    IsFromCustomer BIT NOT NULL,
    CONSTRAINT FK_TicketMessages_Tickets FOREIGN KEY (TicketID) REFERENCES SupportTickets(TicketID) ON DELETE CASCADE,
    CONSTRAINT FK_TicketMessages_Users FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Tabel Reviews
CREATE TABLE Reviews (
    ReviewID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL,
    CustomerID INT NOT NULL,
    Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
    Comment NVARCHAR(1000) NULL,
    ReviewDate DATETIME NOT NULL DEFAULT GETDATE(),
    IsVerifiedPurchase BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Reviews_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE,
    CONSTRAINT FK_Reviews_Users FOREIGN KEY (CustomerID) REFERENCES Users(UserID),
    CONSTRAINT UQ_Reviews_ProductCustomer UNIQUE (ProductID, CustomerID)
);

-- Tabel Tags (pentru many-to-many)
CREATE TABLE Tags (
    TagID INT IDENTITY(1,1) PRIMARY KEY,
    TagName NVARCHAR(50) NOT NULL UNIQUE,
    TagColor NVARCHAR(7) NULL  -- Hex color code
);

-- Tabel de legătură ProductTags (many-to-many între Products și Tags)
CREATE TABLE ProductTags (
    ProductID INT NOT NULL,
    TagID INT NOT NULL,
    PRIMARY KEY (ProductID, TagID),
    CONSTRAINT FK_ProductTags_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE,
    CONSTRAINT FK_ProductTags_Tags FOREIGN KEY (TagID) REFERENCES Tags(TagID) ON DELETE CASCADE
);

-- Tabel StoreSettings (pentru configurări magazin)
CREATE TABLE StoreSettings (
    SettingID INT IDENTITY(1,1) PRIMARY KEY,
    SettingKey NVARCHAR(100) NOT NULL UNIQUE,
    SettingValue NVARCHAR(500) NULL,
    SettingType NVARCHAR(50) NOT NULL,  -- 'Text', 'Color', 'Boolean', 'Number'
    Description NVARCHAR(500) NULL,
    LastUpdated DATETIME NOT NULL DEFAULT GETDATE()
);

-- =====================================================
-- INDEXURI PENTRU PERFORMANCE
-- =====================================================
CREATE INDEX IX_Products_CategoryID ON Products(CategoryID);
CREATE INDEX IX_Products_StoreOwnerID ON Products(StoreOwnerID);
CREATE INDEX IX_Products_IsActive ON Products(IsActive);
CREATE INDEX IX_Inventory_StockQuantity ON Inventory(StockQuantity);
CREATE INDEX IX_Orders_CustomerID ON Orders(CustomerID);
CREATE INDEX IX_Orders_OrderStatus ON Orders(OrderStatus);
CREATE INDEX IX_Orders_OrderDate ON Orders(OrderDate);
CREATE INDEX IX_OrderDetails_OrderID ON OrderDetails(OrderID);
CREATE INDEX IX_OrderDetails_ProductID ON OrderDetails(ProductID);
CREATE INDEX IX_SupportTickets_CustomerID ON SupportTickets(CustomerID);
CREATE INDEX IX_SupportTickets_Status ON SupportTickets(Status);
CREATE INDEX IX_SupportTickets_AssignedToID ON SupportTickets(AssignedToID);
CREATE INDEX IX_Reviews_ProductID ON Reviews(ProductID);
CREATE INDEX IX_Reviews_CustomerID ON Reviews(CustomerID);
GO

-- =====================================================
-- STORED PROCEDURES (pentru Function Imports în EDM)
-- =====================================================

-- SP pentru low stock products
CREATE PROCEDURE GetLowStockProducts
AS
BEGIN
    SELECT p.ProductID, p.ProductName, p.Price, i.StockQuantity, i.MinimumStock,
           c.CategoryName, (i.MinimumStock - i.StockQuantity) AS StockNeeded
    FROM Products p
    INNER JOIN Inventory i ON p.ProductID = i.ProductID
    INNER JOIN Categories c ON p.CategoryID = c.CategoryID
    WHERE i.StockQuantity < i.MinimumStock AND p.IsActive = 1
    ORDER BY (i.MinimumStock - i.StockQuantity) DESC;
END
GO

-- SP pentru sales statistics
CREATE PROCEDURE GetSalesStatistics
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL
AS
BEGIN
    SET @StartDate = ISNULL(@StartDate, DATEADD(MONTH, -1, GETDATE()));
    SET @EndDate = ISNULL(@EndDate, GETDATE());

    SELECT
        COUNT(DISTINCT o.OrderID) AS TotalOrders,
        ISNULL(SUM(o.TotalAmount), 0) AS TotalRevenue,
        ISNULL(AVG(o.TotalAmount), 0) AS AverageOrderValue,
        COUNT(DISTINCT o.CustomerID) AS UniqueCustomers,
        COUNT(DISTINCT od.ProductID) AS ProductsSold
    FROM Orders o
    LEFT JOIN OrderDetails od ON o.OrderID = od.OrderID
    WHERE o.OrderDate BETWEEN @StartDate AND @EndDate
        AND o.OrderStatus NOT IN ('Cancelled');
END
GO

-- SP pentru popular products
CREATE PROCEDURE GetPopularProducts
    @TopCount INT = 10
AS
BEGIN
    SELECT TOP (@TopCount)
        p.ProductID, p.ProductName, p.Price, c.CategoryName,
        COUNT(od.OrderDetailID) AS OrderCount,
        ISNULL(SUM(od.Quantity), 0) AS TotalQuantitySold,
        ISNULL(SUM(od.Subtotal), 0) AS TotalRevenue,
        AVG(CAST(r.Rating AS FLOAT)) AS AverageRating,
        COUNT(r.ReviewID) AS ReviewCount
    FROM Products p
    INNER JOIN Categories c ON p.CategoryID = c.CategoryID
    LEFT JOIN OrderDetails od ON p.ProductID = od.ProductID
    LEFT JOIN Reviews r ON p.ProductID = r.ProductID
    WHERE p.IsActive = 1
    GROUP BY p.ProductID, p.ProductName, p.Price, c.CategoryName
    ORDER BY TotalQuantitySold DESC;
END
GO

-- SP pentru customer order history
CREATE PROCEDURE GetCustomerOrderHistory
    @CustomerID INT
AS
BEGIN
    SELECT
        o.OrderID,
        o.OrderDate,
        o.TotalAmount,
        o.OrderStatus,
        o.ShippingAddress,
        o.PaymentMethod,
        COUNT(od.OrderDetailID) AS ItemCount
    FROM Orders o
    LEFT JOIN OrderDetails od ON o.OrderID = od.OrderID
    WHERE o.CustomerID = @CustomerID
    GROUP BY o.OrderID, o.OrderDate, o.TotalAmount, o.OrderStatus, o.ShippingAddress, o.PaymentMethod
    ORDER BY o.OrderDate DESC;
END
GO

-- SP pentru dashboard statistics
CREATE PROCEDURE GetDashboardStatistics
AS
BEGIN
    -- Total Users by Role
    SELECT
        (SELECT COUNT(*) FROM Users WHERE UserRole = 'Customer' AND IsActive = 1) AS TotalCustomers,
        (SELECT COUNT(*) FROM Users WHERE UserRole = 'StoreOwner' AND IsActive = 1) AS TotalStoreOwners,
        (SELECT COUNT(*) FROM Users WHERE UserRole = 'CustomerService' AND IsActive = 1) AS TotalSupportAgents,
        (SELECT COUNT(*) FROM Products WHERE IsActive = 1) AS TotalProducts,
        (SELECT COUNT(*) FROM Orders WHERE OrderStatus = 'Pending') AS PendingOrders,
        (SELECT COUNT(*) FROM SupportTickets WHERE Status IN ('Open', 'InProgress')) AS OpenTickets,
        (SELECT ISNULL(SUM(TotalAmount), 0) FROM Orders WHERE MONTH(OrderDate) = MONTH(GETDATE()) AND YEAR(OrderDate) = YEAR(GETDATE()) AND OrderStatus != 'Cancelled') AS MonthlyRevenue,
        (SELECT COUNT(*) FROM Inventory WHERE StockQuantity < MinimumStock) AS LowStockProducts;
END
GO

-- =====================================================
-- DATE DE TEST
-- =====================================================

-- Users (Password: "password123" - SHA256 hash)
INSERT INTO Users (Username, Email, HashedPassword, UserRole, FirstName, LastName, PhoneNumber, Address, IsActive) VALUES
('admin', 'admin@ecommerce.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'StoreOwner', 'Admin', 'User', '0712345678', 'Bucharest, Romania', 1),
('john_customer', 'john@email.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Customer', 'John', 'Doe', '0723456789', '123 Main St, New York, NY', 1),
('jane_customer', 'jane@email.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Customer', 'Jane', 'Smith', '0734567890', '456 Oak Ave, Los Angeles, CA', 1),
('support1', 'support@ecommerce.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'CustomerService', 'Support', 'Agent', '0745678901', 'Bucharest, Romania', 1),
('mary_customer', 'mary@email.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Customer', 'Mary', 'Jones', '0756789012', '789 Pine Rd, Chicago, IL', 1);

-- Categories
INSERT INTO Categories (CategoryName, Description, IconCode) VALUES
('Electronics', 'Electronic devices and gadgets', N'📱'),
('Clothing', 'Apparel and fashion items', N'👕'),
('Books', 'Physical and digital books', N'📚'),
('Home & Garden', 'Home decor and garden supplies', N'🏠'),
('Sports', 'Sports equipment and fitness gear', N'⚽');

-- Products
INSERT INTO Products (ProductName, Description, Price, CategoryID, StoreOwnerID, ImageURL, IsActive) VALUES
('iPhone 15 Pro', 'Latest Apple smartphone with A17 Pro chip', 1299.99, 1, 1, '/images/iphone15.jpg', 1),
('Samsung Galaxy S24', 'Flagship Android smartphone with AI', 999.99, 1, 1, '/images/galaxys24.jpg', 1),
('Sony WH-1000XM5', 'Noise canceling wireless headphones', 349.99, 1, 1, '/images/sonywh.jpg', 1),
('MacBook Pro 16"', 'Professional laptop with M3 Max chip', 2499.99, 1, 1, '/images/macbook.jpg', 1),
('Nike Air Max 270', 'Lifestyle sneakers with Max Air', 159.99, 2, 1, '/images/airmax.jpg', 1),
('Levi''s 501 Jeans', 'Classic straight fit denim jeans', 89.99, 2, 1, '/images/levis.jpg', 1),
('North Face Jacket', 'Waterproof winter jacket', 249.99, 2, 1, '/images/jacket.jpg', 1),
('Clean Code Book', 'Software craftsmanship handbook', 39.99, 3, 1, '/images/cleancode.jpg', 1),
('Design Patterns', 'Gang of Four design patterns book', 44.99, 3, 1, '/images/patterns.jpg', 1),
('Modern Desk Lamp', 'LED lamp with wireless charging', 79.99, 4, 1, '/images/lamp.jpg', 1),
('Indoor Plant Set', '3 low-maintenance plants', 45.99, 4, 1, '/images/plants.jpg', 1),
('Yoga Mat Premium', 'Extra thick non-slip mat', 34.99, 5, 1, '/images/yogamat.jpg', 1),
('Dumbbell Set 20kg', 'Adjustable weight system', 199.99, 5, 1, '/images/dumbbells.jpg', 1),
('Running Shoes', 'Lightweight marathon shoes', 129.99, 5, 1, '/images/running.jpg', 1);

-- Inventory
INSERT INTO Inventory (ProductID, StockQuantity, MinimumStock) VALUES
(1, 50, 10), (2, 75, 15), (3, 100, 20), (4, 30, 5),
(5, 200, 25), (6, 150, 30), (7, 80, 15), (8, 120, 20),
(9, 90, 15), (10, 60, 10), (11, 45, 10), (12, 90, 15),
(13, 35, 8), (14, 110, 20);

-- Tags
INSERT INTO Tags (TagName, TagColor) VALUES
('Best Seller', '#FFD700'),
('New Arrival', '#4CAF50'),
('On Sale', '#F44336'),
('Limited Stock', '#FF9800'),
('Premium', '#9C27B0'),
('Eco-Friendly', '#8BC34A');

-- ProductTags (many-to-many relationship)
INSERT INTO ProductTags (ProductID, TagID) VALUES
(1, 1), (1, 5),  -- iPhone: Best Seller, Premium
(2, 2), (2, 3),  -- Samsung: New Arrival, On Sale
(3, 1), (3, 5),  -- Sony: Best Seller, Premium
(4, 5),          -- MacBook: Premium
(5, 1),          -- Nike: Best Seller
(8, 1), (8, 2),  -- Clean Code: Best Seller, New Arrival
(12, 6);         -- Yoga Mat: Eco-Friendly

-- Orders
INSERT INTO Orders (CustomerID, OrderDate, TotalAmount, OrderStatus, ShippingAddress, PaymentMethod) VALUES
(2, DATEADD(DAY, -10, GETDATE()), 1649.98, 'Delivered', '123 Main St, New York, NY', 'Credit Card'),
(2, DATEADD(DAY, -5, GETDATE()), 429.98, 'Delivered', '123 Main St, New York, NY', 'PayPal'),
(3, DATEADD(DAY, -2, GETDATE()), 1089.98, 'Processing', '456 Oak Ave, Los Angeles, CA', 'Credit Card'),
(5, DATEADD(DAY, -1, GETDATE()), 234.98, 'Pending', '789 Pine Rd, Chicago, IL', 'Credit Card'),
(2, GETDATE(), 349.99, 'Pending', '123 Main St, New York, NY', 'Credit Card');

-- OrderDetails
INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice) VALUES
-- Order 1
(1, 1, 1, 1299.99), (1, 3, 1, 349.99),
-- Order 2
(2, 5, 1, 159.99), (2, 6, 2, 89.99), (2, 12, 1, 34.99),
-- Order 3
(3, 2, 1, 999.99), (3, 8, 2, 39.99),
-- Order 4
(4, 10, 1, 79.99), (4, 11, 1, 45.99), (4, 8, 1, 39.99), (4, 9, 1, 44.99),
-- Order 5
(5, 3, 1, 349.99);

-- SupportTickets
INSERT INTO SupportTickets (CustomerID, Subject, Description, Status, Priority, AssignedToID) VALUES
(2, 'Order Delivery Issue', 'Order #1 marked as delivered but not received', 'InProgress', 'High', 4),
(3, 'Product Return Request', 'Samsung has dead pixel, want to return', 'Open', 'Medium', NULL),
(5, 'Payment Question', 'Need clarification on charges', 'Resolved', 'Low', 4),
(2, 'Warranty Information', 'How to register iPhone warranty?', 'Open', 'Low', NULL);

-- TicketMessages
INSERT INTO TicketMessages (TicketID, UserID, MessageText, IsFromCustomer) VALUES
(1, 2, 'My package shows delivered but I did not receive it.', 1),
(1, 4, 'I apologize for the inconvenience. Let me track your package.', 0),
(1, 4, 'The package was delivered to your building reception. Can you check there?', 0),
(3, 5, 'Can you explain the tax calculation on my order?', 1),
(3, 4, 'Taxes are calculated based on your state rate of 8.5%. Let me break it down...', 0);

-- Reviews
INSERT INTO Reviews (ProductID, CustomerID, Rating, Comment, IsVerifiedPurchase) VALUES
(1, 2, 5, 'Best phone ever! Camera is incredible.', 1),
(3, 2, 5, 'Amazing noise cancellation. Very comfortable.', 1),
(5, 3, 4, 'Great sneakers but color slightly different from picture.', 0),
(8, 5, 5, 'Must-read for every developer!', 1),
(12, 3, 4, 'Good quality mat. Helpful alignment guides.', 0);

-- StoreSettings
INSERT INTO StoreSettings (SettingKey, SettingValue, SettingType, Description) VALUES
('StoreName', 'TechStore Premium', 'Text', 'Name of the store'),
('StoreEmail', 'contact@techstore.com', 'Text', 'Contact email'),
('StorePhone', '1-800-TECH-STORE', 'Text', 'Contact phone number'),
('PrimaryColor', '#2196F3', 'Color', 'Primary brand color'),
('AccentColor', '#FF5722', 'Color', 'Accent color'),
('CurrencySymbol', 'RON', 'Text', 'Currency symbol'),
('TaxRate', '19', 'Number', 'VAT percentage'),
('MinimumOrderAmount', '50', 'Number', 'Minimum order amount'),
('FreeShippingThreshold', '200', 'Number', 'Free shipping above this amount'),
('AllowGuestCheckout', 'true', 'Boolean', 'Allow checkout without account');

GO

-- =====================================================
-- VERIFICARE DATE
-- =====================================================
SELECT 'Database created successfully!' AS Status;
SELECT 'Users' AS TableName, COUNT(*) AS Records FROM Users
UNION ALL SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL SELECT 'Products', COUNT(*) FROM Products
UNION ALL SELECT 'Inventory', COUNT(*) FROM Inventory
UNION ALL SELECT 'Tags', COUNT(*) FROM Tags
UNION ALL SELECT 'ProductTags', COUNT(*) FROM ProductTags
UNION ALL SELECT 'Orders', COUNT(*) FROM Orders
UNION ALL SELECT 'OrderDetails', COUNT(*) FROM OrderDetails
UNION ALL SELECT 'SupportTickets', COUNT(*) FROM SupportTickets
UNION ALL SELECT 'TicketMessages', COUNT(*) FROM TicketMessages
UNION ALL SELECT 'Reviews', COUNT(*) FROM Reviews
UNION ALL SELECT 'StoreSettings', COUNT(*) FROM StoreSettings;

PRINT '';
PRINT '=====================================================';
PRINT '  E-Commerce Database created successfully!';
PRINT '=====================================================';
PRINT '';
PRINT '  Demo Users (password: password123):';
PRINT '    - admin (StoreOwner)';
PRINT '    - john_customer (Customer)';
PRINT '    - jane_customer (Customer)';
PRINT '    - support1 (CustomerService)';
PRINT '    - mary_customer (Customer)';
PRINT '';
PRINT '  Stored Procedures Available:';
PRINT '    - GetLowStockProducts';
PRINT '    - GetSalesStatistics';
PRINT '    - GetPopularProducts';
PRINT '    - GetCustomerOrderHistory';
PRINT '    - GetDashboardStatistics';
PRINT '';
GO
