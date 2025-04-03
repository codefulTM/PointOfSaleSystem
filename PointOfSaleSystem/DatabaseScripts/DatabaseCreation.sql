CREATE TABLE SUPPLIER (
    supplier_id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255),
    phone_number VARCHAR(50)
);

CREATE TABLE CATEGORY (
    category_id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
	deleted BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE PRODUCT (
    product_id SERIAL PRIMARY KEY,
    barcode VARCHAR(255),
    name VARCHAR(255) NOT NULL,
    category_id INT,
    supplier_id INT,
    brand VARCHAR(255),
    quantity INT,
    cost_price INT,
    selling_price INT,
    image TEXT,
    deleted BOOLEAN NOT NULL DEFAULT FALSE,
    FOREIGN KEY (category_id) REFERENCES CATEGORY(category_id),
    FOREIGN KEY (supplier_id) REFERENCES SUPPLIER(supplier_id)
);

CREATE TABLE SUPPLIES (
    supplies_id SERIAL PRIMARY KEY,
    supplier_id INT,
    name VARCHAR(255) NOT NULL,
    brand VARCHAR(255),
    stock FLOAT,
    unit VARCHAR(50),
    cost_price INT,
    FOREIGN KEY (supplier_id) REFERENCES SUPPLIER(supplier_id)
);

CREATE TABLE CUSTOMER (
    customer_id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    phone_number VARCHAR(50),
    address VARCHAR(255),
    birthday DATE,
    gender VARCHAR(10),
    deleted BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE "order" (
    order_id SERIAL PRIMARY KEY,
    customer_id INT,
    total_price INT,
    discount INT,
    paid BOOLEAN,
    FOREIGN KEY (customer_id) REFERENCES CUSTOMER(customer_id)
);

CREATE TABLE DETAIL (
    order_id INT,
    product_id INT,
    count INT,
    deleted BOOLEAN NOT NULL DEFAULT FALSE,
    PRIMARY KEY (order_id, product_id),
    FOREIGN KEY (order_id) REFERENCES "order"(order_id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES PRODUCT(product_id) ON DELETE CASCADE
);

CREATE TABLE PURCHASE_ORDER (
    purchase_id SERIAL PRIMARY KEY,
    supplier_id INT,
    purchase_date DATE DEFAULT CURRENT_DATE,
    total_price INT,
    status VARCHAR(50) DEFAULT 'pending',
    FOREIGN KEY (supplier_id) REFERENCES SUPPLIER(supplier_id)
);

CREATE TABLE PURCHASE_DETAIL (
    purchase_id INT,
    supplies_id INT,
    quantity INT,
    cost_price INT,
    PRIMARY KEY (purchase_id, supplies_id),
    FOREIGN KEY (purchase_id) REFERENCES PURCHASE_ORDER(purchase_id) ON DELETE CASCADE,
    FOREIGN KEY (supplies_id) REFERENCES SUPPLIES(supplies_id) ON DELETE CASCADE
);

CREATE TABLE PRODUCT_SUPPLIES (
    product_id INT,
    supplies_id INT,
    quantity_required REAL,
    unit VARCHAR(50),
    PRIMARY KEY (product_id, supplies_id),
    FOREIGN KEY (product_id) REFERENCES PRODUCT(product_id) ON DELETE CASCADE,
    FOREIGN KEY (supplies_id) REFERENCES SUPPLIES(supplies_id) ON DELETE CASCADE
);
