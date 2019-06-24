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
    {
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
        [Fact]
        public async Task Test_Get_NonExitant_ProductType_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/producttype/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task Test_Create_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                ProductType tablet = new ProductType
                {
                    Name = "Tablets"                    
                };
                var tabletAsJSON = JsonConvert.SerializeObject(tablet);


                var response = await client.PostAsync(
                    "api/producttype/",
                    new StringContent(tabletAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var newTablet = JsonConvert.DeserializeObject<ProductType>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Tablets", newTablet.Name);
            }
        }
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
        [Fact]
        public async Task Test_Modify_ProductType()
        {
            // New name to change to and test
            string newName = "Appliances";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                ProductType modifiedAppliance = new ProductType
                {
                    Name = newName
                };
                var modifiedApplianceAsJSON = JsonConvert.SerializeObject(modifiedAppliance);

                var response = await client.PutAsync(
                    "api/producttype/6",
                    new StringContent(modifiedApplianceAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getAppliance = await client.GetAsync("api/producttype/6");
                getAppliance.EnsureSuccessStatusCode();

                string getApplianceBody = await getAppliance.Content.ReadAsStringAsync();
                ProductType newAppliance = JsonConvert.DeserializeObject<ProductType>(getApplianceBody);

                Assert.Equal(HttpStatusCode.OK, getAppliance.StatusCode);
                Assert.Equal(newName, newAppliance.Name);
            }
        }
    }
}
