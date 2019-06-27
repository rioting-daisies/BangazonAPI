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
            string sql = @"SELECT tp.Id, 
                            tp.Name, 
                            tp.StartDate, 
                            tp.EndDate, 
                            tp.MaxAttendees, 
                            e.Id AS employeeId, 
                            e.FirstName,
                            e.LastName, 
                            e.DepartmentId, 
                            e.IsSuperVisor,
                            et.EmployeeId AS etEmployeeId
                            FROM TrainingProgram tp
                          left JOIN EmployeeTraining et ON et.TrainingProgramId = tp.Id
                          left JOIN Employee e ON e.Id = et.EmployeeId";

            if (completed == "false")
            {
                sql += " WHERE tp.StartDate >= CURRENT_TIMESTAMP";
            }
            if (completed == "true")
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

                    var programs = new Dictionary<int, TrainingProgram>();

                    Employee employee = null; 

                    while (reader.Read())
                    {
                        if (!programs.ContainsKey(reader.GetInt32(reader.GetOrdinal("Id"))))
                        {
                            TrainingProgram trainingProgram = new TrainingProgram()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees")),
                                employees = new List<Employee>()
                            };
                            programs.Add(trainingProgram.Id, trainingProgram);

                        }
                        //string employeeName = reader.GetString(reader.GetOrdinal("FirstName"));

                        //if (reader.GetString(reader.GetOrdinal("FirstName")) != null)
                        //{
                        try
                        {
                                employee = new Employee()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("employeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                            };
                        programs[reader.GetInt32(reader.GetOrdinal("Id"))].employees.Add(employee);

                        }
                        catch (Exception)
                        {
                            employee = null;
                        }

                        //}
                        //trainingProgram.employees.Add(employee);


                        //if(!trainingProgram.employees.Any(c => c.Id == c.Id))
                        //{

                        //}
                        //trainingPrograms.Add(trainingProgram);

                    }
                    reader.Close();
                    List<TrainingProgram> trainingPrograms1 = programs.Values.ToList();
                    //if (completed == "true")
                    //{
                    //    return Ok(trainingPrograms.Where(t => t.IsComplete() == completed));
                    //} 
                    //if (completed == "false")
                    //{
                    //    return Ok(trainingPrograms.Where(t => t.IsComplete() == completed));
                    //}
                    return Ok(trainingPrograms1);
                }
            }
        }

        // GET: api/TrainingProgram/5
        [HttpGet("{id}", Name = "GetTrainingProgram")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            if (!TrainingProgramExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.Id, 
                                        tp.Name, 
                                        tp.StartDate, 
                                        tp.EndDate, 
                                        tp.MaxAttendees, 
                                        e.Id AS employeeId, 
                                        e.FirstName, 
                                        e.LastName, 
                                        e.DepartmentId, 
                                        e.IsSuperVisor FROM                         
                                        TrainingProgram tp
                                        left JOIN EmployeeTraining et ON et.TrainingProgramId = tp.Id
                                        left JOIN Employee e ON e.Id = et.EmployeeId WHERE tp.Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    TrainingProgram trainingProgram = null;
                    Employee employee = null;

                    var program = new Dictionary<int, TrainingProgram>();

                    while (reader.Read())
                    {
                        if (!program.ContainsKey(reader.GetInt32(reader.GetOrdinal("Id"))))
                        {


                            trainingProgram = new TrainingProgram()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                                MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                            };
                            program.Add(trainingProgram.Id, trainingProgram);
                        }

                        try
                        {
                        employee = new Employee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("employeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                        };
                        program[reader.GetInt32(reader.GetOrdinal("Id"))].employees.Add(employee);

                        }
                        catch (Exception)
                        {
                            employee = null;
                        }


                        //trainingProgram.employees.Add(employee);
                    }
                    reader.Close();

                    List<TrainingProgram> trainingPrograms1 = program.Values.ToList();
                    return Ok(trainingPrograms1);
                }
            }
        }

        // POST: api/TrainingProgram
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TrainingProgram trainingProgram)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees) OUTPUT INSERTED.Id 
                                        VALUES (@Name, @StartDate, @EndDate, @MaxAttendees)";

                    cmd.Parameters.Add(new SqlParameter("@Name", trainingProgram.Name));
                    cmd.Parameters.Add(new SqlParameter("@StartDate", trainingProgram.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@EndDate", trainingProgram.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@MaxAttendees", trainingProgram.MaxAttendees));

                    int newId = (int)await cmd.ExecuteScalarAsync();
                    trainingProgram.Id = newId;
                    return CreatedAtRoute("GetTrainingProgram", new { id = newId }, trainingProgram);
                }
            }
        }

        // PUT: api/TrainingProgram/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute]int id, [FromBody] TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE TrainingProgram SET 
                                            Name = @N, 
                                            StartDate = @SD,
                                            EndDate = @ED,
                                            MaxAttendees = @MA
                                            WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@N", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@SD", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@ED", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@MA", trainingProgram.MaxAttendees));

                        int rowAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("Did't work yo");
                    }
                }
            }
            catch (Exception)
            {
                if (!TrainingProgramExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM TrainingProgram WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("didnt work homes");
                    }
                }
            }
            catch (Exception)
            {
                if (!TrainingProgramExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool TrainingProgramExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM TrainingProgram WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
