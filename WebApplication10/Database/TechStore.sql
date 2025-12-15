
CREATE DATABASE TechStoreDB;
GO
USE TechStoreDB;
GO

CREATE TABLE Users (
	UserId INT IDENTITY PRIMARY KEY,
	Username NVARCHAR(50) NOT NULL UNIQUE,
	Password NVARCHAR(100) NOT NULL,
	FullName NVARCHAR(100),
	Email NVARCHAR(100) UNIQUE,
	Phone NVARCHAR(20),
	Address NVARCHAR(255),
	Role NVARCHAR(20) DEFAULT 'Customer',
	IsActive BIT DEFAULT 1,
	CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Categories (
	CategoryId INT IDENTITY PRIMARY KEY,
	CategoryName NVARCHAR(100) NOT NULL,
	Description NVARCHAR(255),
	CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Brands (
	BrandId INT IDENTITY PRIMARY KEY,
	BrandName NVARCHAR(100) NOT NULL,
	Country NVARCHAR(100)
);

CREATE TABLE Products (
	ProductId INT IDENTITY PRIMARY KEY,
	ProductName NVARCHAR(200) NOT NULL,
	CategoryId INT NOT NULL,
	BrandId INT,
	Price DECIMAL(18,2) NOT NULL,
	Discount DECIMAL(5,2) DEFAULT 0,
	Quantity INT DEFAULT 0,
	Description NVARCHAR(MAX),
	ImageUrl NVARCHAR(255),
	Status NVARCHAR(20) DEFAULT 'Available',
	CreatedAt DATETIME DEFAULT GETDATE(),


	CONSTRAINT FK_Product_Category FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
	CONSTRAINT FK_Product_Brand FOREIGN KEY (BrandId) REFERENCES Brands(BrandId)
);

CREATE TABLE ProductImages (
	ImageId INT IDENTITY PRIMARY KEY,
	ProductId INT NOT NULL,
	ImageUrl NVARCHAR(255) NOT NULL,


	CONSTRAINT FK_Image_Product FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

CREATE TABLE Orders (
	OrderId INT IDENTITY PRIMARY KEY,
	UserId INT NOT NULL,
	OrderDate DATETIME DEFAULT GETDATE(),
	TotalAmount DECIMAL(18,2),
	Status NVARCHAR(30) DEFAULT 'Pending', -- Pending, Paid, Shipping, Completed, Cancelled
	ShippingAddress NVARCHAR(255),


	CONSTRAINT FK_Order_User FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE OrderDetails (
	OrderDetailId INT IDENTITY PRIMARY KEY,
	OrderId INT NOT NULL,
	ProductId INT NOT NULL,
	Quantity INT NOT NULL,
	UnitPrice DECIMAL(18,2) NOT NULL,


	CONSTRAINT FK_OrderDetail_Order FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
	CONSTRAINT FK_OrderDetail_Product FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

CREATE TABLE Cart (
	CartId INT IDENTITY PRIMARY KEY,
	UserId INT NOT NULL,
	ProductId INT NOT NULL,
	Quantity INT NOT NULL,


	CONSTRAINT FK_Cart_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
	CONSTRAINT FK_Cart_Product FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

CREATE TABLE Reviews (
	ReviewId INT IDENTITY PRIMARY KEY,
	ProductId INT NOT NULL,
	UserId INT NOT NULL,
	Rating INT CHECK (Rating BETWEEN 1 AND 5),
	Comment NVARCHAR(500),
	CreatedAt DATETIME DEFAULT GETDATE(),


	CONSTRAINT FK_Review_Product FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
	CONSTRAINT FK_Review_User FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

INSERT INTO Users (Username, Password, FullName, Email, Phone, Address, Role)
VALUES
('admin', '123', N'Quản trị viên', 'admin@techstore.com', '0900000001', N'Hà Nội', 'Admin'),
('user1', '123', N'Nguyễn Văn A', 'a@gmail.com', '0900000002', N'TP HCM', 'Customer'),
('user2', '123', N'Trần Thị B', 'b@gmail.com', '0900000003', N'Đà Nẵng', 'Customer'),
('user3', '123', N'Lê Văn C', 'c@gmail.com', '0900000004', N'Cần Thơ', 'Customer'),
('user4', '123', N'Phạm Thị D', 'd@gmail.com', '0900000005', N'Hải Phòng', 'Customer');

INSERT INTO Categories (CategoryName, Description)
VALUES
(N'Laptop', N'Các dòng laptop học tập, gaming'),
(N'PC', N'Máy tính để bàn'),
(N'Chuột', N'Chuột máy tính'),
(N'Bàn phím', N'Bàn phím cơ, văn phòng'),
(N'Linh kiện', N'CPU, RAM, VGA');

INSERT INTO Brands (BrandName, Country)
VALUES
(N'Dell', 'USA'),
(N'ASUS', 'Taiwan'),
(N'HP', 'USA'),
(N'Logitech', 'Switzerland'),
(N'MSI', 'Taiwan');

INSERT INTO Products
(ProductName, CategoryId, BrandId, Price, Quantity, Description, ImageUrl)
VALUES
(N'Laptop Dell XPS 13', 1, 1, 35000000, 10, N'Laptop cao cấp', 'xps13.jpg'),
(N'Laptop ASUS TUF Gaming', 1, 2, 28000000, 8, N'Laptop gaming', 'tuf.jpg'),
(N'Chuột Logitech G102', 3, 4, 450000, 50, N'Chuột gaming', 'g102.jpg'),
(N'Bàn phím cơ MSI GK50', 4, 5, 1500000, 20, N'Bàn phím RGB', 'gk50.jpg'),
(N'PC HP Pavilion', 2, 3, 18000000, 5, N'Máy tính để bàn', 'pavilion.jpg');


INSERT INTO ProductImages (ProductId, ImageUrl)
VALUES
(1, 'xps13_1.jpg'),
(2, 'tuf_1.jpg'),
(3, 'g102_1.jpg'),
(4, 'gk50_1.jpg'),
(5, 'pavilion_1.jpg');

INSERT INTO Orders (UserId, TotalAmount, Status, ShippingAddress)
VALUES
(2, 35000000, 'Paid', N'TP HCM'),
(3, 450000, 'Completed', N'Đà Nẵng'),
(4, 1500000, 'Shipping', N'Cần Thơ'),
(5, 28000000, 'Pending', N'Hải Phòng'),
(2, 18000000, 'Paid', N'TP HCM');

INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice)
VALUES
(1, 1, 1, 35000000),
(2, 3, 1, 450000),
(3, 4, 1, 1500000),
(4, 2, 1, 28000000),
(5, 5, 1, 18000000);

INSERT INTO Cart (UserId, ProductId, Quantity)
VALUES
(2, 3, 2),
(3, 1, 1),
(4, 4, 1),
(5, 2, 1),
(2, 5, 1);

INSERT INTO Cart (UserId, ProductId, Quantity)
VALUES
(2, 3, 2),
(3, 1, 1),
(4, 4, 1),
(5, 2, 1),
(2, 5, 1);

INSERT INTO Reviews (ProductId, UserId, Rating, Comment)
VALUES
(1, 2, 5, N'Laptop rất tốt'),
(2, 5, 4, N'Chơi game ổn'),
(3, 3, 5, N'Chuột nhạy'),
(4, 4, 4, N'Bàn phím đẹp'),
(5, 2, 4, N'PC chạy mượt');

SELECT * FROM Users;
SELECT * FROM Products;
SELECT * FROM Orders;
SELECT name FROM sys.databases;
