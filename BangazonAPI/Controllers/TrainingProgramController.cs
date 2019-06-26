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
    public class TrainingProgramController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TrainingProgramController(IConfiguration config)
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

        // GET: api/TrainingProgram
        [HttpGet]
        public async Task<IActionResult> Get(string completed)
        {
            string sql = @"SELECT tp.Id, tp.Name, tp.StartDate, tp.EndDate, tp.MaxAttendees, e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor FROM TrainingProgram tp
                           JOIN EmployeeTraining et ON et.TrainingProgramId = tp.Id
                           JOIN Employee e ON e.Id = et.EmployeeId";

            if(completed == "false")
            {
                sql += " WHERE tp.StartDate >= CURRENT_TIMESTAMP";
            }
            if(completed == "true")
            {
                sql += " WHERE tp.StartDate <= CURRENT_TIMESTAMP";
            }

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();

                    while (reader.Read())
                    {
                        TrainingProgram trainingProgram = new TrainingProgram()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                        };
                        Employee employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                        };
                        trainingProgram.employees.Add(employee);
                        trainingPrograms.Add(trainingProgram);
                    }
                    reader.Close();
                    //if (completed == "true")
                    //{
                    //    return Ok(trainingPrograms.Where(t => t.IsComplete() == completed));
                    //} 
                    //if (completed == "false")
                    //{
                    //    return Ok(trainingPrograms.Where(t => t.IsComplete() == completed));
                    //}
                    return Ok(trainingPrograms);
                }
            }
        }

        // GET: api/TrainingProgram/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/TrainingProgram
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/TrainingProgram/5
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
