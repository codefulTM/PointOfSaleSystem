using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using PointOfSaleSystem.Models;

namespace PointOfSaleSystem.Services
{
    public class PostgresDao : IDao
    {
        public PostgresDao()
        {
            Categories = PostgresCategoryRepository.GetInstance();
            Products = PostgresProductRepository.GetInstance();
            Customers = PostgresCustomerRepository.GetInstance();
        }

        public IRepository<Category> Categories { get; set; }
        public IRepository<Product> Products { get; set; }
        public IRepository<Customer> Customers { get; set; }

        public class PostgresCategoryRepository : IRepository<Category>
        {
            List<Category> categories = new List<Category>();
            private NpgsqlConnection _connection;

            // singleton instance
            private static PostgresCategoryRepository? _instance = null;

            private PostgresCategoryRepository(NpgsqlConnection connection)
            {
                _connection = connection;
                GetAll();
            }

            public static PostgresCategoryRepository GetInstance()
            {
                if (_instance == null)
                {
                    _instance = new PostgresCategoryRepository(new NpgsqlConnection(Configuration.CONNECTION_STRING));
                }
                return _instance;
            }

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

            public void Delete(int id)
            {
                throw new NotImplementedException();
            }

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

            public Category GetById(int id)
            {
                throw new NotImplementedException();
            }

            public void Update(Category entity)
            {
                throw new NotImplementedException();
            }
        }
        public class PostgresProductRepository : IRepository<Product>
        {
            List<Product> products = new List<Product>();
            private NpgsqlConnection _connection;

            // singleton instance
            private static PostgresProductRepository _instance = null;

            private PostgresProductRepository(NpgsqlConnection connection)
            {
                _connection = connection;
                GetAll();
            }

            public static PostgresProductRepository GetInstance()
            {
                if (_instance == null)
                {
                    _instance = new PostgresProductRepository(new NpgsqlConnection(Configuration.CONNECTION_STRING));
                }
                return _instance;
            }

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

            public Product? GetById(int id)
            {
                if (products.Count == 0)
                {
                    GetAll();
                }
                return products.Find(product => product.Id == id);
            }

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
        public class PostgresCustomerRepository : IRepository<Customer>
        {
            List<Customer> customers = new List<Customer>();
            private NpgsqlConnection _connection;

            // singleton instance
            private static PostgresCustomerRepository? _instance = null;

            private PostgresCustomerRepository(NpgsqlConnection connection)
            {
                _connection = connection;
                GetAll();
            }

            public static PostgresCustomerRepository GetInstance()
            {
                if (_instance == null)
                {
                    _instance = new PostgresCustomerRepository(new NpgsqlConnection(Configuration.CONNECTION_STRING));
                }
                return _instance;
            }

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
    }
}
