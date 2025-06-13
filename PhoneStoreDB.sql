-- Xóa cơ sở dữ liệu nếu tồn tại
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'PhoneStoreDB')
BEGIN
    DROP DATABASE PhoneStoreDB;
END
GO

-- Tạo cơ sở dữ liệu
CREATE DATABASE PhoneStoreDB;
GO

USE PhoneStoreDB;
GO

-- Tạo bảng Users
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
    Fullname NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20) NOT NULL,
    Gender TINYINT,
    BirthDate DATE,
    IsAgree BIT NOT NULL DEFAULT 0,
    Photo NVARCHAR(50),
    Activated BIT DEFAULT 1,
    Admin BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);

-- Thêm dữ liệu mẫu cho bảng Users
INSERT INTO Users (Id, Fullname, Email, PasswordHash, PhoneNumber, Gender, BirthDate, IsAgree, Photo, Activated, Admin, CreatedAt, UpdatedAt) VALUES
(NEWID(), N'Nguyễn Văn A', 'nva@gmail.com', 'hashed_pass123', '0901234567', 0, '1990-05-15', 1, 'photo1.jpg', 1, 0, '2025-06-11 14:00:00', '2025-06-11 14:00:00'),
(NEWID(), N'Trần Thị B', 'ttb@gmail.com', 'hashed_pass456', '0912345678', 1, '1995-08-20', 1, 'photo2.jpg', 1, 0, '2025-06-11 14:05:00', '2025-06-11 14:05:00'),
(NEWID(), N'Lê Văn C', 'lvc@gmail.com', 'hashed_pass789', '0923456789', 2, '1988-03-10', 1, 'photo3.jpg', 1, 1, '2025-06-11 14:10:00', '2025-06-11 14:10:00'),
(NEWID(), N'Phạm Thị D', 'ptd@yahoo.com', 'hashed_pass101', '0934567890', 1, '1992-11-25', 1, 'photo4.jpg', 1, 0, '2025-06-11 14:15:00', '2025-06-11 14:15:00'),
(NEWID(), N'Hoàng Văn E', 'hve@hotmail.com', 'hashed_pass202', '0945678901', 0, '1985-07-30', 1, 'photo5.jpg', 1, 0, '2025-06-11 14:20:00', '2025-06-11 14:20:00');

-- Tạo bảng Categories
CREATE TABLE Categories (
    Id INT PRIMARY KEY,
    Name NVARCHAR(50),
    NameVN NVARCHAR(MAX)
);

INSERT INTO Categories (Id, Name, NameVN) VALUES
(1, 'Smartphones', N'Điện thoại'),
(2, 'Phone Accessories', N'Phụ kiện điện thoại');

-- Tạo bảng Products
CREATE TABLE Products (
    Id INT PRIMARY KEY,
    Name NVARCHAR(50),
    UnitPrice FLOAT,
    Image NVARCHAR(50),
    AvailableDate DATE,
    Available BIT,
    CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
    Quantity INT,
    Description NVARCHAR(MAX),
    ViewCount FLOAT,
    Special BIT
);

INSERT INTO Products (Id, Name, UnitPrice, Image, AvailableDate, Available, CategoryId, Quantity, Description, ViewCount, Special) VALUES
(1, 'iPhone 14', 20000000, 'iphone14.jpg', '2023-09-01', 1, 1, 50, N'Điện thoại cao cấp từ Apple', 1500.5, 1),
(2, 'Samsung Galaxy S23', 18000000, 's23.jpg', '2023-10-15', 1, 1, 30, N'Điện thoại mạnh mẽ từ Samsung', 1200.3, 0),
(3, 'Google Pixel 7', 16000000, 'pixel7.jpg', '2023-10-01', 1, 1, 40, N'Điện thoại camera đỉnh cao', 950.4, 0),
(4, 'iPhone 13', 17000000, 'iphone13.jpg', '2022-09-01', 1, 1, 25, N'Phiên bản cũ nhưng chất lượng', 1300.6, 0),
(5, 'Xiaomi 13', 14000000, 'xiaomi13.jpg', '2023-11-10', 1, 1, 35, N'Điện thoại giá rẻ chất lượng', 1100.0, 0),
(6, 'AirPods Pro', 5000000, 'airpods.jpg', '2023-11-01', 1, 2, 100, N'Tai nghe không dây chất lượng cao', 800.7, 0),
(7, 'Wireless Charger', 1500000, 'charger.jpg', '2023-06-01', 1, 2, 200, N'Sạc không dây tiện lợi', 400.2, 0),
(8, 'Power Bank 20000mAh', 1200000, 'powerbank.jpg', '2023-04-15', 1, 2, 150, N'Sạc dự phòng dung lượng lớn', 300.8, 0),
(9, 'Phone Case iPhone 14', 500000, 'caseiphone14.jpg', '2023-09-10', 1, 2, 300, N'Ốp lưng chính hãng cho iPhone 14', 250.5, 0),
(10, 'Screen Protector', 300000, 'screenprotector.jpg', '2023-05-01', 1, 2, 250, N'Miếng dán màn hình chống xước', 200.3, 0);

-- Tạo bảng Orders
CREATE TABLE Orders (
    Id INT PRIMARY KEY,
    CustomerId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Users(Id),
    OrderDate DATETIME,
    Address NVARCHAR(60),
    Amount FLOAT,
    Description NVARCHAR(1000)
);

-- Thêm dữ liệu mẫu cho bảng Orders
DECLARE @User1 UNIQUEIDENTIFIER = (SELECT Id FROM Users WHERE Email = 'nva@gmail.com');
DECLARE @User2 UNIQUEIDENTIFIER = (SELECT Id FROM Users WHERE Email = 'ttb@gmail.com');
DECLARE @User3 UNIQUEIDENTIFIER = (SELECT Id FROM Users WHERE Email = 'lvc@gmail.com');
DECLARE @User4 UNIQUEIDENTIFIER = (SELECT Id FROM Users WHERE Email = 'ptd@yahoo.com');
DECLARE @User5 UNIQUEIDENTIFIER = (SELECT Id FROM Users WHERE Email = 'hve@hotmail.com');

INSERT INTO Orders (Id, CustomerId, OrderDate, Address, Amount, Description) VALUES
(1, @User1, '2025-06-11 14:30:00', N'123 Đường A, TP.HCM', 20000000, N'Đơn hàng iPhone 14'),
(2, @User2, '2025-06-11 14:35:00', N'456 Đường B, Hà Nội', 20500000, N'Đơn hàng iPhone 14 + Wireless Charger'),
(3, @User3, '2025-06-11 14:40:00', N'789 Đường C, Đà Nẵng', 18000000, N'Đơn hàng Samsung Galaxy S23'),
(4, @User4, '2025-06-11 14:45:00', N'101 Đường D, Cần Thơ', 5200000, N'Đơn hàng AirPods Pro'),
(5, @User5, '2025-06-11 14:50:00', N'202 Đường E, Hải Phòng', 1500000, N'Đơn hàng Wireless Charger'),
(6, @User1, '2025-06-11 15:00:00', N'123 Đường A, TP.HCM', 14000000, N'Đơn hàng Xiaomi 13'),
(7, @User2, '2025-06-11 15:05:00', N'456 Đường B, Hà Nội', 1700000, N'Đơn hàng Phone Case iPhone 14');

-- Tạo bảng OrderDetails
CREATE TABLE OrderDetails (
    Id INT PRIMARY KEY,
    OrderId INT FOREIGN KEY REFERENCES Orders(Id),
    ProductId INT FOREIGN KEY REFERENCES Products(Id),
    UnitPrice FLOAT,
    Quantity INT,
    Discount FLOAT
);

-- Thêm dữ liệu mẫu cho bảng OrderDetails
INSERT INTO OrderDetails (Id, OrderId, ProductId, UnitPrice, Quantity, Discount) VALUES
(1, 1, 1, 20000000, 1, 0.0),
(2, 2, 1, 20000000, 1, 0.0),
(3, 2, 7, 1500000, 1, 0.1),
(4, 3, 2, 18000000, 1, 0.05),
(5, 4, 6, 5000000, 1, 0.0),
(6, 5, 7, 1500000, 1, 0.0),
(7, 6, 5, 14000000, 1, 0.0),
(8, 7, 9, 500000, 2, 0.0);