// Author: Chris Morgan

// The purpose of the TestCustomers class is to hold all of the Unit Tests for the methods defined in the CustomerController.cs file. 



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

        // The Test_Get_All_Customers is a test for the primary Get method for the CustomerController. This will make sure that the List that is returned by that method isn't empty as well as making sure that a 200 status code is returned.
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
                var response = await client.GetAsync("api/Customer");


                string responseBody = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customers.Count > 0);
            }
        }

        // The Test_Get_Single_Customer method is testing the Get([FromRoute] int id) method within the CustomerController. The test will try to get Sermour Butts by querying api/Customer/1, which is the item that represents Sermour. We are testing that we get a 200 status code, the first name = Sermour, and last name = Butts

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
        
        // The Test_Get_NonExistent_Customer_Fails method is testing the failure of the Get([FromRoute] int id) method within the CustomerController. The test queries the database for customer 999999999, which doesn't exist, therefore we make sure that the reponse is a 404 status code

        [Fact]
        public async Task Test_Get_NonExistent_Customer_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Customer/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        // The Test_Create_And_Delete_Customer method is testing both Post and Delete within the CustomerController. The test will create a new customer and post it to the database. The post method returns the newly created item, and a created status code. We test this by checking the response of the Post to see if it returns the Created status code, and by checking the firstName and lastName properties of the new customer.

            // Then We Delete this newly created Customer by targeting the reponse of the Post, which is the newly created customer, and grabbing the Id. We call the Delete method, and check to make sure the no content status code is returned, which is what a successful delete returns
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

        // Test_Delete_NonExistent_Customer_Fails is testing the Delete method in the CustomerController. We try to delete a non existent customer and make sure that the success status code isn't returned, and that the deleteResponse also returns 404 status code.

        [Fact]
        public async Task Test_Delete_NonExistent_Customer_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("api/Customer/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        // Test_Modify_Customer is testing the Put method in CustomerController. This is making sure that Put request is successful when targeting customer 1 and using the put request to modify the customers first name. If successful, it should return a no content status code. Then we get the modified student and ensure that the first name equals the value of what we changed it to and a 200 status code.

        [Fact]
        public async Task Test_Modify_Customer()
        {
            // New last name to change to and test
            string newFirstName = "Sermour";

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

        // The Test_Modify_NonExistent_Customer is testing the Put method in customer controller and trying to update a non existent customer in the database. We ensure that the response returns a 404 status code and doesn't return a success code.
        [Fact]
        public async Task Test_Modify_NonExistentCustomer()
        {
            // New last name to change to and test
            string newFirstName = "Sermour";

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
                    "api/Customer/999999",
                    new StringContent(modifiedSeymourAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.False(response.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            }
        }

        // Test_Get_Customer_With_Q_Query_String_Parameter_Success is testing the query string paramter option of 'q' in the Get method in CustomerController. We ensure that sermour is returned by the request q=ser and a 200 status code is given.
        [Fact]
        public async Task Test_Get_Customer_With_Q_Query_String_Parameter_Success()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Customer?q=ser");

                string responseBody = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Sermour", customers[0].FirstName);
                Assert.Equal("Butts", customers[0].LastName);
                Assert.NotNull(customers[0]);
            }
        }

        // Test_Get_Customer_With_Q_Query_String_Parameter_Success is testing the query string paramter option of 'q' in the Get method in CustomerController. We ensure that a 404 status code is returned with the parameter q=z, which doesn't match any customers in the database.
        [Fact]
        public async Task Test_Get_NonExistentCustomer_With_Q_Query_String_Parameter()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Customer?q=z");

                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}
