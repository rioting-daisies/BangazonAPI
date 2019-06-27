// AUTHOR: CLIFTON MATUSZEWSKI

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
    //Setting connection route for api/Department
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public DepartmentController(IConfiguration config)
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
        // Get api/Department Getting all departments from database
        // If string parameter _include=employees is used than all employees associated with each department is included as well
        //If string parameter _filer_budget&_gt_integer than any department with a budget greater than the integer entered is gotten from Database
        [HttpGet]
        public async Task<IActionResult> Get(string _include, int? _gt)

        {
            string sql = $"SELECT Id, Name, Budget FROM Department";

            if (_include != null)
            {
                sql = @"SELECT d.Id AS Id, d.Name AS Name, d.Budget AS Budget, 
                            e.Id AS EmployeeId, e.FirstName AS FirstName, e.LastName AS LastName, e.DepartmentId AS Department
                            FROM Department d
                            JOIN Employee e ON d.Id = e.DepartmentId
                            ";
            }
            if (_gt != null)
            {
                sql = $@"SELECT Id, Name, Budget FROM Department WHERE Budget > {_gt}";
            }

                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        if (_include != null)
                        {
                            cmd.Parameters.Add(new SqlParameter("@_include", $"%{_include}%"));
                        }

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        List<Department> departments = new List<Department>();
                        List<Employee> employees = new List<Employee>();
                        while (reader.Read())
                        {


                            if (_include != null)
                            {
                                Employee person = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("Department"))
                                };
                                employees.Add(person);
                            }

                            Department department = new Department()

                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                                Employees = employees.Where(e => e.DepartmentId == reader.GetInt32(reader.GetOrdinal("Id"))).ToList()
                            };
                            if (!departments.Any(d => d.Id == reader.GetInt32(reader.GetOrdinal("Department"))))
                            {

                                departments.Add(department);
                            }
                            else
                            {
                                departments.Find(d => d.Id == department.Id).Employees = department.Employees;
                            }


                        }
                        reader.Close();
                        return Ok(departments);
                    }
                }

        }

        // GET: api/Department/5 Getting one department by Id from database
        // If string parameter _include=employees is used than all employees associated with targeted department is included as well
        //If string parameter _filer_budget&_gt_integer than the department will only be gotten from the database if its budget is greater than the integer queried

        [HttpGet("{id}")]
        //[HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(int id, string _include, int? _gt)
        {
            if (!DepartmentExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            string sql = "SELECT Id, Name, Budget FROM Department";

            if (_gt != null)
            {
                sql = $@"SELECT Id, Name, Budget FROM Department WHERE Budget > {_gt}";
            }

            if (_include != null)
            {
                sql = @"SELECT d.Id AS Id, d.Name AS Name, d.Budget AS Budget, 
                            e.Id AS EmployeeId, e.FirstName AS FirstName, e.LastName AS LastName, e.DepartmentId AS Department
                            FROM Department d
                            JOIN Employee e ON d.Id = e.DepartmentId
                            ";
            }
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (_include != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@_include", $"%{_include}%"));
                    }
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Department> departments = new List<Department>();
                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {

                        if (_include != null)
                        {
                            Employee person = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("Department"))
                            };
                            employees.Add(person);
                        }
                        Department department = new Department()

                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            Employees = employees.Where(e => e.DepartmentId == reader.GetInt32(reader.GetOrdinal("Id"))).ToList()
                        };
                        if (!departments.Any(d => d.Id == reader.GetInt32(reader.GetOrdinal("Department"))))
                        {

                            departments.Add(department);
                        }
                        else
                        {
                            departments.Find(d => d.Id == department.Id).Employees = department.Employees;
                        }
                    }

                    reader.Close();
                    return Ok(departments);
                }
            }
        }

        //Post api/department Posting new department to the database
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Department department)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Department (Name, Budget)
                        OUTPUT INSERTED.Id
                        VALUES(@Name, @Budget)";
                    cmd.Parameters.Add(new SqlParameter("@Name", department.Name));
                    cmd.Parameters.Add(new SqlParameter("@Budget", department.Budget));


                    department.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtAction("Get", new { id = department.Id }, department);
                }
            }
        }

        //PUT api/department/5 Editing department from database using Department Id to target
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Department department)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Department
                                          SET Name = @Name,
                                              Budget = @Budget
                                              WHERE Id = @id";

                        cmd.Parameters.Add(new SqlParameter("@Name", department.Name));
                        cmd.Parameters.Add(new SqlParameter("@Budget", department.Budget));
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
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //DELETE api/department/1 Delete department from databased using Id as target value
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
                        cmd.CommandText = @"DELETE FROM Department WHERE Id = @id";
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
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        //Method to check if Department with certain Id exists
        private bool DepartmentExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id From Department where Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
