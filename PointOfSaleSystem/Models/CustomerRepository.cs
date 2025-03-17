using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace PointOfSaleSystem.Models
{
    public class CustomerRepository : IRepository<Customer>
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
            if (_instance == null)
            {
                _instance = new CustomerRepository(new NpgsqlConnection(Configuration.CONNECTION_STRING));
            }
            return _instance;
        }
        public void Create(Customer entity)
        {
            string query = "INSERT INTO CUSTOMER(name, phone_number, address, birthday, gender) VALUES(@name, @phone_number, @address, @birthday, @gender) RETURNING customer_id;";
            using (var cmd = new NpgsqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("name", entity.Name);
                cmd.Parameters.AddWithValue("phone_number", entity.Phone);
                cmd.Parameters.AddWithValue("address", entity.Address);
                cmd.Parameters.AddWithValue("birthday", entity.DateOfBirth);
                cmd.Parameters.AddWithValue("gender", entity.Gender);

                _connection.Open();
                int customerId = (int)cmd.ExecuteScalar();
                entity.Id = customerId;
                customers.Add(entity);
                _connection.Close();
            }
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
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
                            customer.Phone = reader.GetString(2);
                            customer.Address = reader.GetString(3);
                            customer.DateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(4));
                            customer.Gender = reader.GetString(5);
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
            else
            {
                customer = customers.Find(c => c.Id == id);
            }
            return customer;
        }

        public void Update(Customer entity)
        {
            throw new NotImplementedException();
        }
    }
}
