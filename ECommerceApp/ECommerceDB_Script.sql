-- =====================================================
-- E-COMMERCE DATABASE SCRIPT
-- Complete SQL Server Database for WPF E-Commerce App
-- =====================================================

-- Create Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ECommerceDB')
BEGIN
    CREATE DATABASE ECommerceDB;
END
GO

USE ECommerceDB;
GO

-- =====================================================
-- DROP EXISTING TABLES (for clean reinstall)
-- =====================================================
IF OBJECT_ID('dbo.Reviews', 'U') IS NOT NULL DROP TABLE dbo.Reviews;
IF OBJECT_ID('dbo.OrderDetails', 'U') IS NOT NULL DROP TABLE dbo.OrderDetails;
IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID('dbo.SupportTickets', 'U') IS NOT NULL DROP TABLE dbo.SupportTickets;
IF OBJECT_ID('dbo.Inventory', 'U') IS NOT NULL DROP TABLE dbo.Inventory;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
GO

-- =====================================================
-- TABLE 1: USERS
-- =====================================================
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    HashedPassword NVARCHAR(255) NOT NULL,
    UserRole NVARCHAR(20) NOT NULL CHECK (UserRole IN ('StoreOwner', 'Customer', 'CustomerService')),
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);
GO

-- =====================================================
-- TABLE 2: CATEGORIES
-- =====================================================
CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL
);
GO

-- =====================================================
-- TABLE 3: PRODUCTS
-- =====================================================
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
GO

-- =====================================================
-- TABLE 4: INVENTORY
-- =====================================================
CREATE TABLE Inventory (
    InventoryID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL UNIQUE,
    StockQuantity INT NOT NULL DEFAULT 0 CHECK (StockQuantity >= 0),
    MinimumStock INT NOT NULL DEFAULT 5 CHECK (MinimumStock >= 0),
    LastUpdated DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Inventory_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE
);
GO

-- =====================================================
-- TABLE 5: ORDERS
-- =====================================================
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (TotalAmount >= 0),
    OrderStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending'
        CHECK (OrderStatus IN ('Pending', 'Processing', 'Shipped', 'Delivered', 'Cancelled')),
    ShippingAddress NVARCHAR(500) NULL,
    CONSTRAINT FK_Orders_Users FOREIGN KEY (CustomerID) REFERENCES Users(UserID)
);
GO

-- =====================================================
-- TABLE 6: ORDER DETAILS
-- =====================================================
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
GO

-- =====================================================
-- TABLE 7: SUPPORT TICKETS
-- =====================================================
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
GO

-- =====================================================
-- TABLE 8: REVIEWS
-- =====================================================
CREATE TABLE Reviews (
    ReviewID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL,
    CustomerID INT NOT NULL,
    Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
    Comment NVARCHAR(1000) NULL,
    ReviewDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Reviews_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE,
    CONSTRAINT FK_Reviews_Users FOREIGN KEY (CustomerID) REFERENCES Users(UserID),
    CONSTRAINT UQ_Reviews_ProductCustomer UNIQUE (ProductID, CustomerID)
);
GO

-- =====================================================
-- CREATE INDEXES FOR PERFORMANCE
-- =====================================================
CREATE INDEX IX_Products_CategoryID ON Products(CategoryID);
CREATE INDEX IX_Products_StoreOwnerID ON Products(StoreOwnerID);
CREATE INDEX IX_Orders_CustomerID ON Orders(CustomerID);
CREATE INDEX IX_Orders_OrderStatus ON Orders(OrderStatus);
CREATE INDEX IX_OrderDetails_OrderID ON OrderDetails(OrderID);
CREATE INDEX IX_SupportTickets_CustomerID ON SupportTickets(CustomerID);
CREATE INDEX IX_SupportTickets_Status ON SupportTickets(Status);
CREATE INDEX IX_Reviews_ProductID ON Reviews(ProductID);
GO

-- =====================================================
-- INSERT SAMPLE DATA
-- =====================================================

-- USERS (Password: "password123" hashed with SHA256)
-- SHA256 hash of "password123" = "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f"
INSERT INTO Users (Username, Email, HashedPassword, UserRole, CreatedDate, IsActive) VALUES
('admin', 'admin@ecommerce.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'StoreOwner', GETDATE(), 1),
('john_doe', 'john.doe@email.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Customer', GETDATE(), 1),
('jane_smith', 'jane.smith@email.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Customer', GETDATE(), 1),
('support1', 'support@ecommerce.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'CustomerService', GETDATE(), 1),
('mary_jones', 'mary.jones@email.com', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'Customer', GETDATE(), 1);
GO

-- CATEGORIES
INSERT INTO Categories (CategoryName, Description) VALUES
('Electronics', 'Electronic devices, gadgets, and accessories'),
('Clothing', 'Apparel, fashion items, and accessories'),
('Books', 'Physical books, e-books, and reading materials'),
('Home & Garden', 'Home decor, furniture, and garden supplies'),
('Sports', 'Sports equipment, fitness gear, and outdoor items');
GO

-- PRODUCTS
INSERT INTO Products (ProductName, Description, Price, CategoryID, StoreOwnerID, ImageURL, CreatedDate, IsActive) VALUES
('iPhone 15 Pro', 'Latest Apple smartphone with A17 Pro chip, titanium design, and advanced camera system.', 1299.99, 1, 1, '/images/iphone15.jpg', GETDATE(), 1),
('Samsung Galaxy S24', 'Flagship Android smartphone with Galaxy AI, 200MP camera, and stunning display.', 999.99, 1, 1, '/images/galaxys24.jpg', GETDATE(), 1),
('Sony WH-1000XM5', 'Industry-leading noise canceling wireless headphones with exceptional sound quality.', 349.99, 1, 1, '/images/sonywh1000.jpg', GETDATE(), 1),
('Nike Air Max 270', 'Iconic lifestyle sneakers with Max Air cushioning for all-day comfort.', 159.99, 2, 1, '/images/airmax270.jpg', GETDATE(), 1),
('Levi''s 501 Original Jeans', 'Classic straight fit jeans with iconic button fly and timeless style.', 89.99, 2, 1, '/images/levis501.jpg', GETDATE(), 1),
('The Art of Programming', 'Comprehensive guide to software development best practices and patterns.', 49.99, 3, 1, '/images/artofprog.jpg', GETDATE(), 1),
('Clean Code', 'A handbook of agile software craftsmanship by Robert C. Martin.', 39.99, 3, 1, '/images/cleancode.jpg', GETDATE(), 1),
('Modern Desk Lamp', 'Adjustable LED desk lamp with wireless charging pad and touch controls.', 79.99, 4, 1, '/images/desklamp.jpg', GETDATE(), 1),
('Indoor Plant Set', 'Set of 3 low-maintenance indoor plants perfect for home or office.', 45.99, 4, 1, '/images/plants.jpg', GETDATE(), 1),
('Yoga Mat Premium', 'Extra thick, non-slip yoga mat with carrying strap and alignment guides.', 34.99, 5, 1, '/images/yogamat.jpg', GETDATE(), 1),
('Dumbbell Set 20kg', 'Adjustable dumbbell set with quick-change weight system, 2-20kg range.', 199.99, 5, 1, '/images/dumbbells.jpg', GETDATE(), 1);
GO

-- INVENTORY
INSERT INTO Inventory (ProductID, StockQuantity, MinimumStock, LastUpdated) VALUES
(1, 50, 10, GETDATE()),   -- iPhone 15 Pro
(2, 75, 15, GETDATE()),   -- Samsung Galaxy S24
(3, 100, 20, GETDATE()),  -- Sony WH-1000XM5
(4, 200, 25, GETDATE()),  -- Nike Air Max 270
(5, 150, 30, GETDATE()),  -- Levi's 501
(6, 80, 10, GETDATE()),   -- The Art of Programming
(7, 120, 15, GETDATE()),  -- Clean Code
(8, 60, 10, GETDATE()),   -- Modern Desk Lamp
(9, 45, 10, GETDATE()),   -- Indoor Plant Set
(10, 90, 15, GETDATE()),  -- Yoga Mat Premium
(11, 35, 8, GETDATE());   -- Dumbbell Set
GO

-- ORDERS
INSERT INTO Orders (CustomerID, OrderDate, TotalAmount, OrderStatus, ShippingAddress) VALUES
(2, DATEADD(DAY, -5, GETDATE()), 1649.98, 'Delivered', '123 Main Street, New York, NY 10001'),
(2, DATEADD(DAY, -2, GETDATE()), 429.98, 'Processing', '123 Main Street, New York, NY 10001'),
(3, DATEADD(DAY, -1, GETDATE()), 1089.98, 'Pending', '456 Oak Avenue, Los Angeles, CA 90001'),
(5, GETDATE(), 234.98, 'Pending', '789 Pine Road, Chicago, IL 60601');
GO

-- ORDER DETAILS
INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice) VALUES
-- Order 1: iPhone + Sony Headphones
(1, 1, 1, 1299.99),
(1, 3, 1, 349.99),
-- Order 2: Nike + Levi's + Yoga Mat
(2, 4, 1, 159.99),
(2, 5, 2, 89.99),
(2, 10, 1, 34.99),
-- Order 3: Galaxy S24 + Clean Code
(3, 2, 1, 999.99),
(3, 7, 2, 39.99),
-- Order 4: Desk Lamp + Plant Set + Books
(4, 8, 1, 79.99),
(4, 9, 1, 45.99),
(4, 6, 1, 49.99),
(4, 7, 1, 39.99);
GO

-- SUPPORT TICKETS
INSERT INTO SupportTickets (CustomerID, Subject, Description, Status, Priority, CreatedDate, AssignedToID, ResolvedDate) VALUES
(2, 'Order Delivery Issue', 'My order #1 was marked as delivered but I haven''t received it yet. Please help track the package.', 'InProgress', 'High', DATEADD(DAY, -3, GETDATE()), 4, NULL),
(3, 'Product Return Request', 'I would like to return the Samsung Galaxy S24 from order #3. The screen has a dead pixel.', 'Open', 'Medium', DATEADD(DAY, -1, GETDATE()), NULL, NULL),
(5, 'Payment Question', 'Can you help me understand the charges on my recent order? The total seems different from what I calculated.', 'Resolved', 'Low', DATEADD(DAY, -7, GETDATE()), 4, DATEADD(DAY, -6, GETDATE())),
(2, 'Product Warranty Info', 'How do I register my iPhone 15 Pro for extended warranty coverage?', 'Open', 'Low', GETDATE(), NULL, NULL);
GO

-- REVIEWS
INSERT INTO Reviews (ProductID, CustomerID, Rating, Comment, ReviewDate) VALUES
(1, 2, 5, 'Absolutely love this phone! The camera is incredible and the titanium feels premium. Best iPhone yet!', DATEADD(DAY, -4, GETDATE())),
(3, 2, 5, 'These headphones are amazing. The noise cancellation is top-notch and they''re so comfortable for long listening sessions.', DATEADD(DAY, -4, GETDATE())),
(4, 3, 4, 'Great sneakers, very comfortable. Took off one star because the color was slightly different from the picture.', DATEADD(DAY, -2, GETDATE())),
(7, 5, 5, 'Must-read for every developer! Uncle Bob''s insights on clean code practices are invaluable.', DATEADD(DAY, -1, GETDATE())),
(10, 3, 4, 'Good quality yoga mat. Nice thickness and the alignment guides are helpful for beginners.', GETDATE());
GO

-- =====================================================
-- VERIFY DATA
-- =====================================================
SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM Users
UNION ALL
SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL
SELECT 'Products', COUNT(*) FROM Products
UNION ALL
SELECT 'Inventory', COUNT(*) FROM Inventory
UNION ALL
SELECT 'Orders', COUNT(*) FROM Orders
UNION ALL
SELECT 'OrderDetails', COUNT(*) FROM OrderDetails
UNION ALL
SELECT 'SupportTickets', COUNT(*) FROM SupportTickets
UNION ALL
SELECT 'Reviews', COUNT(*) FROM Reviews;
GO

PRINT 'E-Commerce Database created successfully!';
PRINT 'Demo Users:';
PRINT '  - admin (StoreOwner) - password: password123';
PRINT '  - john_doe (Customer) - password: password123';
PRINT '  - jane_smith (Customer) - password: password123';
PRINT '  - support1 (CustomerService) - password: password123';
PRINT '  - mary_jones (Customer) - password: password123';
GO
