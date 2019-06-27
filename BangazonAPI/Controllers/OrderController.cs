// Author: Billy Mitchell
// The purpose for the Order controller is to define the methods to be used for accessing all tables associated with an order in the BangazonAPI database.
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{   // Setting the default route, api/order 
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _config;
        public OrderController(IConfiguration config)
        {
            _config = config;
        }
        // Getting and setting the connection property through the DefaultConnection in appsettings.json which connects this file to the database
        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: api/Order
        // Method to get all Order entries in the database if no query parameter is inputed
        [HttpGet]
        public async Task<IActionResult> Get(string _include)
        {
            string sql = @"SELECT 
                            o.Id AS OrderId, o.CustomerId, o.PaymentTypeId,
                            op.Id, op.OrderId, op.ProductId,
                            p.Id, p.Price, p.Title, p.Description, p.Quantity, p.ProductTypeId, p.CustomerId,
                            pt.Id, pt.Name,
                            c.Id, c.FirstName, c.LastName,
                            payt.Id, payt.AcctNumber, payt.Name, payt.CustomerId
                            FROM [Order] o
                            JOIN OrderProduct op ON o.Id = op.OrderId
                            JOIN Product p ON p.Id = op.ProductId
                            JOIN ProductType pt ON pt.Id = p.ProductTypeId
                            JOIN Customer c ON c.Id = o.CustomerId
                            JOIN PaymentType payt ON payt.Id = o.PaymentTypeId
                            ";

         
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Order> orders = new List<Order>();
                    List<Product> products = new List<Product>();

                    while (reader.Read())
                    {
                        Order order = new Order()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId"))
                        };
                        Product product = null;                        

                        if (_include == "products")
                        {
                            product = new Product
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                            };
                            products.Add(product);
                        }
                        if (!orders.Any(o => o.Id == order.Id))
                        {
                            orders.Add(order); 
                        }

                        if (_include == "products")
                        {
                            orders.Find(o => o.Id == order.Id).ListOfProducts = products.Where(p => p.CustomerId == order.Id).ToList();
                        }                       

                    }
                    reader.Close();
                    return Ok(orders);
                }
            }
        }

        // GET: api/Order/5
        // Method to get one Order entry in the database if no query parameter is inputed
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            if (!OrderExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            string sql = "SELECT Id AS OrderId, CustomerId, PaymentTypeId FROM [Order] WHERE Id = @id";
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    Order order = null;

                    if (reader.Read())
                    {
                        order = new Order()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId"))
                        };
                    }
                    reader.Close();

                    return Ok(order);
                }
            }
        }

        // POST One Order Entry
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO [Order] (CustomerId, PaymentTypeId)
                        OUTPUT INSERTED.Id
                        VALUES (@CustomerId, @PaymentTypeId)";
                    cmd.Parameters.Add(new SqlParameter("@CustomerId", order.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@PaymentTypeId", order.PaymentTypeId));

                    int NewId = (int)await cmd.ExecuteScalarAsync();
                    order.Id = NewId;
                    return CreatedAtRoute("GetOrder", new { id = NewId }, order);
                }
            }
        }

        // PUT (update) One Order Entry
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Order order)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE [Order]
                                            SET PaymentTypeId = @paymentTypeId                      
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@paymentTypeId", order.PaymentTypeId));
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
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE One Product Type Entry
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM [Order] WHERE Id = @id";
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
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        //This method is used within the get by id, put, and delete methods to make sure the object exists.
        private bool OrderExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id FROM [Order] WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
