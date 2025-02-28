-- Tạo cơ sở dữ liệu
CREATE DATABASE pos_management;

-- Sử dụng cơ sở dữ liệu
\c pos_management;

-- Bảng CATEGORY
CREATE TABLE CATEGORY (
    category_id SERIAL PRIMARY KEY,
    name NVARCHAR(255) NOT NULL
);

-- Bảng PRODUCT
CREATE TABLE PRODUCT (
    product_id SERIAL PRIMARY KEY,
    barcode NVARCHAR(255),
    name NVARCHAR(255) NOT NULL,
    category_id INT,
    supplier_id INT,
    brand NVARCHAR(255),
    quantity INT,
    cost_price INT,
    selling_price INT,
    image NVARCHAR(255),
    FOREIGN KEY (category_id) REFERENCES CATEGORY(category_id),
    FOREIGN KEY (supplier_id) REFERENCES SUPPLIER(supplier_id)
);

-- Bảng SUPPLIER
CREATE TABLE SUPPLIER (
    supplier_id SERIAL PRIMARY KEY,
    name NVARCHAR(255) NOT NULL,
    email NVARCHAR(255),
    phone_number NVARCHAR(50)
);

-- Bảng SUPPLIES
CREATE TABLE SUPPLIES (
    supplies_id SERIAL PRIMARY KEY,
    supplier_id INT,
    name NVARCHAR(255) NOT NULL,
    brand NVARCHAR(255),
    stock FLOAT,
    unit NVARCHAR(50),
    cost_price INT,
    FOREIGN KEY (supplier_id) REFERENCES SUPPLIER(supplier_id)
);

-- Bảng CUSTOMER
CREATE TABLE CUSTOMER (
    customer_id SERIAL PRIMARY KEY,
    name NVARCHAR(255) NOT NULL,
    phone_number NVARCHAR(50),
    address NVARCHAR(255),
    birthday DATE,
    gender NVARCHAR(10)
);

-- Bảng ORDER
CREATE TABLE "ORDER" (
    order_id SERIAL PRIMARY KEY,
    customer_id INT,
    total_price INT,
    discount INT,
    paid BOOLEAN,
    FOREIGN KEY (customer_id) REFERENCES CUSTOMER(customer_id)
);

-- Bảng DETAIL (chi tiết đơn hàng)
CREATE TABLE DETAIL (
    order_id INT,
    product_id INT,
    count INT,
    PRIMARY KEY (order_id, product_id),
    FOREIGN KEY (order_id) REFERENCES "ORDER"(order_id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES PRODUCT(product_id) ON DELETE CASCADE
);

-- Bảng PURCHASE_ORDER (đơn hàng nhập)
CREATE TABLE PURCHASE_ORDER (
    purchase_id SERIAL PRIMARY KEY,
    supplier_id INT,
    purchase_date DATE DEFAULT CURRENT_DATE,
    total_price INT,
    status NVARCHAR(50) DEFAULT 'pending',
    FOREIGN KEY (supplier_id) REFERENCES SUPPLIER(supplier_id)
);

-- Bảng PURCHASE_DETAIL (chi tiết đơn nhập)
CREATE TABLE PURCHASE_DETAIL (
    purchase_id INT,
    supplies_id INT,
    quantity INT,
    cost_price INT,
    PRIMARY KEY (purchase_id, supplies_id),
    FOREIGN KEY (purchase_id) REFERENCES PURCHASE_ORDER(purchase_id) ON DELETE CASCADE,
    FOREIGN KEY (supplies_id) REFERENCES SUPPLIES(supplies_id) ON DELETE CASCADE
);

-- Bảng PRODUCT_SUPPLIES (sản phẩm sử dụng nguyên liệu)
CREATE TABLE PRODUCT_SUPPLIES (
    product_id INT,
    supplies_id INT,
    quantity_required REAL,
    unit NVARCHAR(50),
    PRIMARY KEY (product_id, supplies_id),
    FOREIGN KEY (product_id) REFERENCES PRODUCT(product_id) ON DELETE CASCADE,
    FOREIGN KEY (supplies_id) REFERENCES SUPPLIES(supplies_id) ON DELETE CASCADE
);
