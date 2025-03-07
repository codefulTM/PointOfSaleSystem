using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Windows.ApplicationModel.Store;

namespace PointOfSaleSystem.Models
{
    public class CategoryRepository : IRepository<Category>
    {
        List<Category> categories = new List<Category>();
        private NpgsqlConnection _connection;

        // singleton instance
        private static CategoryRepository? _instance = null;

        private CategoryRepository(NpgsqlConnection connection) 
        {
            _connection = connection;
            GetAll();
        }

        public static CategoryRepository GetInstance()
        {
            if(_instance == null)
            {
                _instance = new CategoryRepository(new NpgsqlConnection(Configuration.CONNECTION_STRING));
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
                int categoryId = (int) cmd.ExecuteScalar();
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
            if(categories.Count == 0)
            {
                string query = "SELECT * FROM CATEGORY WHERE deleted = @deleted";
                _connection.Open();
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("deleted", false);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
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
}
