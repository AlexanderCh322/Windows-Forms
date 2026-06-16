-- Таблица: Заказчики (Customers)
CREATE TABLE Customers (
    customer_id INT IDENTITY(1,1) PRIMARY KEY,
    customer_name NVARCHAR(255) NOT NULL,
    inn NVARCHAR(50),              
    address NVARCHAR(255),         
    phone NVARCHAR(50),            
    is_seller BIT DEFAULT 0,       
    is_buyer BIT DEFAULT 0         
);

-- Таблица: Материалы (Materials)
CREATE TABLE Materials (
    material_id INT IDENTITY(1,1) PRIMARY KEY,
    material_name NVARCHAR(255) NOT NULL,
    unit_price DECIMAL(10, 2) NOT NULL
);

-- Таблица: Продукция (Products)
CREATE TABLE Products (
    product_id INT IDENTITY(1,1) PRIMARY KEY,
    product_name NVARCHAR(255) NOT NULL
);

-- Таблица: Спецификации (Specifications)
CREATE TABLE Specifications (
    spec_id INT IDENTITY(1,1) PRIMARY KEY,
    product_id INT NOT NULL,
    material_id INT NOT NULL,
    consumption_norm DECIMAL(10, 4) NOT NULL, -- Норма расхода
    FOREIGN KEY (product_id) REFERENCES Products(product_id),
    FOREIGN KEY (material_id) REFERENCES Materials(material_id)
);

-- Таблица: Заказы (Orders)
CREATE TABLE Orders (
    order_id INT IDENTITY(1,1) PRIMARY KEY,
    customer_id INT NOT NULL,
    order_date DATE NOT NULL,
    FOREIGN KEY (customer_id) REFERENCES Customers(customer_id)
);

-- Таблица: Состав заказа (Order_Items)
CREATE TABLE Order_Items (
    item_id INT IDENTITY(1,1) PRIMARY KEY,
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    FOREIGN KEY (order_id) REFERENCES Orders(order_id),
    FOREIGN KEY (product_id) REFERENCES Products(product_id)
);

-- Таблица: Пользователи (Users) — для Модуля 4
CREATE TABLE Users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    login NVARCHAR(100) UNIQUE NOT NULL,
    password NVARCHAR(255) NOT NULL,
    role NVARCHAR(50) NOT NULL DEFAULT 'User', -- 'Admin' или 'User'
    is_blocked BIT DEFAULT 0,
    failed_attempts INT DEFAULT 0
);
GO

-- =================================================================
-- 4. ИМПОРТ ДАННЫХ ИЗ JSON В ТАБЛИЦУ CUSTOMERS
-- =================================================================

DECLARE @JsonData NVARCHAR(MAX) = N'
[
	{
		"id": "000000001",
		"name": "ООО \"Поставка\"",
		"inn": "",
		"addres": "г.Пятигорск",
		"phone": "+79198634592",
		"salesman": true,
		"buyer": true
	},
	{
		"id": "000000002",
		"name": "ООО \"Кинотеатр Квант\"",
		"inn": "26320045123",
		"addres": "г. Железноводск, ул. Мира, 123",
		"phone": "+79884581555",
		"salesman": true,
		"buyer": false
	},
	{
		"id": "000000008",
		"name": "ООО \"Новый JDTO\"",
		"inn": "26320045111",
		"addres": "г. Железноводсу",
		"phone": "+79884581555",
		"salesman": true,
		"buyer": false
	},
	{
		"id": "000000003",
		"name": "ООО \"Ромашка\"",
		"inn": "4140784214",
		"addres": "г. Омск, ул. Строителей, 294",
		"phone": "+79882584546",
		"salesman": false,
		"buyer": true
	},
	{
		"id": "000000009",
		"name": "ООО \"Ипподром\"",
		"inn": "5874045632",
		"addres": "г. Уфа, ул. Набережная,  37",
		"phone": "+79627486389",
		"salesman": true,
		"buyer": true
	},
	{
		"id": "000000010",
		"name": "ООО \"Ассоль\"",
		"inn": "2629011278",
		"addres": "г. Калуга, ул. Пушкина, 94",
		"phone": "+79184572398",
		"salesman": false,
		"buyer": true
	}
]
';

-- Импортируем JSON в созданную таблицу Customers
INSERT INTO Customers (customer_name, inn, address, phone, is_seller, is_buyer)
SELECT 
    [name], 
    [inn], 
    [addres], 
    [phone], 
    [salesman], 
    [buyer]
FROM OPENJSON(@JsonData)
WITH (
    name NVARCHAR(255) '$.name',
    inn NVARCHAR(50) '$.inn',
    addres NVARCHAR(255) '$.addres',
    phone NVARCHAR(50) '$.phone',
    salesman BIT '$.salesman',
    buyer BIT '$.buyer'
);
