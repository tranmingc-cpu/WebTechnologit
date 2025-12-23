
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

ALTER TABLE Products
ALTER COLUMN Discount DECIMAL(18,2);

INSERT INTO Products
(ProductName, CategoryId, BrandId, Price, Discount, Quantity, Description, ImageUrl, Status)
VALUES
-- LAPTOP
(N'Laptop Dell Inspiron 15', 1, 1, 22000000, 1000000, 15, N'Laptop văn phòng Dell', 'inspiron15.jpg', 'Available'),
(N'Laptop ASUS VivoBook 14', 1, 2, 19000000, 500000, 12, N'Laptop mỏng nhẹ', 'vivobook14.jpg', 'Available'),
(N'Laptop HP Envy 13', 1, 3, 26000000, 1500000, 7, N'Laptop cao cấp HP', 'envy13.jpg', 'Available'),
(N'Laptop MSI Katana GF66', 1, 5, 30000000, 2000000, 6, N'Laptop gaming MSI', 'katana.jpg', 'Available'),

-- PC
(N'PC Gaming RTX 3060', 2, 5, 25000000, 0, 5, N'PC gaming mạnh mẽ', 'pc_rtx3060.jpg', 'Available'),
(N'PC Văn Phòng i5', 2, 3, 15000000, 0, 10, N'PC cho văn phòng', 'pc_i5.jpg', 'Available'),
(N'PC Gaming RTX 4060', 2, 2, 32000000, 1000000, 4, N'PC gaming RTX 4060', 'pc_rtx4060.jpg', 'Available'),

-- CHUỘT
(N'Chuột Logitech G Pro', 3, 4, 2500000, 200000, 30, N'Chuột eSports', 'gpro.jpg', 'Available'),
(N'Chuột ASUS ROG Gladius', 3, 2, 1800000, 0, 20, N'Chuột gaming ASUS', 'gladius.jpg', 'Available'),
(N'Chuột MSI Clutch GM20', 3, 5, 900000, 0, 25, N'Chuột gaming MSI', 'gm20.jpg', 'Available'),

-- BÀN PHÍM
(N'Bàn phím cơ Logitech G Pro X', 4, 4, 3200000, 300000, 18, N'Bàn phím cơ cao cấp', 'gprox.jpg', 'Available'),
(N'Bàn phím ASUS ROG Strix', 4, 2, 2800000, 0, 15, N'Bàn phím RGB', 'rog_strix.jpg', 'Available'),
(N'Bàn phím Dell KB216', 4, 1, 450000, 0, 40, N'Bàn phím văn phòng', 'kb216.jpg', 'Available'),

-- LINH KIỆN
(N'RAM Corsair 16GB DDR4', 5, 5, 1800000, 0, 50, N'RAM gaming', 'ram16gb.jpg', 'Available'),
(N'SSD Samsung 970 EVO 1TB', 5, 3, 2900000, 200000, 35, N'SSD NVMe tốc độ cao', '970evo.jpg', 'Available'),
(N'CPU Intel Core i7-12700K', 5, 1, 9500000, 0, 8, N'CPU Intel thế hệ 12', 'i7_12700k.jpg', 'Available'),
(N'VGA RTX 3060 MSI', 5, 5, 12000000, 500000, 6, N'Card đồ họa RTX 3060', 'rtx3060.jpg', 'Available'),

-- THÊM MỘT SỐ SẢN PHẨM KHÁC
(N'Laptop HP Pavilion 14', 1, 3, 21000000, 1000000, 9, N'Laptop phổ thông', 'pavilion14.jpg', 'Available'),
(N'PC Mini ASUS PN', 2, 2, 13000000, 0, 7, N'PC mini gọn nhẹ', 'asus_pn.jpg', 'Available'),
(N'Chuột Logitech MX Master 3', 3, 4, 2300000, 0, 22, N'Chuột văn phòng cao cấp', 'mxmaster3.jpg', 'Available');


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
