-- IF DB_ID('FoodOrderingDb') IS NULL
-- 	CREATE DATABASE FoodOrderingDb;
-- GO

-- USE FoodOrderingDb;
-- GO

-- -- table

-- IF OBJECT_ID('dbo.Categories', 'U') IS NULL
-- CREATE TABLE dbo.Categories
-- (
-- 	Id   INT IDENTITY(1,1) NOT NULL,
-- 	Name NVARCHAR(50)      NOT NULL,

-- 	CONSTRAINT PK_Categories      PRIMARY KEY (Id),
-- 	CONSTRAINT UQ_Categories_Name UNIQUE (Name)
-- );
-- GO

-- IF OBJECT_ID('dbo.MenuItems', 'U') IS NULL
-- CREATE TABLE dbo.MenuItems
-- (
-- 	Id         INT IDENTITY(1,1) NOT NULL,
-- 	Name       NVARCHAR(100)     NOT NULL,
-- 	Price      INT               NOT NULL,
-- 	Stock      INT               NOT NULL,
-- 	CategoryId INT               NOT NULL,

-- 	CONSTRAINT PK_MenuItems      PRIMARY KEY (Id),
-- 	CONSTRAINT UQ_MenuItems_Name UNIQUE (Name), 
-- 	CONSTRAINT CK_MenuItems_Price CHECK (Price > 0),
-- 	CONSTRAINT CK_MenuItems_Stock CHECK (Stock >= 0),

-- 	CONSTRAINT FK_MenuItems_Categories FOREIGN KEY (CategoryId)
-- 		REFERENCES dbo.Categories (Id) ON DELETE NO ACTION
-- );
-- GO

-- -- index

-- IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MenuItems_CategoryId')
-- 	CREATE INDEX IX_MenuItems_CategoryId ON dbo.MenuItems (CategoryId);

-- IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MenuItems_Price')
-- 	CREATE INDEX IX_MenuItems_Price ON dbo.MenuItems (Price); 
-- GO

-- -- seed data

-- IF NOT EXISTS (SELECT 1 FROM dbo.Categories)
-- 	INSERT INTO dbo.Categories (Name) VALUES ('Burger'), ('Paket'), ('Minuman');
-- GO

-- IF NOT EXISTS (SELECT 1 FROM dbo.MenuItems)
-- BEGIN
-- 	DECLARE @Burger INT  = (SELECT Id FROM dbo.Categories WHERE Name = 'Burger');
-- 	DECLARE @Paket INT   = (SELECT Id FROM dbo.Categories WHERE Name = 'Paket');
-- 	DECLARE @Minuman INT = (SELECT Id FROM dbo.Categories WHERE Name = 'Minuman');

-- 	INSERT INTO dbo.MenuItems (Name, Price, Stock, CategoryId) VALUES
-- 		('Pitik Bangor',        15000, 20, @Burger),
-- 		('Single Burger',       23000, 15, @Burger),
-- 		('Double Burger',       33000, 12, @Burger),
-- 		('Cheese Burger',       27000, 15, @Burger),
-- 		('Double Cheese Burger',38000, 10, @Burger),
-- 		('Smoky Beef Burger',   30000, 10, @Burger),
-- 		('Pitik Cheese Burger', 20000, 15, @Burger),
-- 		('Bangor X',            35000,  8, @Burger),
-- 		('Paket Hemat Bangor',  42000, 10, @Paket),
-- 		('Es Teh',               7000, 30, @Minuman),
-- 		('Es Jeruk',             8000, 30, @Minuman);
-- END
-- GO
-- SELECT * FROM dbo.MenuItems;
-- SELECT * FROM dbo.Categories;