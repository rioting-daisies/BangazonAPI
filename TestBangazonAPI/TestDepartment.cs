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
   public class TestDepartment
    {
        //Testing to see if getting all the departments from the database works
        [Fact]
        public async Task Test_Get_All_Departments()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Department");

                string responseBody = await response.Content.ReadAsStringAsync();
                var departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(departments.Count > 0);
            }
        }
        //Test to get a single department by its unique ID
        [Fact]
        public async Task Test_Get_Single_Department()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Department/1");

                string responseBody = await response.Content.ReadAsStringAsync();
                var department = JsonConvert.DeserializeObject<List<Department>>(responseBody);

                foreach (Department d in department)
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.Equal("HR", d.Name);
                    Assert.Equal(25000, d.Budget);
                    Assert.NotNull(d);
                }
            }
        }
        //Testing to see if correct exception is thrown if nonexistant department id is searched
        [Fact]
        public async Task Test_Get_Nonexistant_Department()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Department/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        //Test to see if the POST and DELETE methods works for Departments
        [Fact]
        public async Task Test_Create_Delete_Department()
        {
            using (var client = new APIClientProvider().Client)
            {
                Department place = new Department
                {
                    Name = "Mcdonalds",
                    Budget = 2
                };
                var placeAsJSON = JsonConvert.SerializeObject(place);

                var response = await client.PostAsync(
                    "api/Department",
                    new StringContent(placeAsJSON, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();
                var newPlace = JsonConvert.DeserializeObject<Department>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Mcdonalds", newPlace.Name);
                Assert.Equal(2, newPlace.Budget);
                

                var deleteResponse = await client.DeleteAsync($"api/Department/{newPlace.Id}");
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        //Test to make sure when a nonexistant department id is entered to be deleted correct exception is thrown.
        [Fact]
        public async Task Test_Delete_NonExistent_Department_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("api/Department/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        //Test to check if query for department with its employees works using the string parameter _include=employees
        [Fact]
        public async Task Test_Get_All_Departments_With_Employees()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Department?_include=employees");

                string responseBody = await response.Content.ReadAsStringAsync();
                var departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);
                var employees = JsonConvert.DeserializeObject<List<Employee>>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(departments.Count > 0);
                Assert.True(employees.Count > 0);
            }
        }
        //Test to check if query for single department targerted by Id with its employees works using the string parameter _include=employees
        [Fact]
        public async Task Test_Get_Single_Department_With_Employees()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Department/1?_include=employees");

                string responseBody = await response.Content.ReadAsStringAsync();
                var department = JsonConvert.DeserializeObject<List<Department>>(responseBody);
                var employees = JsonConvert.DeserializeObject<List<Employee>>(responseBody);
                foreach (Department d in department)
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.Equal("HR", d.Name);
                    Assert.Equal(25000, d.Budget);
                    Assert.True(employees.Count > 0);
                    Assert.NotNull(d);
                }
            }
        }
        //Test using the string query _filer_budget&_gt_integer to search for all departments that have a budget greater than the integer entered
        [Fact]
        public async Task Test_Get_Departments_By_Budget()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Department?_filter=budget&_gt=100");

                string responseBody = await response.Content.ReadAsStringAsync();
                var departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);

                foreach(Department d in departments)
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.True(departments.Count > 0);
                }
            }
        }
        //Test using the string query _filer_budget&_gt_integer to search and see if single department with Id targeted has a budget greater than the integer entered
        [Fact]
        public async Task Test_Get_Departments_By_ID_By_Budget()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Department/1?_filter=budget&_gt=300");

                string responseBody = await response.Content.ReadAsStringAsync();
              

                var departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);

                foreach (Department d in departments)
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.True(departments.Count > 0);
                }
            }
        }
    }
}
