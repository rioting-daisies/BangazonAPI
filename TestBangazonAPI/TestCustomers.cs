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
    public class TestCustomers
    {
        [Fact]
        public async Task Test_Get_All_Customers()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/Customer");


                string responseBody = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customers.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Customer/1");

                string responseBody = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<Customer>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Sermour", customer.FirstName);
                Assert.Equal("Butts", customer.LastName);
                Assert.NotNull(customer);
            }
        }

        [Fact]
        public async Task Test_Get_NonExistant_Customer_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/Customer/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                Customer helen = new Customer
                {
                    FirstName = "Helen",
                    LastName = "Chalmers"
                };
                var helenAsJSON = JsonConvert.SerializeObject(helen);


                var response = await client.PostAsync(
                    "api/Customer",
                    new StringContent(helenAsJSON, Encoding.UTF8, "application/json")
                );


                string responseBody = await response.Content.ReadAsStringAsync();
                var newHelen = JsonConvert.DeserializeObject<Customer>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Helen", newHelen.FirstName);
                Assert.Equal("Chalmers", newHelen.LastName);


                var deleteResponse = await client.DeleteAsync($"api/Customer/{newHelen.Id}");
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_Customer_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/api/Customer/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Customer()
        {
            // New last name to change to and test
            string newFirstName = "Seymour";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                Customer modifiedSeymour = new Customer
                {
                    FirstName = newFirstName,
                    LastName = "Butts"
                };
                var modifiedSeymourAsJSON = JsonConvert.SerializeObject(modifiedSeymour);

                var response = await client.PutAsync(
                    "api/Customer/1",
                    new StringContent(modifiedSeymourAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getSeymour = await client.GetAsync("api/Customer/1");
                getSeymour.EnsureSuccessStatusCode();

                string getSeymourBody = await getSeymour.Content.ReadAsStringAsync();
                Customer newSeymour = JsonConvert.DeserializeObject<Customer>(getSeymourBody);

                Assert.Equal(HttpStatusCode.OK, getSeymour.StatusCode);
                Assert.Equal(newFirstName, newSeymour.FirstName);
            }
        }
    }
}
