# E-Commerce WPF Application

A complete E-Commerce platform built with WPF, MVVM architecture, and Entity Framework 6 Database-First approach.

## Table of Contents

- [Features](#features)
- [Technologies](#technologies)
- [Project Structure](#project-structure)
- [Setup Instructions](#setup-instructions)
- [Demo Users](#demo-users)
- [Architecture](#architecture)
- [Database Schema](#database-schema)

## Features

### Store Owner
- **Product Management**: Add, edit, delete, and search products
- **Inventory Management**: Update stock levels, view low stock alerts
- **Order Management**: View and update order statuses

### Customer
- **Product Browsing**: Browse products, filter by category, search
- **Shopping Cart**: Add/remove items, update quantities
- **Order Placement**: Place orders with shipping address
- **Order History**: View past orders and their status

### Customer Service
- **Ticket Management**: View all support tickets
- **Ticket Assignment**: Assign tickets to self
- **Ticket Resolution**: Mark tickets as resolved or closed
- **Filtering**: Filter by status, view unassigned tickets

## Technologies

- **.NET Framework 4.7.2**
- **WPF (Windows Presentation Foundation)**
- **MVVM Architecture** (Model-View-ViewModel)
- **Entity Framework 6.4.4** (Database-First)
- **SQL Server**
- **C# 7.3**

## Project Structure

```
ECommerceApp/
├── Commands/                    # ICommand implementations
│   ├── RelayCommand.cs
│   └── AsyncRelayCommand.cs
│
├── Converters/                  # XAML Value Converters
│   └── BooleanToVisibilityConverter.cs
│
├── Helpers/                     # Utility classes
│   ├── PasswordHelper.cs
│   └── ValidationHelper.cs
│
├── Models/                      # Entity Framework models
│   ├── User.cs
│   ├── Product.cs
│   ├── Category.cs
│   ├── Order.cs
│   ├── OrderDetail.cs
│   ├── Inventory.cs
│   ├── SupportTicket.cs
│   ├── Review.cs
│   ├── CartItem.cs
│   └── ECommerceEntities.cs     # DbContext
│
├── Services/                    # Repository pattern services
│   ├── IUserService.cs / UserService.cs
│   ├── IProductService.cs / ProductService.cs
│   ├── IInventoryService.cs / InventoryService.cs
│   ├── IOrderService.cs / OrderService.cs
│   └── ISupportService.cs / SupportService.cs
│
├── Stores/                      # State management
│   ├── CurrentUserStore.cs
│   └── NavigationStore.cs
│
├── ViewModels/                  # MVVM ViewModels
│   ├── ViewModelBase.cs
│   ├── MainViewModel.cs
│   ├── LoginViewModel.cs
│   ├── StoreOwnerDashboardViewModel.cs
│   ├── ProductManagementViewModel.cs
│   ├── InventoryManagementViewModel.cs
│   ├── OrderManagementViewModel.cs
│   ├── CustomerShopViewModel.cs
│   ├── ShoppingCartViewModel.cs
│   ├── OrderHistoryViewModel.cs
│   └── CustomerServiceViewModel.cs
│
├── Views/                       # XAML Views
│   ├── LoginView.xaml
│   ├── StoreOwnerDashboardView.xaml
│   ├── ProductManagementView.xaml
│   ├── InventoryManagementView.xaml
│   ├── OrderManagementView.xaml
│   ├── CustomerShopView.xaml
│   ├── ShoppingCartView.xaml
│   ├── OrderHistoryView.xaml
│   └── CustomerServiceView.xaml
│
├── App.xaml / App.xaml.cs
├── MainWindow.xaml / MainWindow.xaml.cs
├── App.config
├── ECommerceDB_Script.sql       # Database creation script
└── README.md
```

## Setup Instructions

### Prerequisites

1. **Visual Studio 2019 or later** (with .NET desktop development workload)
2. **SQL Server** (LocalDB, Express, or full version)
3. **.NET Framework 4.7.2 SDK**

### Database Setup

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to your SQL Server instance
3. Open the file `ECommerceDB_Script.sql`
4. Execute the script to create the database and sample data

```sql
-- The script will:
-- 1. Create the ECommerceDB database
-- 2. Create all 8 tables with proper relationships
-- 3. Insert sample data including demo users
```

### Project Setup

1. **Open the Solution**
   - Open `ECommerceApp.csproj` in Visual Studio

2. **Restore NuGet Packages**
   - Right-click solution → "Restore NuGet Packages"
   - Or: `Install-Package EntityFramework -Version 6.4.4`

3. **Update Connection String** (if needed)
   - Open `App.config`
   - Update the `ECommerceEntities` connection string to match your SQL Server:
   ```xml
   <add name="ECommerceEntities"
        connectionString="data source=YOUR_SERVER;initial catalog=ECommerceDB;integrated security=True;..."
        providerName="System.Data.SqlClient" />
   ```

4. **Build and Run**
   - Press F5 or click "Start Debugging"

### Entity Framework Setup (Database-First)

If you need to regenerate the Entity Framework model:

1. Right-click Models folder → Add → New Item
2. Select "ADO.NET Entity Data Model"
3. Name it `ECommerceDataModel.edmx`
4. Choose "EF Designer from database"
5. Connect to your ECommerceDB database
6. Select all tables
7. Generate

## Demo Users

All demo users have the password: `password123`

| Username | Role | Description |
|----------|------|-------------|
| `admin` | Store Owner | Full access to product and inventory management |
| `john_doe` | Customer | Can browse products, place orders |
| `jane_smith` | Customer | Can browse products, place orders |
| `support1` | Customer Service | Can manage support tickets |
| `mary_jones` | Customer | Can browse products, place orders |

## Architecture

### MVVM Pattern

The application follows the **Model-View-ViewModel (MVVM)** architectural pattern:

- **Model**: Entity Framework entities and business logic
- **View**: XAML files with no code-behind logic (except DataContext and PasswordBox binding)
- **ViewModel**: Handles UI logic, commands, and data binding

### Key MVVM Components

#### ViewModelBase
- Implements `INotifyPropertyChanged`
- Provides `SetProperty<T>()` helper method
- Base class for all ViewModels

#### RelayCommand
- Implements `ICommand`
- Supports `Execute` and `CanExecute`
- Wired to `CommandManager.RequerySuggested`

#### NavigationStore
- Manages current ViewModel for view switching
- Singleton pattern for application-wide state

#### CurrentUserStore
- Stores authenticated user information
- Singleton pattern for user state management

### Data Access Layer

Uses the **Repository Pattern** with service classes:
- Each entity has an interface (e.g., `IProductService`) and implementation (e.g., `ProductService`)
- Services encapsulate Entity Framework operations
- Supports LINQ queries with eager loading (`Include()`)

## Database Schema

### Tables

1. **Users** - User accounts (Store Owner, Customer, Customer Service)
2. **Categories** - Product categories
3. **Products** - Product listings
4. **Inventory** - Stock levels per product
5. **Orders** - Customer orders
6. **OrderDetails** - Order line items
7. **SupportTickets** - Customer support tickets
8. **Reviews** - Product reviews

### Relationships

```
Users 1 ────── N Products (StoreOwnerID)
Users 1 ────── N Orders (CustomerID)
Users 1 ────── N SupportTickets (CustomerID, AssignedToID)
Users 1 ────── N Reviews (CustomerID)

Categories 1 ── N Products (CategoryID)
Products 1 ──── 1 Inventory (ProductID)
Products 1 ──── N OrderDetails (ProductID)
Products 1 ──── N Reviews (ProductID)

Orders 1 ────── N OrderDetails (OrderID)
```

## Security

- **Password Hashing**: SHA256 hash for password storage
- **Role-Based Access**: UI elements and navigation based on user role
- **Input Validation**: Server-side validation for all user inputs

## Code Quality

- Comments for complex logic
- Consistent naming conventions
- Proper error handling (try-catch)
- IDisposable implementation for resource cleanup
- Separation of concerns (MVVM, Repository Pattern)

## Troubleshooting

### Connection Issues
- Verify SQL Server is running
- Check connection string in `App.config`
- Test connection in SSMS

### NuGet Package Issues
- Clean solution and rebuild
- Delete `packages` folder and restore

### Entity Framework Issues
- Regenerate `.edmx` file
- Verify all tables exist in database
- Check navigation properties

## License

This project is for educational purposes.

## Author

E-Commerce WPF Application - MVVM Architecture Demo
