// Author: Billy Mitchell
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Net.Http;

namespace TestBangazonAPI
{
    public class TestProductTypes
    {   // Test to check functionality of the get all ProductType controller.
        [Fact]
        public async Task Test_Get_All_ProductTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/ProductType");


                string responseBody = await response.Content.ReadAsStringAsync();
                var productTypes = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productTypes.Count > 0);
            }
        }
        // Test to check functionality of the get one ProductType by Id controller.
        [Fact]
        public async Task Test_Get_Single_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/ProductType/1");

                string responseBody = await response.Content.ReadAsStringAsync();
                var productType = JsonConvert.DeserializeObject<ProductType>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);                
                Assert.Equal("Computers", productType.Name);                
                Assert.NotNull(productType);
            }
        }
        // Test to check attempt to get a non existent product type.
        [Fact]
        public async Task Test_Get_NonExitant_ProductType_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/producttype/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        // Test to check the post and delete product type controllers. 
        [Fact]
        public async Task Test_Create_Delete_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                ProductType tablet = new ProductType
                {
                    Name = "Televisions"
                };
                var tabletAsJSON = JsonConvert.SerializeObject(tablet);


                var response = await client.PostAsync(
                    "api/producttype/",
                    new StringContent(tabletAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var newTablet = JsonConvert.DeserializeObject<ProductType>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Televisions", newTablet.Name);

                var deleteResponse = await client.DeleteAsync($"api/producttype/{newTablet.Id}");
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }
        // Test to check for attempt to delete a non existent product type. 
        [Fact]
        public async Task Test_Delete_NonExistent_ProductType_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/api/producttype/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }
        // Test to check the modify product type controller. 
        [Fact]
        public async Task Test_Modify_ProductType()
        {
            // New name to change to and test
            string newName = "Computers";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                ProductType modifiedComputer = new ProductType
                {
                    Name = newName
                };
                var modifiedComputerAsJSON = JsonConvert.SerializeObject(modifiedComputer);

                var response = await client.PutAsync(
                    "api/producttype/1",
                    new StringContent(modifiedComputerAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getComputer = await client.GetAsync("api/producttype/1");
                getComputer.EnsureSuccessStatusCode();

                string getComputerBody = await getComputer.Content.ReadAsStringAsync();
                ProductType newComputer = JsonConvert.DeserializeObject<ProductType>(getComputerBody);

                Assert.Equal(HttpStatusCode.OK, getComputer.StatusCode);
                Assert.Equal(newName, newComputer.Name);
            }
        }
        // Test to check for attempt to modify a non existent product type. 
        [Fact]
        public async Task Test_Modify_NonExistent_ProductType_Fails()
        {
            string newName = "Computers";
            using (var client = new APIClientProvider().Client)
            {
                ProductType modifiedComputer = new ProductType
                {
                    Name = newName
                };
                var modifiedComputerAsJSON = JsonConvert.SerializeObject(modifiedComputer);
                var modifyResponse = await client.PutAsync("/api/producttype/600000",
                    new StringContent(modifiedComputerAsJSON, Encoding.UTF8, "application/json"));

                Assert.False(modifyResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, modifyResponse.StatusCode);
            }
        }
    }
}
