using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Npgsql;
using PointOfSaleSystem.Models;
using Windows.ApplicationModel.Store;

namespace PointOfSaleSystem.Services
{
    /// <summary>
    /// The <c>PostgresDao</c> class serves as a Data Access Object (DAO) implementation
    /// for interacting with a PostgreSQL database. It provides access to various 
    /// repository instances for managing entities such as Categories, Products, Customers, 
    /// Orders, OrderDetails, and PaymentMethods. Each repository follows the singleton 
    /// pattern to ensure a single instance is used throughout the application.
    /// </summary>
    public class PostgresDao : IDao
    {
        public PostgresDao()
        {
            Categories = PostgresCategoryRepository.GetInstance();
            Products = PostgresProductRepository.GetInstance();
            Customers = PostgresCustomerRepository.GetInstance();
            PaymentMethods = PostgresPaymentMethodRepository.GetInstance();
            Orders = PostgresOrderRepository.GetInstance();
        }

        public IRepository<Category> Categories { get; set; }
        public IRepository<Product> Products { get; set; }
        public IRepository<Customer> Customers { get; set; }
        public IRepository<Order> Orders { get; set; }
        public IRepository<OrderDetail> OrderDetails { get; set; }
        public IRepository<PaymentMethod> PaymentMethods { get; set; }

        /// <summary>
        /// Represents a repository for managing categories in the Point of Sale system.
        /// This class implements the <see cref="IRepository{Category}"/> interface and provides 
        /// methods for creating, reading, updating, and deleting categories in the database.
        /// </summary>
        public class PostgresCategoryRepository : IRepository<Category>
        {
            List<Category> categories = new List<Category>();
            private NpgsqlConnection _connection;

            // singleton instance
            private static PostgresCategoryRepository? _instance = null;

            /// <summary>
            /// Initializes a new instance of the <c>PostgresCategoryRepository</c> class with the specified
            /// connection to the PostgreSQL database.
            /// </summary>
            /// <param name="connection">The connection to the PostgreSQL database.</param>
            /// <remarks>
            /// This constructor is private and intended to be used by the singleton instance of the
            /// <c>PostgresCategoryRepository</c> class.
            /// </remarks>
            private PostgresCategoryRepository(NpgsqlConnection connection)
            {
                _connection = connection;
                GetAll();
            }

            /// <summary>
            /// Retrieves the singleton instance of the <c>PostgresCategoryRepository</c> class.
            /// </summary>
            /// <returns>
            /// The singleton instance of the <c>PostgresCategoryRepository</c> class.
            /// </returns>
            public static PostgresCategoryRepository GetInstance()
            {
                if (_instance == null)
                {
                    _instance = new PostgresCategoryRepository(new NpgsqlConnection(Configuration.CONNECTION_STRING));
                }
                return _instance;
            }

            /// <summary>
            /// Creates a new category in the database.
            /// </summary>
            /// <param name="entity">The category to create.</param>
            public void Create(Category entity)
            {
                string query = "INSERT INTO CATEGORY(name) VALUES(@name) RETURNING category_id;";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("name", entity.Name);

                    _connection.Open();
                    int categoryId = (int)cmd.ExecuteScalar();
                    entity.Id = categoryId;
                    categories.Add(entity);
                    _connection.Close();
                }
            }

            /// <summary>
            /// Deletes a category from the database.
            /// </summary>
            /// <param name="id">The ID of the category to delete.</param>
            public void Delete(int id)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Retrieves all categories from the database.
            /// </summary>
            /// <returns>
            /// A list of all categories in the database.
            /// </returns>
            public IEnumerable<Category> GetAll()
            {
                if (categories.Count == 0)
                {
                    string query = "SELECT * FROM CATEGORY WHERE deleted = @deleted";
                    _connection.Open();
                    using (var cmd = new NpgsqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("deleted", false);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Category category = new Category();
                                category.Id = reader.GetInt32(0);
                                category.Name = reader.GetString(1);
                                categories.Add(category);
                            }
                        }
                    }
                    _connection.Close();
                }
                return categories;
            }

            /// <summary>
            /// Retrieves a category from the database by its ID.
            /// </summary>
            /// <param name="id">The ID of the category to retrieve.</param>
            /// <returns>The category with the specified ID or null if not found.</returns>
            public Category GetById(int id)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Updates an existing category in the database.
            /// </summary>
            /// <param name="entity">The category to update.</param>
            /// <returns>True if the category was updated successfully, false otherwise.</returns>
            public void Update(Category entity)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Represents a repository for managing products in the Point of Sale system.
        /// This class implements the <see cref="IRepository{Product}"/> interface and provides 
        /// methods for creating, reading, updating, and deleting products in the database.
        /// </summary>
        public class PostgresProductRepository : IRepository<Product>
        {
            List<Product> products = new List<Product>();
            private NpgsqlConnection _connection;

            // singleton instance
            private static PostgresProductRepository _instance = null;


            /// <summary>
            /// Initializes a new instance of the <see cref="PostgresProductRepository"/> class.
            /// Retrieves all products from the database.
            /// </summary>
            /// <param name="connection">The database connection to use.</param>
            /// <returns>This method does not return a value.</returns>
            private PostgresProductRepository(NpgsqlConnection connection)
            {
                _connection = connection;
                GetAll();
            }

            /// <summary>
            /// Retrieves the singleton instance of the <see cref="PostgresProductRepository"/> class.
            /// </summary>
            /// <returns>
            /// The singleton instance of the <see cref="PostgresProductRepository"/> class.
            /// </returns>
            public static PostgresProductRepository GetInstance()
            {
                if (_instance == null)
                {
                    _instance = new PostgresProductRepository(new NpgsqlConnection(Configuration.CONNECTION_STRING));
                }
                return _instance;
            }

            /// <summary>
            /// Creates a new product in the database.
            /// </summary>
            /// <param name="entity">The product to create.</param>
            /// <returns>Nothing.</returns>
            public void Create(Product entity)
            {
                string query;

                // Find category id
                int? categoryId = null;
                if (entity.Category != null)
                {
                    query = "SELECT category_id FROM CATEGORY WHERE name = @name";

                    using (var cmd = new NpgsqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("name", entity.Category);
                        _connection.Open();
                        categoryId = (int?)cmd.ExecuteScalar();
                        _connection.Close();
                    }

                    // If category id is null, add a new category then get the id of it
                    if (categoryId == null)
                    {
                        PostgresCategoryRepository catRepo = PostgresCategoryRepository.GetInstance();
                        Category category = new Category() { Name = entity.Category };
                        catRepo.Create(category);
                        categoryId = category.Id;
                    }
                }

                // Find supplier id
                int? supplierId = null;
                if (entity.Supplier != null)
                {
                    query = "SELECT supplier_id FROM SUPPLIER WHERE name = @name";

                    using (var cmd = new NpgsqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("name", entity.Supplier);
                        _connection.Open();
                        supplierId = (int?)cmd.ExecuteScalar();
                        _connection.Close();
                    }

                    // If supplierId is null, add a new supplier then get the id of it
                    // ** This code is not implemented yet **
                }

                // Use query to insert product
                query = "INSERT INTO PRODUCT(barcode, name, category_id, supplier_id, brand, quantity, cost_price, selling_price, image) " +
                    "VALUES(@barcode, @name, @categoryId, @supplierId, @brand, @quantity, @costPrice, @sellingPrice, @image) RETURNING product_id;";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("barcode", entity.Barcode == null ? DBNull.Value : entity.Barcode);
                    cmd.Parameters.AddWithValue("name", entity.Name);
                    cmd.Parameters.AddWithValue("categoryId", categoryId == null ? DBNull.Value : categoryId);
                    cmd.Parameters.AddWithValue("supplierId", supplierId == null ? DBNull.Value : supplierId);
                    cmd.Parameters.AddWithValue("brand", entity.Brand == null ? DBNull.Value : entity.Brand);
                    cmd.Parameters.AddWithValue("quantity", entity.Quantity == null ? DBNull.Value : entity.Quantity);
                    cmd.Parameters.AddWithValue("costPrice", entity.CostPrice == null ? DBNull.Value : entity.CostPrice);
                    cmd.Parameters.AddWithValue("sellingPrice", entity.SellingPrice == null ? DBNull.Value : entity.SellingPrice);
                    cmd.Parameters.AddWithValue("image", entity.Image == null ? DBNull.Value : entity.Image);

                    _connection.Open();
                    int productId = (int)cmd.ExecuteScalar();
                    entity.Id = productId;
                    products.Add(entity);
                    _connection.Close();
                }
            }

            /// <summary>
            /// Deletes the product with the specified id from the database and the cache.
            /// </summary>
            /// <param name="id">The id of the product to delete.</param>
            /// <returns>Nothing.</returns>
            public void Delete(int id)
            {
                string query = "UPDATE PRODUCT SET deleted = TRUE WHERE product_id = @id";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("id", id);

                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    _connection.Close();
                }

                var productToRemove = products.FirstOrDefault(p => p.Id == id);
                if (productToRemove != null)
                {
                    products.Remove(productToRemove);
                }
            }

            /// <summary>
            /// Retrieves all products from the database.
            /// </summary>
            /// <returns>
            /// A list of all products in the database.
            /// </returns>
            public IEnumerable<Product> GetAll()
            {
                if (products.Count == 0)
                {
                    string query = "SELECT p.product_id, p.barcode, p.name, c.name, s.name, p.brand, p.quantity, p.cost_price, p.selling_price, p.image " +
                        "FROM PRODUCT p " +
                        "LEFT JOIN CATEGORY c ON c.category_id = p.category_id " +
                        "LEFT JOIN SUPPLIER s ON s.supplier_id = p.supplier_id " +
                        "WHERE p.deleted = @deleted";
                    using (var cmd = new NpgsqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("deleted", false);
                        _connection.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product product = new Product();
                                product.Id = reader.GetInt32(0);
                                product.Barcode = reader.IsDBNull(1) ? null : reader.GetString(1);
                                product.Name = reader.GetString(2);
                                product.Category = reader.IsDBNull(3) ? null : reader.GetString(3);
                                product.Supplier = reader.IsDBNull(4) ? null : reader.GetString(4);
                                product.Brand = reader.IsDBNull(5) ? null : reader.GetString(5);
                                product.Quantity = reader.IsDBNull(6) ? null : reader.GetInt32(6);
                                product.CostPrice = reader.IsDBNull(7) ? null : reader.GetInt32(7);
                                product.SellingPrice = reader.IsDBNull(8) ? null : reader.GetInt32(8);
                                product.Image = reader.IsDBNull(9) ? null : reader.GetString(9);
                                products.Add(product);
                            }
                        }
                        _connection.Close();
                    }
                }
                return products;
            }

            /// <summary>
            /// Retrieves a product from the database by its ID.
            /// </summary>
            /// <param name="id">The ID of the product to retrieve.</param>
            /// <returns>The product with the specified ID or null if not found.</returns>
            public Product? GetById(int id)
            {
                if (products.Count == 0)
                {
                    GetAll();
                }
                return products.Find(product => product.Id == id);
            }

            /// <summary>
            /// Updates an existing product in the database.
            /// </summary>
            /// 
            /// <param name="entity">The product to update.</param>
            /// <returns>Nothing.</returns>
            public void Update(Product entity)
            {
                string query;
                // Find category id
                int? categoryId = null;
                if (entity.Category != null)
                {
                    query = "SELECT category_id FROM CATEGORY WHERE name = @name";

                    using (var cmd = new NpgsqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("name", entity.Category != null ? entity.Category : DBNull.Value);
                        _connection.Open();
                        categoryId = (int?)cmd.ExecuteScalar();
                        _connection.Close();

                        // If category id is null, add a new category then get the id of it
                        if (categoryId == null)
                        {
                            PostgresCategoryRepository catRepo = PostgresCategoryRepository.GetInstance();
                            Category cat = new Category() { Name = entity.Category };
                            catRepo.Create(cat);
                            categoryId = cat.Id;
                        }
                    }
                }

                // Find supplier id
                int? supplierId = null;
                if (entity.Supplier != null)
                {
                    query = "SELECT supplier_id FROM SUPPLIER WHERE name = @name";

                    using (var cmd = new NpgsqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("name", entity.Supplier != null ? entity.Supplier : DBNull.Value);
                        _connection.Open();
                        supplierId = (int?)cmd.ExecuteScalar();
                        _connection.Close();
                    }

                    // If supplier id is null, add a new supplier then get the id of it
                    // ** This code is not implemented yet **
                }

                // Update product
                query = "UPDATE PRODUCT SET barcode = @barcode, name = @name, category_id = @categoryId, supplier_id = @supplierId, " +
                       "brand = @brand, quantity = @quantity, cost_price = @costPrice, selling_price = @sellingPrice, image = @image " +
                       "WHERE product_id = @id";

                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("barcode", entity.Barcode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("name", entity.Name);
                    cmd.Parameters.AddWithValue("categoryId", categoryId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("supplierId", supplierId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("brand", entity.Brand ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("quantity", entity.Quantity ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("costPrice", entity.CostPrice ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("sellingPrice", entity.SellingPrice ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("image", entity.Image ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("id", entity.Id);

                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    _connection.Close();
                }

                var existingProduct = products.FirstOrDefault(p => p.Id == entity.Id);
                if (existingProduct != null)
                {
                    products.Remove(existingProduct);
                    products.Add(entity);
                }
            }
        }
        
        /// <summary>
        /// Represents a repository for managing customers in the Point of Sale system.
        /// This class implements the <see cref="IRepository{Customer}"/> interface and provides 
        /// methods for creating, reading, updating, and deleting customers in the database.
        /// </summary>
        public class PostgresCustomerRepository : IRepository<Customer>
        {
            List<Customer> customers = new List<Customer>();
            private NpgsqlConnection _connection;

            // singleton instance
            private static PostgresCustomerRepository? _instance = null;

            /// <summary>
            /// Initializes a new instance of the <see cref="PostgresCustomerRepository"/> class.
            /// Retrieves all customers from the database.
            /// </summary>
            /// <param name="connection">The database connection to use.</param>
            /// <returns>This method does not return a value.</returns>
            private PostgresCustomerRepository(NpgsqlConnection connection)
            {
                _connection = connection;
                GetAll();
            }

            /// <summary>
            /// Retrieves the singleton instance of the <see cref="PostgresCustomerRepository"/> class.
            /// </summary>
            /// <returns>
            /// The singleton instance of the <see cref="PostgresCustomerRepository"/> class.
            /// </returns>
            public static PostgresCustomerRepository GetInstance()
            {
                if (_instance == null)
                {
                    _instance = new PostgresCustomerRepository(new NpgsqlConnection(Configuration.CONNECTION_STRING));
                }
                return _instance;
            }

            /// <summary>
            /// Deletes the customer with the specified id from the database and the cache.
            /// </summary>
            /// <param name="id">The id of the customer to delete.</param>
            /// <returns>Nothing.</returns>
            public void Delete(int id)
            {
                string query = "UPDATE CUSTOMER SET deleted = TRUE WHERE customer_id = @id";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    _connection.Close();
                }

                var customerToRemove = customers.FirstOrDefault(c => c.Id == id);
                if (customerToRemove is not null)
                {
                    customers.Remove(customerToRemove);
                }
            }

            /// <summary>
            /// Creates a new customer in the database.
            /// </summary>
            /// <param name="entity">The customer to create.</param>
            /// <returns>Nothing.</returns>
            public void Create(Customer entity)
            {
                string query = "INSERT INTO CUSTOMER(name, phone_number, address, birthday, gender) VALUES(@name, @phone_number, @address, @birthday, @gender) RETURNING customer_id;";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("name", entity.Name);
                    cmd.Parameters.AddWithValue("phone_number", entity.PhoneNumber);
                    cmd.Parameters.AddWithValue("address", entity.Address);
                    cmd.Parameters.AddWithValue("birthday", entity.Birthday);
                    cmd.Parameters.AddWithValue("gender", entity.Gender);

                    _connection.Open();
                    int customerId = (int)cmd.ExecuteScalar();
                    entity.Id = customerId;
                    customers.Add(entity);
                    _connection.Close();
                }
            }

            /// <summary>
            /// Retrieves all customers from the database.
            /// </summary>
            /// <returns>
            /// A list of all customers in the database.
            /// </returns>
            public IEnumerable<Customer> GetAll()
            {
                if (customers.Count == 0)
                {
                    string query = "SELECT * FROM CUSTOMER WHERE deleted = @deleted";
                    _connection.Open();
                    using (var cmd = new NpgsqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("deleted", false);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Customer customer = new Customer();
                                customer.Id = reader.GetInt32(0);
                                customer.Name = reader.GetString(1);
                                customer.PhoneNumber = reader.IsDBNull(2) ? null : reader.GetString(2);
                                customer.Address = reader.IsDBNull(3) ? null : reader.GetString(3);
                                customer.Birthday = reader.IsDBNull(4) ? null : reader.GetDateTime(4);
                                customer.Gender = reader.IsDBNull(5) ? null : reader.GetString(5);
                                customers.Add(customer);
                            }
                        }
                    }
                    _connection.Close();
                }
                return customers;
            }

            /// <summary>
            /// Retrieves a customer from the database by its ID.
            /// </summary>
            /// <param name="id">The ID of the customer to retrieve.</param>
            /// <returns>The customer with the specified ID or null if not found.</returns>
            public Customer GetById(int id)
            {
                Customer customer = null;
                if (customers.Count == 0)
                {
                    GetAll();
                }
                customer = customers.Find(c => c.Id == id);
                return customer;
            }
            
            /// <summary>
            /// Updates an existing customer in the database.
            /// </summary>
            /// <param name="entity">The customer to update.</param>
            /// <returns>Nothing.</returns>
            public void Update(Customer entity)
            {
                string query = "UPDATE CUSTOMER " +
                    "SET name = @name, phone_number = @phone_number," +
                    "address = @address, birthday = @birthday, gender = @gender " +
                    "WHERE customer_id = @id";

                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("name", entity.Name);
                    cmd.Parameters.AddWithValue("phone_number", entity.PhoneNumber ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("address", entity.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("birthday", entity.Birthday ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("gender", entity.Gender ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("id", entity.Id);

                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    _connection.Close();
                }

                var existingProduct = customers.FirstOrDefault(c => c.Id == entity.Id);
                if (existingProduct != null)
                {
                    customers.Remove(existingProduct);
                    customers.Add(entity);
                }
            }
        }
        
        /// <summary>
        /// Represents a repository for managing orders in the Point of Sale system.
        /// This class implements the <see cref="IRepository{Order}"/> interface and provides 
        /// methods for creating, reading, updating, and deleting orders in the database.
        /// </summary>
        public class PostgresOrderRepository : IRepository<Order>
        {
            List<Order> orders = new List<Order>();
            private NpgsqlConnection _connection;
            // singleton instance
            private static PostgresOrderRepository? _instance = null;

            /// <summary>
            /// Initializes a new instance of the <see cref="PostgresOrderRepository"/> class.
            /// Retrieves all orders from the database.
            /// </summary>
            /// <param name="connection">The database connection to use.</param>
            /// <returns>This method does not return a value.</returns>
            private PostgresOrderRepository(NpgsqlConnection connection)
            {
                _connection = connection;
                GetAll();
            }

            /// <summary>
            /// Retrieves the singleton instance of the <see cref="PostgresOrderRepository"/> class.
            /// </summary>
            /// <returns>
            /// The singleton instance of the <see cref="PostgresOrderRepository"/> class.
            /// </returns>
            public static PostgresOrderRepository GetInstance()
            {
                if (_instance == null)
                {
                    _instance = new PostgresOrderRepository(new NpgsqlConnection(Configuration.CONNECTION_STRING));
                }
                return _instance;
            }

            /// <summary>
            /// Creates a new order in the database.
            /// </summary>
            /// <param name="entity">The order to create.</param>
            /// <returns>Nothing.</returns>
            public void Create(Order entity)
            {
                string query = "INSERT INTO \"order\"(customer_id, total_price, discount, paid, order_time) VALUES(@customerId, @totalPrice, @discount, @isPaid, @orderTime) RETURNING order_id;";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("customerId", entity.CustomerId);
                    cmd.Parameters.AddWithValue("totalPrice", entity.TotalPrice);
                    cmd.Parameters.AddWithValue("discount", entity.Discount);
                    cmd.Parameters.AddWithValue("isPaid", entity.IsPaid);
                    cmd.Parameters.AddWithValue("orderTime", entity.OrderTime);

                    _connection.Open();
                    int orderId = (int)cmd.ExecuteScalar();
                    entity.Id = orderId;
                    orders.Add(entity);
                    _connection.Close();
                }
            }

            /// <summary>
            /// Deletes the order with the specified id from the database and the cache.
            /// </summary>
            /// <param name="id">The id of the order to delete.</param>
            /// <returns>Nothing.</returns>
            public void Delete(int id)
            {
                string query = "UPDATE \"order\" SET deleted = TRUE WHERE order_id = @id";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("id", id);

                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    _connection.Close();
                }

                var orderToRemove = orders.FirstOrDefault(o => o.Id == id);
                if (orderToRemove != null)
                {
                    orders.Remove(orderToRemove);
                }
            }

            /// <summary>
            /// Retrieves all orders from the database.
            /// </summary>
            /// <returns>
            /// A list of all orders in the database.
            /// </returns>
            public IEnumerable<Order> GetAll()
            {
                if (orders.Count == 0)
                {
                    string query = "SELECT * FROM \"order\" WHERE deleted = @deleted";
                    using (var cmd = new NpgsqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("deleted", false);
                        _connection.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Order order = new Order();
                                order.Id = reader.GetInt32(0);
                                order.CustomerId = reader.IsDBNull(1) ? null : reader.GetInt32(1);
                                order.TotalPrice = reader.IsDBNull(2) ? null : reader.GetInt32(2);
                                order.Discount = reader.IsDBNull(3) ? null : reader.GetInt32(3);
                                order.IsPaid = reader.IsDBNull(4) ? null : reader.GetBoolean(4);
                                order.OrderTime = reader.IsDBNull(5) ? null : reader.GetDateTime(5);
                                orders.Add(order);
                            }
                        }
                        _connection.Close();
                    }
                }
                return orders;
            }

            /// <summary>
            /// Retrieves an order from the database by its ID.
            /// </summary>
            /// <param name="id">The ID of the order to retrieve.</param>
            /// <returns>The order with the specified ID or null if not found.</returns>
            public Order GetById(int id)
            {
                if (orders.Count == 0)
                {
                    GetAll();
                }
                return orders.Find(order => order.Id == id);
            }

            /// <summary>
            /// Updates an existing order in the database.
            /// </summary>
            /// <param name="entity">The order to update.</param>
            /// <returns>Nothing.</returns>
            public void Update(Order entity)
            {
                string query = "UPDATE \"order\" SET customer_id = @customerId, total_price = @totalPrice, discount = @discount, paid = @isPaid, order_time = @orderTime WHERE order_id = @id";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("customerId", entity.CustomerId);
                    cmd.Parameters.AddWithValue("totalPrice", entity.TotalPrice);
                    cmd.Parameters.AddWithValue("discount", entity.Discount);
                    cmd.Parameters.AddWithValue("isPaid", entity.IsPaid);
                    cmd.Parameters.AddWithValue("orderTime", entity.OrderTime);
                    cmd.Parameters.AddWithValue("id", entity.Id);

                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    _connection.Close();
                }

                var existingOrder = orders.FirstOrDefault(o => o.Id == entity.Id);
                if (existingOrder != null)
                {
                    orders.Remove(existingOrder);
                    orders.Add(entity);
                }
            }
        }

        /// <summary>
        /// Represents a repository for managing order details in the Point of Sale system.
        /// This class implements the <see cref="IRepository{OrderDetail}"/> interface and provides 
        /// methods for creating, reading, updating, and deleting order details in the database.
        /// </summary>
        public class PostgresOrderDetailRepository : IRepository<OrderDetail>
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            private NpgsqlConnection _connection;

            // singleton instance
            private static PostgresOrderDetailRepository? _instance = null;

            /// <summary>
            /// Initializes a new instance of the <see cref="PostgresOrderDetailRepository"/> class.
            /// Retrieves all order details from the database.
            /// </summary>
            /// <param name="connection">The database connection to use.</param>
            /// <returns>This method does not return a value.</returns>
            private PostgresOrderDetailRepository(NpgsqlConnection connection)
            {
                _connection = connection;
                GetAll();
            }

            /// <summary>
            /// Retrieves the singleton instance of the <see cref="PostgresOrderDetailRepository"/> class.
            /// </summary>
            /// <returns>
            /// The singleton instance of the <see cref="PostgresOrderDetailRepository"/> class.
            /// </returns>
            public static PostgresOrderDetailRepository GetInstance()
            {
                if(_instance == null)
                {
                    _instance = new PostgresOrderDetailRepository(new NpgsqlConnection(Configuration.CONNECTION_STRING));
                }
                return _instance;
            }

            /// <summary>
            /// Creates a new order detail in the database.
            /// </summary>
            /// <param name="entity">The order detail to create.</param>
            /// <returns>Nothing.</returns>
            public void Create(OrderDetail entity)
            {
                // Check if orderId and productId are not null
                if (entity.OrderId == null || entity.ProductId == null)
                {
                    return;
                }
                string query = "INSERT INTO DETAIL(order_id, product_id, count) VALUES(@orderId, @productId, @count);";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("orderId", entity.OrderId);
                    cmd.Parameters.AddWithValue("productId", entity.ProductId);
                    cmd.Parameters.AddWithValue("count", entity.Quantity);
                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    _connection.Close();
                }
                orderDetails.Add(entity);
            }

            /// <summary>
            /// Deletes the order details associated with the specified order ID from the database.
            /// </summary>
            /// <param name="orderId">The ID of the order to delete.</param>
            /// <returns>Nothing.</returns>
            public void Delete(int orderId)
            {
                string query = "UPDATE PRODUCT SET deleted = TRUE WHERE order_id = @orderId";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("orderId", orderId);
                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    _connection.Close();
                }

                // Find order details by orderId and remove them from the list
                var orderDetailsToRemove = orderDetails.Where(od => od.OrderId == orderId).ToList();
                foreach(var orderDetail in orderDetailsToRemove)
                {
                    orderDetails.Remove(orderDetail);
                }
            }

            /// <summary>
            /// Retrieves all order details from the database.
            /// </summary>
            /// <returns>
            /// A list of all order details in the database.
            /// </returns>
            public IEnumerable<OrderDetail> GetAll()
            {
                if(orderDetails.Count == 0)
                {
                    string query = "SELECT d.order_id, d.product_id, d.count " +
                        "FROM DETAIL d " +
                        "WHERE d.deleted = @deleted";
                        
                    using (var cmd = new NpgsqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("deleted", false);
                        _connection.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                OrderDetail orderDetail = new OrderDetail();
                                orderDetail.OrderId = reader.GetInt32(0);
                                orderDetail.ProductId = reader.GetInt32(1);
                                orderDetail.Quantity = reader.GetInt32(2);
                                orderDetails.Add(orderDetail);
                            }
                        }
                        _connection.Close();
                    }
                }
                return orderDetails;
            }

            /// <summary>
            /// Retrieves an order detail from the database by its ID.
            /// </summary>
            /// <param name="id">The ID of the order detail to retrieve.</param>
            /// <returns>The order detail with the specified ID or null if not found.</returns>
            public OrderDetail GetById(int id)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Retrieves all order details associated with the specified order ID from the database.
            /// </summary>
            /// <param name="orderId">The ID of the order to retrieve.</param>
            /// <returns>A list of order details associated with the specified order ID.</returns>
            public IEnumerable<OrderDetail> GetByOrderId(int orderId)
            {
                if(orderDetails.Count == 0)
                {
                    GetAll();
                }
                return orderDetails.Where(od => od.OrderId == orderId);
            }

            /// <summary>
            /// Updates an order detail in the database.
            /// </summary>
            /// <param name="entity">The order detail to update.</param>
            /// <returns>Nothing.</returns>
            public void Update(OrderDetail entity)
            {
                if(entity.OrderId == null || entity.ProductId == null)
                {
                    return;
                }
                string query = "UPDATE DETAIL SET count = @count WHERE order_id = @orderId AND product_id = @productId";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("count", entity.Quantity != null ? entity.Quantity : DBNull.Value);
                    cmd.Parameters.AddWithValue("orderId", entity.OrderId);
                    cmd.Parameters.AddWithValue("productId", entity.ProductId);
                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    _connection.Close();
                }

                // Update the order detail in the list
                var existingOrderDetail = orderDetails.FirstOrDefault(od => od.OrderId == entity.OrderId && od.ProductId == entity.ProductId);
                if (existingOrderDetail != null)
                {
                    existingOrderDetail.Quantity = entity.Quantity;
                }
            }
        }

        /// <summary>
        /// Represents a repository for managing payment methods in the Point of Sale system.
        /// This class implements the <see cref="IRepository{PaymentMethod}"/> interface and provides 
        /// methods for creating, reading, updating, and deleting payment methods in the database.
        /// </summary>
        public class PostgresPaymentMethodRepository : IRepository<PaymentMethod>
        {
            List<PaymentMethod> paymentMethods = new List<PaymentMethod>();
            private NpgsqlConnection _connection;

            // singleton instance
            private static PostgresPaymentMethodRepository? _instance = null;

            /// <summary>
            /// Initializes a new instance of the <see cref="PostgresPaymentMethodRepository"/> class.
            /// Retrieves all payment methods from the database.
            /// </summary>
            /// <param name="connection">The database connection to use.</param>
            /// <returns>This method does not return a value.</returns>
            private PostgresPaymentMethodRepository(NpgsqlConnection connection)
            {
                _connection = connection;
                GetAll();
            }

            /// <summary>
            /// Retrieves all payment methods from the database.
            /// </summary>
            /// <returns>
            /// A list of all payment methods in the database.
            /// </returns>
            public static PostgresPaymentMethodRepository GetInstance()
            {
                if(_instance == null)
                {
                    _instance = new PostgresPaymentMethodRepository(new NpgsqlConnection(Configuration.CONNECTION_STRING));
                }
                return _instance;
            }

            /// <summary>
            /// Retrieves all payment methods from the database.
            /// </summary>
            /// <returns>
            /// A list of all payment methods in the database.
            /// </returns>
            public IEnumerable<PaymentMethod> GetAll()
            {
                if(paymentMethods.Count == 0)
                {
                    string query = "SELECT pm.id, pm.type, pm.account_number, pm.bank_name, pm.account_holder, pm.phone_number, is_default " +
                        "FROM PAYMENT_METHOD pm " +
                        "WHERE pm.deleted = @deleted";
                    using (var cmd = new NpgsqlCommand(query, _connection))
                    {
                        cmd.Parameters.AddWithValue("deleted", false);
                        _connection.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PaymentMethod paymentMethod = new PaymentMethod();
                                paymentMethod.Id = reader.GetInt32(0);
                                paymentMethod.Type = reader.GetString(1);
                                paymentMethod.AccountNumber = reader.IsDBNull(2) ? null : reader.GetString(2);
                                paymentMethod.BankName = reader.IsDBNull(3) ? null : reader.GetString(3);
                                paymentMethod.AccountHolder = reader.IsDBNull(4) ? null : reader.GetString(4);
                                paymentMethod.PhoneNumber = reader.IsDBNull(5) ? null : reader.GetString(5);
                                paymentMethod.IsDefault = reader.GetBoolean(6);
                                paymentMethods.Add(paymentMethod);
                            }
                        }
                        _connection.Close();
                    }
                }
                return paymentMethods;
            }

            /// <summary>
            /// Retrieves a payment method from the database by its ID.
            /// </summary>
            /// <param name="id">The ID of the payment method to retrieve.</param>
            /// <returns>The payment method with the specified ID or null if not found.</returns>
            public PaymentMethod? GetById(int id)
            {
                if(paymentMethods.Count == 0)
                {
                    GetAll();
                }
                return paymentMethods.Find(pm => pm.Id == id);
            }

            /// <summary>
            /// Creates a new payment method in the database.
            /// </summary>
            /// <param name="entity">The payment method to create.</param>
            /// <returns>Nothing.</returns>
            public void Create(PaymentMethod entity)
            {
                string query = "INSERT INTO PAYMENT_METHOD(type, account_number, bank_name, account_holder, phone_number, is_default) " +
                    "VALUES(@type, @accountNumber, @bankName, @accountHolder, @phoneNumber, @isDefault) RETURNING id;";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("type", entity.Type);
                    cmd.Parameters.AddWithValue("accountNumber", entity.AccountNumber == null ? DBNull.Value : entity.AccountNumber);
                    cmd.Parameters.AddWithValue("bankName", entity.BankName == null ? DBNull.Value : entity.BankName);
                    cmd.Parameters.AddWithValue("accountHolder", entity.AccountHolder == null ? DBNull.Value : entity.AccountHolder);
                    cmd.Parameters.AddWithValue("phoneNumber", entity.PhoneNumber == null ? DBNull.Value : entity.PhoneNumber);
                    cmd.Parameters.AddWithValue("isDefault", entity.IsDefault);

                    _connection.Open();
                    int paymentMethodId = (int)cmd.ExecuteScalar();
                    entity.Id = paymentMethodId;
                    paymentMethods.Add(entity);
                    _connection.Close();
                }
            }

            /// <summary>
            /// Updates an existing payment method in the database.
            /// </summary>
            /// <param name="entity">The payment method to update.</param>
            /// <returns>Nothing.</returns>
            public void Update(PaymentMethod entity)
            {
                string query = "UPDATE PAYMENT_METHOD " +
                    "SET type = @type, account_number = @accountNumber, bank_name = @bankName, account_holder = @accountHolder, phone_number = @phoneNumber, is_default = @isDefault " +
                    "WHERE id = @id";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("type", entity.Type);
                    cmd.Parameters.AddWithValue("accountNumber", entity.AccountNumber == null ? DBNull.Value : entity.AccountNumber);
                    cmd.Parameters.AddWithValue("bankName", entity.BankName == null ? DBNull.Value : entity.BankName);
                    cmd.Parameters.AddWithValue("accountHolder", entity.AccountHolder == null ? DBNull.Value : entity.AccountHolder);
                    cmd.Parameters.AddWithValue("phoneNumber", entity.PhoneNumber == null ? DBNull.Value : entity.PhoneNumber);
                    cmd.Parameters.AddWithValue("isDefault", entity.IsDefault);
                    cmd.Parameters.AddWithValue("id", entity.Id);
                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    _connection.Close();
                }

                var existingPaymentMethod = paymentMethods.FirstOrDefault(pm => pm.Id == entity.Id);
                if(existingPaymentMethod != null)
                {
                    paymentMethods.Remove(existingPaymentMethod);
                    paymentMethods.Add(entity);
                }
            }

            /// <summary>
            /// Deletes a payment method from the database by its ID.
            /// </summary>
            /// <param name="id">The ID of the payment method to delete.</param>
            /// <returns>Nothing.</returns>
            public void Delete(int id)
            {
                string query = "UPDATE PAYMENT_METHOD SET deleted = TRUE WHERE id = @id";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    _connection.Open();
                    cmd.ExecuteNonQuery();
                    _connection.Close();
                }
                var paymentMethodToRemove = paymentMethods.FirstOrDefault(pm => pm.Id == id);
                if (paymentMethodToRemove != null)
                {
                    paymentMethods.Remove(paymentMethodToRemove);
                }
            }
        }
    }
}
