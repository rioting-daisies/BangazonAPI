// Author: Chris Morgan

// The purpose of the customer controller is to define normal database interactions (Get, Put, Post, Delete) plus any other requirements made by the client. 


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

//User should be able to GET a list of customers, and GET a single customer.

//If the query string parameter of? _include = products is provided, then any products that the customer is selling should be included in the response.

//If the query string parameter of? _include = payments is provided, then any payment types that the customer has used to pay for an order should be included in the response.

//If the query string parameter of q is provided when querying the list of customers, then any customer that has property value that matches the pattern should be returned.

//If /customers? q = mic is requested, then any customer whose first name is Michelle, or Michael, or Domicio should be returned.Any customer whose last name is Michaelangelo, or Omici, Dibromic should be returned. Every property of the customer object should be checked for a match.

namespace BangazonAPI.Controllers
{
    // The route is setup to be the controller name which is Customer, the route for these functions is api/Customer

    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        // We establish the Connection property by referencing the DefaultConnection value in the appsettings.json file. This allows us to connect to BangazonAPI database
        private readonly IConfiguration _config;

        public CustomerController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET api/Customer
        // This method returns a list of all Customers in the database if no query paramter is input.
        [HttpGet]
        public async Task<IActionResult> Get(string q)
        {

            string sql = @"SELECT c.Id AS CustomerId, c.FirstName, c.LastName FROM Customer c";

            if (q != null)
            {
                sql = $@"{sql} WHERE (
                    c.LastName LIKE @q
                    OR c.FirstName LIKE @q
                    )
                    ";

            }
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    if (q != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@q", $"%{q}%"));

                    }

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Customer> customers = new List<Customer>();
                    while (reader.Read())
                    {
                        Customer customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            // You might have more columns
                        };

                        customers.Add(customer);
                    }

                    reader.Close();

                    // If no customers are found (especially matching the 'q' query string paramater), the get will return a 404

                    if(customers.Count == 0)
                    {
                        return new StatusCodeResult(StatusCodes.Status404NotFound);
                    }

                    return Ok(customers);
                }
            }
        }

        // GET api/Customer/5
        // This is the Get Customer by Id method that returns a single customer whose id = parameter Id obtained by the route. If no customer is found, a 404 status code is returned. 
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            string sql = @"SELECT c.Id AS CustomerId, c.FirstName, c.LastName FROM Customer c WHERE c.Id = @id";

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Customer customer = null;
                    if (reader.Read())
                    {
                        customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            // You might have more columns
                        };
                    }

                    reader.Close();

                    if(customer == null)
                    {
                        return new StatusCodeResult(StatusCodes.Status404NotFound);
                    }

                    return Ok(customer);
                }
            }
        }

        // POST api/Customer
        // The Post method inserts a new Customer item into the database and then returns the new customer instance
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO Customer (FirstName, LastName)
                        OUTPUT INSERTED.Id
                        VALUES (@FirstName, @LastName)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", customer.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", customer.LastName));

                    customer.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtAction("Get", new { id = customer.Id }, customer);
                }
            }
        }

        // PUT api/Customer/5
        // The put method is used to modify a customer item in the database. It accepts 2 parameters, an Id of the customer to modify, and then a Customer object with the updated values that will be passed in to modify if the customer in the database is found.
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute]int id, [FromBody] Customer customer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Customer
                            SET FirstName = @FirstName,
                                LastName = @LastName
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@FirstName", customer.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@LastName", customer.LastName));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/Customer/5
        // The Delete method is used to delete a customer from the database. It accepts one parameter which is used to find the customer in the database to delete. If the delete is successful, it will return a 204 code. If a customer with this Id is not found, it will return a 404 status code.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
           
                try
                {
                    using (SqlConnection conn = Connection)
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = @"DELETE FROM Customer WHERE Id = @id";
                            cmd.Parameters.Add(new SqlParameter("@id", id));

                            int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            if (rowsAffected > 0)
                            {
                                return new StatusCodeResult(StatusCodes.Status204NoContent);
                            }
                            throw new Exception("No rows affected");
                        }
                    }
                }
                catch (Exception)
                {
                    if (!CustomerExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            
        }

        // The CustomerExists method returns a boolean value that represents whether or not a customer with the Id passed in from the parameter exists. This method is used throughout this file to minimize code.

        private bool CustomerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM Customer WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
