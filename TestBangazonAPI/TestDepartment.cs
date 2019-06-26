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
        //Testing to see if getting all the products from the database works
        [Fact]
        public async Task Test_Get_All_Departments()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Department");

                string responseBody = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<Department>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(products.Count > 0);
            }
        }

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
        //Testing to see if correct exception is thrown if nonexistant product id is searched
        [Fact]
        public async Task Test_Get_Nonexistant_Department()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Department/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

    }
}
