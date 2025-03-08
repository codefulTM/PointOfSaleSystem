using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Npgsql;
using System.Security.Cryptography;
using System.Diagnostics;

namespace PointOfSaleSystem.Models
{
    public class ProductRepository : IRepository<Product>
    {
        List<Product> products = new List<Product>();
        private NpgsqlConnection _connection;

        // singleton instance
        private static ProductRepository _instance = null;

        private ProductRepository(NpgsqlConnection connection)
        {
            _connection = connection;
            GetAll();
        }

        public static ProductRepository GetInstance() 
        {    
            if(_instance == null)
            {
                _instance = new ProductRepository(new NpgsqlConnection(Configuration.CONNECTION_STRING));
            }
            return _instance;
        }

        public void Create(Product entity)
        {
            string query;

            // Find category id
            int? categoryId = null;
            if(entity.Category != null)
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
                    CategoryRepository catRepo = CategoryRepository.GetInstance();
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
                int productId = (int) cmd.ExecuteScalar();
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
            if(products.Count == 0)
            {
                string query = "SELECT p.product_id, p.barcode, p.name, c.name, s.name, p.brand, p.quantity, p.cost_price, p.selling_price, p.image " +
                    "FROM PRODUCT p " +
                    "LEFT JOIN CATEGORY c ON c.category_id = p.category_id " +
                    "LEFT JOIN SUPPLIER s ON s.supplier_id = p.supplier_id " +
                    "WHERE p.deleted = @deleted";
                using(var cmd = new NpgsqlCommand(query, _connection))
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
            if(products.Count == 0)
            {
                string query = "SELECT p.product_id, p.barcode, p.name, c.name, s.name, p.brand, p.quantity, p.cost_price, p.selling_price, p.image " +
                    "FROM PRODUCT p " +
                    "LEFT JOIN CATEGORY c ON c.category_id = p.category_id " +
                    "LEFT JOIN SUPPLIER s ON s.supplier_id = p.supplier_id " +
                    "WHERE p.id = @id AND p.deleted = @deleted";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("deleted", false);
                    cmd.Parameters.AddWithValue("id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
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

                            return product;
                        }
                        return null;
                    }
                }
            }
            else
            {
                return products.Find(product => product.Id == id);
            }
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
                        CategoryRepository catRepo = CategoryRepository.GetInstance();
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
}
