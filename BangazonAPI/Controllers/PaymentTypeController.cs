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
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTypeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public PaymentTypeController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: api/PaymentType
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string sql = "SELECT Id, AcctNumber, Name, CustomerId FROM PaymentType";
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<PaymentType> paymentTypes = new List<PaymentType>();

                    while (reader.Read())
                    {
                        PaymentType paymentType = new PaymentType()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                        };
                        paymentTypes.Add(paymentType);
                    }
                    reader.Close();
                    return Ok(paymentTypes);
                }
            }
        }

        // GET: api/PaymentType/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/PaymentType
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/PaymentType/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
