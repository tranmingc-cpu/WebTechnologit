
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

ALTER TABLE Categories
ALTER COLUMN Description NVARCHAR(MAX);

UPDATE Categories
SET Description = N'
<h2>Laptop là gì?</h2>
<p>
<strong>Laptop</strong> là dòng máy tính xách tay được thiết kế nhỏ gọn,
phù hợp cho học tập, làm việc và giải trí.
</p>

<p>
Laptop hiện đại được trang bị CPU mạnh mẽ, ổ cứng SSD tốc độ cao
và màn hình có độ phân giải cao, đáp ứng tốt các nhu cầu từ cơ bản
đến nâng cao.
</p>

<h3>Phân loại laptop phổ biến</h3>
<ul>
  <li>Laptop văn phòng – tiết kiệm pin</li>
  <li>Laptop gaming – hiệu năng cao</li>
  <li>Laptop đồ họa – màn hình chuẩn màu</li>
</ul>

<figure class="image">
  <img src="/uploads/categories/laptop.jpg" />
  <figcaption>Hình ảnh laptop hiện đại</figcaption>
</figure>

<p>
Tại <strong>TechStore</strong>, chúng tôi cung cấp đa dạng các dòng laptop
chính hãng từ Dell, ASUS, HP, MSI với mức giá cạnh tranh.
</p>
'
WHERE CategoryId = 1;

UPDATE Categories
SET Description = N'
<h2>PC – Máy tính để bàn là gì?</h2>
<p>
<strong>PC (Personal Computer)</strong> hay còn gọi là
<strong>máy tính để bàn</strong> là thiết bị có hiệu năng cao,
phù hợp cho làm việc, học tập và giải trí chuyên sâu.
</p>

<p>
So với laptop, PC có ưu thế về khả năng nâng cấp linh kiện,
tản nhiệt tốt và hiệu suất ổn định khi hoạt động trong thời gian dài.
</p>

<h3>Phân loại PC phổ biến</h3>
<ul>
  <li>PC văn phòng – ổn định, tiết kiệm chi phí</li>
  <li>PC gaming – cấu hình mạnh, xử lý đồ họa cao</li>
  <li>PC đồ họa – dựng phim, thiết kế 3D</li>
</ul>

<p>
Người dùng có thể tùy chọn cấu hình CPU, RAM, SSD, VGA
phù hợp với nhu cầu sử dụng và ngân sách.
</p>

<figure class="image">
  <img src="/uploads/categories/pc.jpg" />
  <figcaption>Máy tính để bàn hiệu năng cao</figcaption>
</figure>

<p>
Tại <strong>TechStore</strong>, chúng tôi cung cấp các dòng PC
chính hãng, lắp ráp sẵn và build theo yêu cầu với bảo hành uy tín.
</p>
'
WHERE CategoryId = 2;

UPDATE Categories
SET Description = N'
<h2>Chuột máy tính là gì?</h2>
<p>
<strong>Chuột máy tính</strong> là thiết bị ngoại vi quan trọng,
giúp người dùng điều khiển và thao tác chính xác trên máy tính.
</p>

<p>
Hiện nay, chuột được thiết kế đa dạng về kiểu dáng,
trọng lượng và cảm biến, phù hợp cho từng đối tượng sử dụng.
</p>

<h3>Các loại chuột phổ biến</h3>
<ul>
  <li>Chuột văn phòng – gọn nhẹ, sử dụng lâu không mỏi tay</li>
  <li>Chuột gaming – DPI cao, cảm biến chính xác</li>
  <li>Chuột không dây – tiện lợi, thẩm mỹ</li>
</ul>

<p>
Chuột gaming thường được trang bị nút macro,
RGB và thiết kế công thái học cho game thủ chuyên nghiệp.
</p>

<figure class="image">
  <img src="/uploads/categories/mouse.jpg" />
  <figcaption>Chuột máy tính hiện đại</figcaption>
</figure>

<p>
TechStore phân phối chuột chính hãng từ Logitech, ASUS, MSI
với nhiều mức giá và chế độ bảo hành rõ ràng.
</p>
'
WHERE CategoryId = 3;

UPDATE Categories
SET Description = N'
<h2>Bàn phím máy tính</h2>
<p>
<strong>Bàn phím</strong> là thiết bị nhập liệu không thể thiếu,
giúp người dùng thao tác và nhập dữ liệu nhanh chóng.
</p>

<p>
Ngày nay, bàn phím không chỉ phục vụ công việc
mà còn đóng vai trò quan trọng trong trải nghiệm chơi game.
</p>

<h3>Các loại bàn phím phổ biến</h3>
<ul>
  <li>Bàn phím cơ – độ bền cao, cảm giác gõ tốt</li>
  <li>Bàn phím văn phòng – gõ êm, giá thành hợp lý</li>
  <li>Bàn phím gaming – RGB, switch chất lượng cao</li>
</ul>

<p>
Bàn phím cơ sử dụng switch cơ học với tuổi thọ
lên đến hàng chục triệu lần nhấn.
</p>

<figure class="image">
  <img src="/uploads/categories/keyboard.jpg" />
  <figcaption>Bàn phím cơ hiện đại</figcaption>
</figure>

<p>
TechStore cung cấp đa dạng các mẫu bàn phím từ Logitech,
ASUS, MSI, Dell đáp ứng mọi nhu cầu sử dụng.
</p>
'
WHERE CategoryId = 4;

UPDATE Categories
SET Description = N'
<h2>Linh kiện máy tính</h2>
<p>
<strong>Linh kiện máy tính</strong> là các thành phần quan trọng
quyết định hiệu năng và độ ổn định của hệ thống.
</p>

<p>
Việc lựa chọn linh kiện phù hợp giúp máy tính hoạt động
mượt mà, bền bỉ và đáp ứng đúng nhu cầu sử dụng.
</p>

<h3>Các loại linh kiện phổ biến</h3>
<ul>
  <li>CPU – bộ não xử lý của máy tính</li>
  <li>RAM – bộ nhớ tạm thời</li>
  <li>Ổ cứng SSD/HDD – lưu trữ dữ liệu</li>
  <li>Card đồ họa (VGA) – xử lý hình ảnh</li>
</ul>

<p>
Linh kiện chính hãng đảm bảo hiệu năng cao,
độ bền lâu dài và khả năng tương thích tốt.
</p>

<figure class="image">
  <img src="/uploads/categories/components.jpg" />
  <figcaption>Linh kiện máy tính chính hãng</figcaption>
</figure>

<p>
TechStore cam kết cung cấp linh kiện chính hãng từ Intel,
AMD, Samsung, Corsair, MSI với giá cả cạnh tranh.
</p>
'
WHERE CategoryId = 5;

CREATE TABLE AboutPage (
    AboutId INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);

INSERT INTO AboutPage (Title, Description)
VALUES
(N'Giới thiệu TechStore', 
N'
<p>Chào mừng đến với <strong>TechStore</strong> – hệ thống bán lẻ thiết bị công nghệ chính hãng hàng đầu Việt Nam. Với hơn 10 năm kinh nghiệm trong lĩnh vực công nghệ, TechStore đã trở thành địa chỉ tin cậy cho những người yêu công nghệ và muốn sở hữu thiết bị chất lượng cao.</p>

<h2>Lịch sử hình thành</h2>
<p>TechStore được thành lập vào năm 2013 với mục tiêu cung cấp sản phẩm công nghệ chính hãng, giá cả minh bạch và dịch vụ chăm sóc khách hàng tận tâm. Từ một cửa hàng nhỏ, TechStore đã mở rộng ra nhiều thành phố lớn và phát triển mạnh mảng bán hàng trực tuyến, phục vụ hàng trăm nghìn khách hàng trên toàn quốc.</p>

<h3>Hình ảnh cửa hàng chính</h3>
<p><img src="/uploads/about/store_front.jpg" alt="Cửa hàng TechStore" style="max-width:100%;height:auto;" /></p>

<h2>Sứ mệnh & Tầm nhìn</h2>
<p><strong>Sứ mệnh:</strong> Mang đến trải nghiệm mua sắm thiết bị công nghệ tiện lợi, nhanh chóng và đáng tin cậy.</p>
<p><strong>Tầm nhìn:</strong> Trở thành nhà bán lẻ thiết bị công nghệ hàng đầu tại Việt Nam, đồng hành cùng sự phát triển công nghệ của người dùng và doanh nghiệp.</p>

<h2>Giá trị cốt lõi</h2>
<ul>
    <li><strong>Chất lượng:</strong> Tất cả sản phẩm đều chính hãng, kiểm tra kỹ lưỡng trước khi đến tay khách hàng.</li>
    <li><strong>Tin cậy:</strong> Giá cả minh bạch, bảo hành chính hãng, đổi trả linh hoạt.</li>
    <li><strong>Nhanh chóng:</strong> Hệ thống giao hàng và chăm sóc khách hàng hiệu quả, hỗ trợ 24/7.</li>
    <li><strong>Đổi mới:</strong> Luôn cập nhật sản phẩm công nghệ mới nhất và xu hướng thị trường.</li>
</ul>

<h2>Dịch vụ nổi bật</h2>
<ul>
    <li><strong>Mua sắm trực tuyến:</strong> Giao diện thân thiện, tìm kiếm sản phẩm dễ dàng theo tên, hãng hoặc loại.</li>
    <li><strong>Giao hàng toàn quốc:</strong> Giao hàng nhanh trong 2h tại TP.HCM và trong 24-48h tại các tỉnh khác.</li>
    <li><strong>Hỗ trợ kỹ thuật 24/7:</strong> Tư vấn, cài đặt, sửa chữa thiết bị khi khách hàng cần.</li>
    <li><strong>Bảo hành & Đổi trả:</strong> Chính sách minh bạch, đổi trả trong 7 ngày, bảo hành chính hãng đầy đủ.</li>
    <li><strong>Tin tức & Khuyến mãi:</strong> Cập nhật các sản phẩm công nghệ mới nhất, chương trình ưu đãi hấp dẫn.</li>
</ul>

<h2>Danh mục sản phẩm</h2>
<p>Tại TechStore, chúng tôi cung cấp các sản phẩm chính hãng thuộc các nhóm:</p>
<ul>
    <li>Laptop: Văn phòng, Gaming, Đồ họa chuyên nghiệp</li>
    <li>PC: Lắp ráp sẵn, gaming, mini PC</li>
    <li>Gaming Gear: Chuột, bàn phím, tai nghe, ghế gaming</li>
    <li>Linh kiện: CPU, RAM, SSD, VGA, bo mạch chủ</li>
    <li>Phụ kiện công nghệ: USB, ổ cứng di động, webcam, màn hình</li>
</ul>

<h2>Cam kết chất lượng</h2>
<p>Chúng tôi cam kết:</p>
<ul>
    <li>100% sản phẩm chính hãng, đầy đủ tem, nhãn, giấy tờ bảo hành.</li>
    <li>Giá cả cạnh tranh và minh bạch.</li>
    <li>Chăm sóc khách hàng tận tâm, phản hồi nhanh chóng.</li>
    <li>Đảm bảo trải nghiệm mua sắm trực tuyến và tại cửa hàng tốt nhất.</li>
</ul>

<h3>Tải catalogue sản phẩm</h3>
<p>Bạn có thể tải catalogue mới nhất của chúng tôi tại đây: <a href="/uploads/about/catalogue_2025.pdf" target="_blank">Download PDF</a></p>

<h2>Thông tin liên hệ</h2>
<p>Địa chỉ: 123 Nguyễn Văn Linh, TP.HCM</p>
<p>Hotline: <a href="tel:19005301">1900 5301</a></p>
<p>Email: <a href="mailto:support@techstore.com">support@techstore.com</a></p>
<p>Fanpage: <a href="https://www.facebook.com/TechStore">https://www.facebook.com/TechStore</a></p>
<p>Website: <a href="https://www.techstore.com.vn">www.techstore.com.vn</a></p>

<p>Chúng tôi luôn sẵn sàng phục vụ và đồng hành cùng khách hàng trong mọi nhu cầu công nghệ.</p>
');

CREATE TABLE InfoPages (
    InfoPageId INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Slug NVARCHAR(100) NOT NULL UNIQUE, 
    Content NVARCHAR(MAX),             
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);

Delete from InfoPages
where InfoPageId = 8;

INSERT INTO InfoPages (Title, Slug, Content) VALUES
(N'Giới thiệu TechStore', 'about', 
N'<h2>Giới thiệu TechStore</h2>
<p>Chào mừng bạn đến với TechStore – nơi cung cấp các sản phẩm công nghệ chất lượng cao, chính hãng và giá cả hợp lý. Chúng tôi cam kết mang đến trải nghiệm mua sắm trực tuyến an toàn và tiện lợi cho mọi khách hàng.</p>
<p>Với đội ngũ nhân viên tận tâm, TechStore luôn nỗ lực mang đến dịch vụ hỗ trợ khách hàng tốt nhất, đồng thời cập nhật các sản phẩm mới và công nghệ hiện đại nhất.</p>
<p><img src="/Content/images/about/storefront.jpg" alt="TechStore" style="max-width:100%; border-radius:8px; margin:10px 0;" /></p>
<p>Dowload brochure TechStore: <a href="/Content/files/TechStore-Brochure.pdf" target="_blank">TechStore Brochure (PDF)</a></p>
<p>Video giới thiệu: <br/><iframe width="560" height="315" src="https://www.youtube.com/embed/dQw4w9WgXcQ" frameborder="0" allowfullscreen></iframe></p>');

-- 2. Tin công nghệ
INSERT INTO InfoPages (Title, Slug, Content) VALUES
(N'Tin công nghệ', 'news', 
N'<h2>Tin công nghệ mới nhất</h2>
<p>Cập nhật những tin tức, xu hướng và công nghệ mới nhất trên thị trường. Từ smartphone, laptop, đến các thiết bị thông minh, TechStore luôn tổng hợp thông tin để bạn không bỏ lỡ bất cứ xu hướng nào.</p>
<ul>
    <li>Smartphone mới ra mắt</li>
    <li>Laptop cấu hình cao</li>
    <li>Thiết bị IoT thông minh</li>
</ul>
<p><img src="/Content/images/news/news1.jpg" alt="News" style="max-width:100%; border-radius:8px; margin:10px 0;" /></p>
<p>Tải PDF tổng hợp tin tức tuần: <a href="/Content/files/Weekly-News.pdf" target="_blank">Weekly News (PDF)</a></p>');

-- 3. Tuyển dụng
INSERT INTO InfoPages (Title, Slug, Content) VALUES
(N'Tuyển dụng', 'careers', 
N'<h2>Tuyển dụng tại TechStore</h2>
<p>TechStore luôn tìm kiếm những nhân sự tài năng, nhiệt huyết và sáng tạo để gia nhập đội ngũ. Chúng tôi cung cấp môi trường làm việc chuyên nghiệp, cơ hội thăng tiến và đãi ngộ hấp dẫn.</p>
<p>Hiện tại, chúng tôi đang tuyển dụng các vị trí:</p>
<ul>
    <li>Nhân viên bán hàng</li>
    <li>Nhân viên kỹ thuật</li>
    <li>Marketing</li>
    <li>Quản lý kho</li>
</ul>
<p><img src="/Content/images/careers/team.jpg" alt="Team" style="max-width:100%; border-radius:8px; margin:10px 0;" /></p>
<p>Download form ứng tuyển: <a href="/Content/files/Job-Application.pdf" target="_blank">Job Application (PDF)</a></p>');

-- 4. Liên hệ
INSERT INTO InfoPages (Title, Slug, Content) VALUES
(N'Liên hệ', 'contact', 
N'<h2>Liên hệ với TechStore</h2>
<p>Nếu bạn có bất kỳ thắc mắc hay cần hỗ trợ, vui lòng liên hệ với chúng tôi qua các kênh sau:</p>
<ul>
    <li>Email: support@techstore.com</li>
    <li>Điện thoại: 1900 1234</li>
    <li>Địa chỉ: 123 Đường Công Nghệ, Quận 1, TP.HCM</li>
</ul>
<p>Bản đồ: <br/><iframe src="https://www.google.com/maps/embed?pb=!1m18!..." width="600" height="450" style="border:0;" allowfullscreen="" loading="lazy"></iframe></p>
<p><img src="/Content/images/contact/contact-office.jpg" alt="Office" style="max-width:100%; border-radius:8px; margin:10px 0;" /></p>');

-- 5. Bảo hành
INSERT INTO InfoPages (Title, Slug, Content) VALUES
(N'Bảo hành', 'warranty', 
N'<h2>Chính sách bảo hành</h2>
<pTechStore cung cấp bảo hành chính hãng từ 12-24 tháng tùy sản phẩm. Để đảm bảo quyền lợi, vui lòng giữ hóa đơn mua hàng và tem bảo hành.</p>
<p>Chi tiết từng loại sản phẩm:</p>
<ul>
    <li>Điện thoại: 12 tháng</li>
    <li>Laptop: 24 tháng</li>
    <li>Thiết bị phụ kiện: 6-12 tháng</li>
</ul>
<p>Tải PDF hướng dẫn bảo hành: <a href="/Content/files/Warranty-Policy.pdf" target="_blank">Warranty Policy (PDF)</a></p>');

-- 6. Đổi trả
INSERT INTO InfoPages (Title, Slug, Content) VALUES
(N'Đổi trả', 'returns', 
N'<h2>Chính sách đổi trả</h2>
<p>TechStore hỗ trợ đổi trả trong vòng 7 ngày kể từ ngày nhận hàng. Sản phẩm phải còn nguyên tem, chưa qua sử dụng và đầy đủ phụ kiện.</p>
<p><img src="/Content/images/returns/returns.jpg" alt="Returns" style="max-width:100%; border-radius:8px; margin:10px 0;" /></p>
<p>Download hướng dẫn đổi trả: <a href="/Content/files/Returns-Guide.pdf" target="_blank">Returns Guide (PDF)</a></p>');

-- 7. Vận chuyển
INSERT INTO InfoPages (Title, Slug, Content) VALUES
(N'Vận chuyển', 'shipping', 
N'<h2>Chính sách vận chuyển</h2>
<p>TechStore hỗ trợ giao hàng toàn quốc, với thời gian từ 1-5 ngày tùy khu vực. Chúng tôi sử dụng các đơn vị vận chuyển uy tín để đảm bảo hàng hóa được giao nhanh và an toàn.</p>
<p>Tải PDF chi tiết vận chuyển: <a href="/Content/files/Shipping-Policy.pdf" target="_blank">Shipping Policy (PDF)</a></p>');

-- 8. Thanh toán
INSERT INTO InfoPages (Title, Slug, Content) VALUES
(N'Thanh toán', 'payment', 
N'<h2>Chính sách thanh toán</h2>
<p>TechStore hỗ trợ nhiều phương thức thanh toán:</p>
<ul>
    <li>Thanh toán khi nhận hàng (COD)</li>
    <li>Chuyển khoản ngân hàng</li>
    <li>Thanh toán online qua thẻ quốc tế (Visa, Mastercard)</li>
</ul>
<p>Tải PDF hướng dẫn thanh toán: <a href="/Content/files/Payment-Guide.pdf" target="_blank">Payment Guide (PDF)</a></p>');


CREATE TABLE NewsPosts (
    NewsId INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Slug NVARCHAR(100) NOT NULL UNIQUE,
    Content NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Contacts (
    ContactId INT IDENTITY PRIMARY KEY,
    FullName NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    Message NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE NewsletterSubscribers (
    SubscriberId INT IDENTITY PRIMARY KEY,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    UserId INT NULL, 
    IsActive BIT DEFAULT 1,
    Source NVARCHAR(50) DEFAULT 'Footer',
    CreatedAt DATETIME DEFAULT GETDATE(),
    UnsubscribedAt DATETIME NULL,
    CONSTRAINT FK_Newsletter_User
        FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE EmailQueue (
    EmailQueueId INT IDENTITY PRIMARY KEY,
    ToEmail NVARCHAR(100) NOT NULL,       
    Subject NVARCHAR(255) NOT NULL,       
    Body NVARCHAR(MAX) NOT NULL,         
    EmailType NVARCHAR(50) NOT NULL,    
    Status TINYINT NOT NULL DEFAULT 0,     
    RetryCount INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    SentAt DATETIME NULL,
    SubscriberId INT NULL,
    CONSTRAINT FK_EmailQueue_Subscriber
        FOREIGN KEY (SubscriberId)
        REFERENCES NewsletterSubscribers(SubscriberId)
);
