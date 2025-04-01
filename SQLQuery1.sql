CREATE DATABASE OfficeFurnitureStore;
GO
USE OfficeFurnitureStore;
GO

-- Bảng quản trị viên
CREATE TABLE Admin (
    AdminID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Phone NVARCHAR(20),
);

-- Bảng khách hàng
CREATE TABLE Customer (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Phone NVARCHAR(20),
    Address NVARCHAR(255),
    Avatar NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Bảng danh mục sản phẩm
CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL
);

-- Bảng sản phẩm
CREATE TABLE Product (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Price DECIMAL(18,2) NOT NULL,
    StockQuantity INT NOT NULL,
    OrderedQuantity INT DEFAULT 0,
    CategoryID INT FOREIGN KEY REFERENCES Category(CategoryID) ON DELETE CASCADE,
    Status NVARCHAR(50) NOT NULL,
    Image NVARCHAR(255)
);

-- Bảng giỏ hàng
CREATE TABLE Cart (
    CartID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT FOREIGN KEY REFERENCES Customer(CustomerID) ON DELETE CASCADE,
    ProductID INT FOREIGN KEY REFERENCES Product(ProductID) ON DELETE CASCADE,
    Quantity INT NOT NULL
);

-- Bảng đơn đặt hàng
CREATE TABLE [Order] (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT FOREIGN KEY REFERENCES Customer(CustomerID) ON DELETE CASCADE,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Pending'
);

-- Bảng chi tiết đơn hàng
CREATE TABLE OrderDetail (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT FOREIGN KEY REFERENCES [Order](OrderID) ON DELETE CASCADE,
    ProductID INT FOREIGN KEY REFERENCES Product(ProductID) ON DELETE CASCADE,
    Quantity INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL
);

-- Bảng bình luận
CREATE TABLE Comment (
    CommentID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT FOREIGN KEY REFERENCES Customer(CustomerID) ON DELETE CASCADE,
    ProductID INT FOREIGN KEY REFERENCES Product(ProductID) ON DELETE CASCADE,
    Content NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Bảng thống kê doanh thu
CREATE TABLE Statistic (
    StatisticID INT IDENTITY(1,1) PRIMARY KEY,
    ReportDate DATE NOT NULL,  -- Ngày thống kê
    Period NVARCHAR(50),  -- Thời kỳ thống kê (Ngày, Tuần, Tháng, Năm)
    PeriodStartDate DATE,  -- Ngày bắt đầu của kỳ thống kê (nếu là tuần hoặc tháng)
    PeriodEndDate DATE,    -- Ngày kết thúc của kỳ thống kê (nếu là tuần hoặc tháng)
    TotalOrders INT NOT NULL,  -- Tổng số đơn hàng
    TotalRevenue DECIMAL(18,2) NOT NULL,  -- Tổng doanh thu
    TotalProductsSold INT,  -- Tổng số sản phẩm đã bán
    TotalCustomers INT,     -- Tổng số khách hàng
    TotalProductsInStock INT,  -- Tổng số sản phẩm còn trong kho
    CategoryRevenue DECIMAL(18,2),  -- Doanh thu theo từng danh mục (nếu cần)
    ProductRevenue DECIMAL(18,2),   -- Doanh thu theo sản phẩm (nếu cần)
    CustomerRevenue DECIMAL(18,2)   -- Doanh thu theo khách hàng (nếu cần)
);


-- Bảng thanh toán
CREATE TABLE Payment (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT FOREIGN KEY REFERENCES [Order](OrderID) ON DELETE CASCADE,
    PaymentMethod NVARCHAR(50) NOT NULL,
    PaymentStatus NVARCHAR(50) DEFAULT 'Pending',
    TransactionDate DATETIME DEFAULT GETDATE()
);

-- Bảng đối tác
CREATE TABLE Partner (
    PartnerID INT IDENTITY(1,1) PRIMARY KEY,
    PartnerName NVARCHAR(255) NOT NULL,
    ContactPerson NVARCHAR(100),
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Phone NVARCHAR(20),
    Address NVARCHAR(255)
);

-- Bảng liên hệ
CREATE TABLE Contact (
    ContactID INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Subject NVARCHAR(255) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
