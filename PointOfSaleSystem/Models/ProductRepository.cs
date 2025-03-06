using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Npgsql;
using System.Security.Cryptography;

namespace PointOfSaleSystem.Models
{
    public class ProductRepository : IRepository<Product>
    {
        List<Product> products = new List<Product>();
        private NpgsqlConnection _connection;

        public ProductRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public void Create(Product entity)
        {
            string query = "INSERT INTO PRODUCT(barcode, name, category_id, supplier_id, brand, quantity, cost_price, selling_price, image) " +
                "VALUES(@barcode, @name, @categoryId, @supplierId, @brand, @quantity, @costPrice, @sellingPrice, @image) RETURNING product_id;";
            using (var cmd = new NpgsqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("barcode", entity.Barcode == null ? DBNull.Value : entity.Barcode);
                cmd.Parameters.AddWithValue("name", entity.Name);
                cmd.Parameters.AddWithValue("categoryId", entity.CategoryId == null ? DBNull.Value : entity.CategoryId);
                cmd.Parameters.AddWithValue("supplierId", entity.SupplierId == null ? DBNull.Value : entity.SupplierId);
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
            string query = "DELETE FROM PRODUCT WHERE product_id = @id";
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
                string query = "SELECT * FROM PRODUCT";
                using(var cmd = new NpgsqlCommand(query, _connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product();
                            product.Id = reader.GetInt32(0);
                            product.Barcode = reader.IsDBNull(1) ? null : reader.GetString(1);
                            product.Name = reader.GetString(2);
                            product.CategoryId = reader.IsDBNull(3) ? null : reader.GetInt32(3);
                            product.SupplierId = reader.IsDBNull(4) ? null : reader.GetInt32(4);
                            product.Brand = reader.IsDBNull(5) ? null : reader.GetString(5);
                            product.Quantity = reader.IsDBNull(6) ? null : reader.GetInt32(6);
                            product.CostPrice = reader.IsDBNull(7) ? null : reader.GetInt32(7);
                            product.SellingPrice = reader.IsDBNull(8) ? null : reader.GetInt32(8);
                            product.Image = reader.IsDBNull(9) ? null : reader.GetString(9);
                            products.Add(product);
                        }
                    }
                }
            }
            return products;
        }

        public Product? GetById(int id)
        {
            if(products.Count == 0)
            {
                string query = "SELECT * FROM PRODUCT WHERE id = @id";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Product product = new Product();
                            product.Id = reader.GetInt32(0);
                            product.Barcode = reader.IsDBNull(1) ? null : reader.GetString(1);
                            product.Name = reader.GetString(2);
                            product.CategoryId = reader.IsDBNull(3) ? null : reader.GetInt32(3);
                            product.SupplierId = reader.IsDBNull(4) ? null : reader.GetInt32(4);
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
            string query = "UPDATE PRODUCT SET barcode = @barcode, name = @name, category_id = @categoryId, supplier_id = @supplierId, " +
                   "brand = @brand, quantity = @quantity, cost_price = @costPrice, selling_price = @sellingPrice, image = @image " +
                   "WHERE product_id = @id";

            using (var cmd = new NpgsqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("barcode", entity.Barcode ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("name", entity.Name);
                cmd.Parameters.AddWithValue("categoryId", entity.CategoryId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("supplierId", entity.SupplierId ?? (object)DBNull.Value);
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
