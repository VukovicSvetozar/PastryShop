-- Kreiranje šeme
CREATE SCHEMA IF NOT EXISTS `hci_pastryshop` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `hci_pastryshop`;

-- Kreiranje osnovne tabele za korisnike (superklasa User)
CREATE TABLE Users (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password CHAR(64) NOT NULL,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    PhoneNumber VARCHAR(15) NOT NULL,
    Address VARCHAR(255),
    ImagePath VARCHAR(255),
    HireDate DATE NOT NULL,
    Salary DECIMAL(10, 2) NOT NULL,
    LastLogin DATETIME,
    UserType ENUM('Manager', 'Cashier') NOT NULL,
    Theme ENUM('Light', 'Dark', 'Blue') DEFAULT 'Light',
    Language ENUM('English', 'Serbian') DEFAULT 'Serbian',
    IsActive BIT(1) DEFAULT 1,
    ForcePasswordChange BIT(1) DEFAULT 0
);

-- Kreiranje tabele za menadžere (podklasa Manager)
CREATE TABLE Managers (
    UserId INT PRIMARY KEY,
    Department VARCHAR(100) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Kreiranje tabele za blagajnike (podklasa Cashier)
CREATE TABLE Cashiers (
    UserId INT PRIMARY KEY,
    CashRegisterId INT NOT NULL,
    ShiftStart DATETIME NOT NULL,
    ShiftEnd DATETIME NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
-- Kreiranje testnog naloga (Username: maja, Password: ajam; Username: marko, Password:akrom)
INSERT INTO Users (
    Username, Password, FirstName, LastName, PhoneNumber, Address, ImagePath, HireDate, Salary, LastLogin, UserType, Theme, Language, IsActive, ForcePasswordChange) 
	VALUES (
    'maja', 'f48dfa0619e8ce6f4719de9ba26ae3a26119ac7be004a8f1ef666ad17062c231', 'Maja', 'Matić', '+381541234568', 'Trebinje, RS', 'Maja.png', '2024-06-13', 1700.00, NOW(),
    'Manager', 'Light', 'Serbian', 1, 0);
INSERT INTO Users (
    Username, Password, FirstName, LastName, PhoneNumber, Address, ImagePath, HireDate, Salary, LastLogin, UserType, Theme, Language, IsActive, ForcePasswordChange) 
	VALUES (
    'marko', 'd2533c5b16b9af8da62af8a30f0bac13a6db78e1f8742e14d507da9bd6c26f8b', 'Marko', 'Marković', '+381541234569', 'Banja Luka, RS', 'Marko.png', '2024-06-13', 1100.00, NOW(),
    'Cashier', 'Light', 'Serbian', 1, 0);
INSERT INTO Managers (userId, department) VALUES (10, 'Test');
INSERT INTO Managers (userId, department) VALUES (11, 'Sales');
INSERT INTO Cashiers (userId, cashRegisterId, shiftStart, shiftEnd) VALUES (12, 1, '2025-04-03 10:41:33', '2025-04-03 18:41:33');

-- Kreiranje osnovne tabele za proizvode (superklasa Product)
CREATE TABLE Products (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    ProductType ENUM('Food', 'Drink', 'Accessory') NOT NULL,
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    Price DECIMAL(10, 2) NOT NULL,
    ImagePath VARCHAR(255),
	IsAvailable BIT(1) DEFAULT 1,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedDate DATETIME,
    IsFeatured BIT(1) DEFAULT 0,
    Discount DECIMAL(5, 2) DEFAULT 0
);
-- Kreiranje tabele za proizvode hrane (podklasa FoodProduct)
CREATE TABLE FoodProducts (
    ProductId INT PRIMARY KEY,
    FoodType ENUM('Cake', 'Pastry', 'Sweet', 'Bakery') NOT NULL,
    Weight DECIMAL(10, 2) NOT NULL DEFAULT 0,
	IsPerishable BIT(1) DEFAULT 1,
    Calories INT,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);
-- Kreiranje tabele za pića (podklasa DrinkProduct)
CREATE TABLE DrinkProducts (
    ProductId INT PRIMARY KEY,
    Volume DECIMAL(10, 2),
	IsAlcoholic BIT(1) DEFAULT 0,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);
-- Kreiranje tabele za dodatke (podklasa AccessoryProduct)
CREATE TABLE AccessoryProducts (
    ProductId INT PRIMARY KEY,
    Material VARCHAR(100) NOT NULL,
    Dimensions VARCHAR(100),
	IsReusable BIT(1) DEFAULT 0,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);

-- Umetanje proizvoda u tabelu
INSERT INTO Products (Id, ProductType, Name, Description, Price, ImagePath, IsAvailable, CreatedDate, UpdatedDate, IsFeatured, Discount)
VALUES
(1, 'Food', 'Chocolate Cake', 'Rich and creamy chocolate cake', 4.99, 'Food_Chocolate_Cake.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(2, 'Food', 'Red Velvet Cake', 'Soft and moist red velvet cake with cream cheese frosting', 5.50, 'Food_Red_Velvet_Cake.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(3, 'Food', 'Cheesecake', 'Classic New York-style cheesecake', 2.00, 'Food_Cheesecake.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(4, 'Food', 'Black Forest Cake', 'Layered chocolate cake with cherries and whipped cream', 6.75, 'Food_Black_Forest_Cake.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(5, 'Food', 'Carrot Cake', 'Delicious carrot cake with cream cheese frosting', 4.00, 'Food_Carrot_Cake.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(6, 'Food', 'Lemon Cake', 'Refreshing lemon-flavored cake with a citrus glaze', 4.50, 'Food_Lemon_Cake.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(7, 'Food', 'Strawberry Shortcake', 'Light and fluffy cake layered with strawberries and cream', 5.00, 'Food_Strawberry_Shortcake.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(8, 'Food', 'Tiramisu', 'Classic Italian dessert with layers of mascarpone and coffee-soaked ladyfingers', 3.00, 'Food_Tiramisu.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0);

INSERT INTO FoodProducts (ProductId, FoodType, Weight, IsPerishable, Calories)
VALUES
(1, 'Cake', 0.2, TRUE, 350),
(2, 'Cake', 0.5, TRUE, 400),
(3, 'Cake', 0.3, TRUE, 450),
(4, 'Cake', 0.6, TRUE, 370),
(5, 'Cake', 0.4, TRUE, 320),
(6, 'Cake', 0.2, TRUE, 300),
(7, 'Cake', 0.3, TRUE, 340),
(8, 'Cake', 0.3, TRUE, 410);

INSERT INTO Products (Id, ProductType, Name, Description, Price, ImagePath, IsAvailable, CreatedDate, UpdatedDate, IsFeatured, Discount)
VALUES
(9, 'Food', 'Croissant', 'Buttery and flaky French pastry', 3.50, 'Food_Croissant.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(10, 'Food', 'Chocolate Croissant', 'Croissant filled with rich chocolate', 4.00, 'Food_Chocolate_Croissant.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(11, 'Food', 'Danish Pastry', 'Sweet pastry with fruit or cream filling', 4.25, 'Food_Danish_Pastry.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(12, 'Food', 'Cinnamon Roll', 'Soft pastry roll with cinnamon sugar and glaze', 3.75, 'Food_Cinnamon_Roll.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(13, 'Food', 'Apple Turnover', 'Crispy pastry filled with sweet apple compote', 4.50, 'Food_Apple_Turnover.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(14, 'Food', 'Eclair', 'Light pastry filled with cream and topped with chocolate', 3.90, 'Food_Eclair.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(15, 'Food', 'Puff Pastry', 'Delicate layers of pastry, ideal for desserts', 2.80, 'Food_Puff_Pastry.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(16, 'Food', 'Baklava', 'Sweet pastry made with layers of filo and nuts', 5.50, 'Food_Baklava.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0);

INSERT INTO FoodProducts (ProductId, FoodType, Weight, IsPerishable, Calories)
VALUES
(9, 'Pastry', 0.08, TRUE, 120),
(10, 'Pastry', 0.10, TRUE, 140),
(11, 'Pastry', 0.09, TRUE, 160),
(12, 'Pastry', 0.12, TRUE, 200),
(13, 'Pastry', 0.15, TRUE, 180),
(14, 'Pastry', 0.10, TRUE, 170),
(15, 'Pastry', 0.12, TRUE, 150),
(16, 'Pastry', 0.13, TRUE, 190);

INSERT INTO Products (Id, ProductType, Name, Description, Price, ImagePath, IsAvailable, CreatedDate, UpdatedDate, IsFeatured, Discount)
VALUES
(17, 'Food', 'Baguette', 'Crispy and soft French bread', 2.5, 'Food_Baguette.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(18, 'Food', 'Sourdough Bread', 'Artisan bread with a tangy flavor', 3.8, 'Food_Sourdough_Bread.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(19, 'Food', 'Ciabatta', 'Rustic Italian bread with a soft texture', 3.5, 'Food_Ciabatta.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(20, 'Food', 'Whole Grain Bread', 'Healthy bread made with whole grains', 3.0, 'Food_Whole_Grain_Bread.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(21, 'Food', 'Focaccia', 'Italian flatbread seasoned with herbs', 3.2, 'Food_Focaccia.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(22, 'Food', 'Pretzel', 'Soft and chewy pretzel with a golden crust', 2.0, 'Food_Pretzel.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(23, 'Food', 'Brioche', 'Rich and buttery French bread', 3.9, 'Food_Brioche.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(24, 'Food', 'Bagel', 'Dense and chewy bread ring', 1.8, 'Food_Bagel.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0);

INSERT INTO FoodProducts (ProductId, FoodType, Weight, IsPerishable, Calories)
VALUES
(17, 'Bakery', 0.4, TRUE, 260),
(18, 'Bakery', 0.5, TRUE, 300),
(19, 'Bakery', 0.45, TRUE, 280),
(20, 'Bakery', 0.5, TRUE, 240),
(21, 'Bakery', 0.4, TRUE, 270),
(22, 'Bakery', 0.2, TRUE, 200),
(23, 'Bakery', 0.35, TRUE, 310),
(24, 'Bakery', 0.2, TRUE, 220);

INSERT INTO Products (Id, ProductType, Name, Description, Price, ImagePath, IsAvailable, CreatedDate, UpdatedDate, IsFeatured, Discount)
VALUES
(25, 'Food', 'Milk Chocolate Bar', 'Smooth and creamy milk chocolate', 2.5, 'Food_Milk_Chocolate_Bar.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(26, 'Food', 'Dark Chocolate Truffles', 'Rich dark chocolate with a smooth filling', 8.5, 'Food_Dark_Chocolate_Truffles.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(27, 'Food', 'Gummy Bears', 'Soft and chewy fruit-flavored gummies', 3.0, 'Food_Gummy_Bears.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(28, 'Food', 'Marshmallows', 'Soft and fluffy marshmallow treats', 2.8, 'Food_Marshmallows.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(29, 'Food', 'Caramel Candies', 'Rich and buttery caramel candies', 3.5, 'Food_Caramel_Candies.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(30, 'Food', 'Lollipops', 'Brightly colored fruit-flavored lollipops', 1.5, 'Food_Lollipops.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(31, 'Food', 'Hard Candy Mix', 'Assorted hard candies with different flavors', 4.0, 'Food_Hard_Candy_Mix.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(32, 'Food', 'Fudge', 'Creamy and rich fudge squares', 5.0, 'Food_Fudge.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0);

INSERT INTO FoodProducts (ProductId, FoodType, Weight, IsPerishable, Calories)
VALUES
(25, 'Sweet', 0.1, FALSE, 250),
(26, 'Sweet', 0.2, FALSE, 400),
(27, 'Sweet', 0.15, FALSE, 150),
(28, 'Sweet', 0.2, FALSE, 100),
(29, 'Sweet', 0.15, FALSE, 120),
(30, 'Sweet', 0.1, FALSE, 80),
(31, 'Sweet', 0.25, FALSE, 200),
(32, 'Sweet', 0.2, FALSE, 220);

INSERT INTO Products (Id, ProductType, Name, Description, Price, ImagePath, IsAvailable, CreatedDate, UpdatedDate, IsFeatured, Discount)
VALUES
(33, 'Drink', 'Classic Lemonade', 'Refreshing homemade lemonade with a hint of mint.', 3.50, 'Drink_Classic_Lemonade.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(34, 'Drink', 'Iced Coffee', 'Chilled coffee served with milk and ice.', 4.00, 'Drink_Iced_Coffee.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(35, 'Drink', 'Fresh Orange Juice', '100% freshly squeezed orange juice.', 4.50, 'Drink_Fresh_Orange_Juice.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(36, 'Drink', 'Espresso', 'Rich and aromatic espresso shot.', 2.50, 'Drink_Espresso.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(37, 'Drink', 'Cappuccino', 'Creamy cappuccino with a frothy top.', 3.50, 'Drink_Cappuccino.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(38, 'Drink', 'Mango Smoothie', 'Blended mango and yogurt smoothie.', 5.00, 'Drink_Mango_Smoothie.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(39, 'Drink', 'Green Tea', 'Healthy and soothing hot green tea.', 2.00, 'Drink_Green_Tea.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(40, 'Drink', 'Hot Chocolate', 'Warm and creamy hot chocolate with whipped cream.', 3.00, 'Drink_Hot_Chocolate.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(41, 'Drink', 'Strawberry Milkshake', 'Delicious strawberry milkshake with fresh cream.', 4.50, 'Drink_Strawberry_Milkshake.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(42, 'Drink', 'Classic Mojito', 'Refreshing mojito with mint, lime, and soda water.', 5.00, 'Drink_Classic_Mojito.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(43, 'Drink', 'Red Wine', 'Premium dry red wine from local vineyards.', 7.50, 'Drink_Red_Wine.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(44, 'Drink', 'Pina Colada', 'Tropical blend of pineapple juice, coconut, and rum.', 6.50, 'Drink_Pina_Colada.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(45, 'Drink', 'Sparkling Water', 'Crisp and refreshing sparkling water.', 2.00, 'Drink_Sparkling_Water.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(46, 'Drink', 'Craft Beer', 'Locally brewed craft beer with rich flavor.', 4.00, 'Drink_Craft_Beer.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(47, 'Drink', 'Peach Iced Tea', 'Sweet peach iced tea served cold.', 3.00, 'Drink_Peach_Iced_Tea.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(48, 'Drink', 'Whiskey Sour', 'Classic whiskey sour with a hint of lemon.', 7.00, 'Drink_Whiskey_Sour.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(49, 'Drink', 'Herbal Tea', 'Warm herbal tea made with a blend of chamomile and lavender.', 2.50, 'Drink_Herbal_Tea.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(50, 'Drink', 'Milk Coffee', 'Smooth and creamy coffee with milk.', 3.00, 'Drink_Milk_Coffee.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(51, 'Drink', 'Energy Drink', 'Boost your energy with this refreshing drink.', 2.50, 'Drink_Energy_Drink.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(52, 'Drink', 'Lime Soda', 'Chilled soda with a splash of lime flavor.', 2.50, 'Drink_Lime_Soda.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0);

INSERT INTO DrinkProducts (ProductId, Volume, IsAlcoholic)
VALUES
(33, 0.3, FALSE),
(34, 0.25, FALSE),
(35, 0.3, FALSE),
(36, 0.06, FALSE),
(37, 0.2, FALSE),
(38, 0.35, FALSE),
(39, 0.25, FALSE),
(40, 0.3, FALSE),
(41, 0.35, FALSE),
(42, 0.4, TRUE),
(43, 0.15, TRUE),
(44, 0.4, TRUE),
(45, 0.5, FALSE),
(46, 0.33, TRUE),
(47, 0.3, FALSE),
(48, 0.15, TRUE),
(49, 0.25, FALSE),
(50, 0.3, FALSE),
(51, 0.33, FALSE),
(52, 0.3, FALSE);

INSERT INTO Products (Id, ProductType, Name, Description, Price, ImagePath, IsAvailable, CreatedDate, UpdatedDate, IsFeatured, Discount)
VALUES 
(53, 'Accessory', 'Kitchen Timer', 'Kitchen timer for precise cooking', 10.00, 'Accessory_Kitchen_Timer.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(54, 'Accessory', 'Cutting Board', 'High-quality wooden cutting board for all kitchens', 8.00, 'Accessory_Cutting_Board.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(55, 'Accessory', 'Bamboo Utensil Set', 'Eco-friendly bamboo utensil set including fork, spoon, and knife', 12.75, 'Accessory_Bamboo_Utensil_Set.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(56, 'Accessory', 'Baking Pan', 'Non-stick baking pan for versatile use', 25.00, 'Accessory_Baking_Pan.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(57, 'Accessory', 'Cotton Napkin Set', 'A set of premium cotton napkins for dining and decor', 6.50, 'Accessory_Cotton_Napkin_Set.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0),
(58, 'Accessory', 'Silicone Spatula', 'High-quality silicone spatula suitable for cooking and baking', 8.99, 'Accessory_Silicone_Spatula.png', TRUE, CURRENT_TIMESTAMP, NULL, FALSE, 0);

INSERT INTO AccessoryProducts (ProductId, Material, Dimensions, IsReusable)
VALUES 
(53, 'Plastic', '30x30', 1),
(54, 'Wood', '40x30', 1),
(55, 'Bamboo', NULL, 1),
(56, 'Aluminum', '40x30', 1),
(57, 'Cotton', '30x30', 1),
(58, 'Silicone', '30', 1);

-- Kreiranje tabele za zalihe (Stock)
CREATE TABLE Stocks (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    ProductId INT NOT NULL,
    Quantity INT DEFAULT 0,
    ExpirationDate DATETIME,
    ExpirationWarningDays INT,
	IsActive BIT(1) DEFAULT 1,
	IsWarning  BIT(1) DEFAULT 0,
    AddedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);

-- Kreiranje tabele za narudzbe
CREATE TABLE Orders (
    Id INT AUTO_INCREMENT PRIMARY KEY,
	UserId INT NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    TotalPrice DECIMAL(10,2) NOT NULL,
    Status ENUM('Completed', 'Cancelled', 'OnHold') NOT NULL,
	FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Kreiranje tabele za stavke narudzbe
CREATE TABLE OrderItems (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
	FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
	FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);

-- Kreiranje tabele za placanje
CREATE TABLE Payments (
    Id INT AUTO_INCREMENT PRIMARY KEY,
	UserId INT NOT NULL,
    OrderId INT NOT NULL,
    PaymentMethod ENUM('Cash', 'Card') NOT NULL,
    PaymentStatus ENUM('Completed', 'Failed', 'Refunded') NOT NULL,
    AmountPaid DECIMAL(10,2) NOT NULL,
	CardNumber VARCHAR(255),
    PaymentDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
	FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE
);

-- Kreiranje tabele za transakcije zaliha
CREATE TABLE StockTransactions (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    StockId INT NOT NULL,
    ProductId INT NOT NULL,
	OrderId INT,
	UserId INT NOT NULL,
    QuantityChanged INT DEFAULT 0,
    TransactionDate DATETIME,
    TransactionType ENUM('Addition', 'Sale', 'Return', 'Adjustment') NOT NULL,
    FOREIGN KEY (StockId) REFERENCES Stocks(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Kreiranje tabele za modifikacije zaliha
CREATE TABLE StockModifications (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    StockId INT NOT NULL,
    ProductId INT NOT NULL,
    UserId INT NOT NULL,
    OldValue VARCHAR(255) NOT NULL,
    NewValue VARCHAR(255) NOT NULL,
    ModificationDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ModificationType ENUM('ExpirationDateChange', 'WarningDaysChange', 'StatusChange') NOT NULL,
    FOREIGN KEY (StockId) REFERENCES Stocks(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
