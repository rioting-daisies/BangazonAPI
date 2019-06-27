//Author: CLIFTON MATUSZEWSKI
using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TestBangazonAPI
{
    public class TestEmployee
    {
        //Testing to see if getting all the employees from the database works
        [Fact]
        public async Task Test_Get_All_Employees()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Employee");

                string responseBody = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<List<Employee>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(employees.Count > 0);
            }
        }
        //Testing to see if getting a single employee by id from the database works
        [Fact]
        public async Task Test_Get_Single_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Employee/1");

                string responseBody = await response.Content.ReadAsStringAsync();
                var employee = JsonConvert.DeserializeObject<Employee>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Firey", employee.FirstName);
                Assert.Equal("Dragon", employee.LastName);
                
                Assert.NotNull(employee);
            }
        }
        //Testing to see if correct exception is thrown if nonexistant Employee id is searched
        [Fact]
        public async Task Test_Get_Nonexistant_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Employee/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        //Test to see if the POST and DELETE methods works for Employees
        [Fact]
        public async Task Test_Create_Delete_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                Employee person = new Employee
                {
                   FirstName = "Joe",
                   LastName = "BillyBobSeamore",
                   IsSuperVisor = true,
                   DepartmentId = 1,
                   department = new Department
                   {
                       
                           Id = 1,
                           Name = "HR",
                           Budget = 25000
                       
                   }
                };
                var personAsJSON = JsonConvert.SerializeObject(person);

                var response = await client.PostAsync(
                    "api/Employee",
                    new StringContent(personAsJSON, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();
                var newPerson = JsonConvert.DeserializeObject<Employee>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Joe", person.FirstName);
                Assert.Equal("BillyBobSeamore", person.LastName);
                Assert.Equal(1, person.DepartmentId);
               

                var deleteResponse = await client.DeleteAsync($"api/Employee/{newPerson.Id}");
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }
        //Test to make sure when a nonexistant employee id is entered to be deleted correct exception is thrown.
        [Fact]
        public async Task Test_Delete_NonExistent_Employee_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("api/Employee/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }
        //Test for PUT on employee works for editing existing products
        [Fact]
        public async Task Test_Modify_Product()
        {

            string NewFirstName = "Fiery";

            using (var client = new APIClientProvider().Client)
            {

                Employee modifiedEmployee = new Employee
                {
                    FirstName = NewFirstName,
                    LastName = "Dragon",
                    IsSuperVisor = true,
                    DepartmentId = 1,
                    
                };
                var modifiedEmployeeASJSON = JsonConvert.SerializeObject(modifiedEmployee);

                var response = await client.PutAsync(
                    "api/Employee/1",
                    new StringContent(modifiedEmployeeASJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                var getEmployee = await client.GetAsync("api/Employee/1");
                getEmployee.EnsureSuccessStatusCode();

                string getEmployeeBody = await getEmployee.Content.ReadAsStringAsync();
                Employee newEmployee = JsonConvert.DeserializeObject<Employee>(getEmployeeBody);

                Assert.Equal(HttpStatusCode.OK, getEmployee.StatusCode);
                Assert.Equal(NewFirstName, newEmployee.FirstName);
            }
        }
        //Test to make sure that correct exception is thrown if you try to modify a non existant product
        [Fact]
        public async Task Test_Edit_NonExistent_Product_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                Employee thing = new Employee()
                {
                    FirstName = "Blah",
                    LastName = "Dragon",
                    IsSuperVisor = true,
                    DepartmentId = 1,
                    department = new Department
                    {
                        Id = 1,
                        Name = "HR",
                        Budget = 25000
                    }
                };
                var thingASJSON = JsonConvert.SerializeObject(thing);
                var editResponse = await client.PutAsync("api/Employee/600000", new StringContent(thingASJSON, Encoding.UTF8, "application/json"));

                Assert.False(editResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, editResponse.StatusCode);
            }
        }

    }
}
