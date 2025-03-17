using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Windows.ApplicationModel.Store;

namespace PointOfSaleSystem.Models
{
    class CustomerRepository : IRepository<Customer>
    {
        List<Customer> customers = new List<Customer>();
        private NpgsqlConnection _connection;

        // singleton instance
        private static CustomerRepository? _instance = null;

        private CustomerRepository(NpgsqlConnection connection)
        {
            _connection = connection;
            GetAll();
        }

        public static CustomerRepository GetInstance()
        {
            if(_instance == null)
            {
                _instance = new CustomerRepository(new NpgsqlConnection(Configuration.CONNECTION_STRING));
            }
            return _instance;
        }

        public void Create(Customer entity)
        {
            throw new NotImplementedException();
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
            if(customerToRemove is not null)
            {
                customers.Remove(customerToRemove);
            }
        }

        public IEnumerable<Customer> GetAll()
        {
            throw new NotImplementedException();
        }

        public Customer GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Customer entity)
        {
            string query = "UPDATE CUSTOMER " +
                "SET name = @name, phone_number = @phone_number," +
                "address = @address, birthday = @birthday, gender = @gender" +
                "WHERE customer_id = @id";

            using (var cmd = new NpgsqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("name", entity.Name);
                cmd.Parameters.AddWithValue("phone_number", entity.PhoneNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("address", entity.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("birthday", entity.Birthday ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("gender", entity.Gender ?? (object)DBNull.Value);

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
