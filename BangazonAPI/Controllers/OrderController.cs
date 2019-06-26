// Author: Billy Mitchell
// The purpose fo the Order controller is to define the methods to be used for accessing the orders table in the BangazonAPI database GET PUT POST and DELETE  
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
{   // Setting the default route, api/producttype, 
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
        public async Task<IActionResult> Get()
        {
            string sql = @"SELECT Id AS OrderId, CustomerId, PaymentTypeId FROM [Order]";
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Order> orders = new List<Order>();

                    while (reader.Read())
                    {
                        Order order = new Order()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId"))
                        };
                        orders.Add(order);
                    }
                    reader.Close();
                    return Ok(orders);
                }
            }
        }

        // GET: api/Order/5
        // Method to get one Order entry in the database if no query parameter is inputed
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> Get(int id)
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

        // POST: api/Order
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Order/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
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
