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
    public class TestProduct
    {
        //Testing to see if getting all the products from the database works
        [Fact]
        public async Task Test_Get_All_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Product");

                string responseBody = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<Product>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(products.Count > 0);
            }
        }
        //Testing to see if getting a single product by id from the database works
        [Fact] 
        public async Task Test_Get_Single_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Product/1");

                string responseBody = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(1, product.ProductTypeId);
                Assert.Equal(1, product.CustomerId);
                Assert.Equal(222, product.Price);
                Assert.Equal("Vaccuum of Destiny", product.Title);
                Assert.Equal("Sucks you into your destiny. Youll prolly die oops.", product.Description);
                Assert.Equal(2, product.Quantity);
                Assert.NotNull(product);
            }
        }
        //Testing to see if correct exception is thrown if nonexistant product id is searched
        [Fact]
        public async Task Test_Get_Nonexistant_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Product/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        //Test to see if the POST and DELETE methods works for Products
        [Fact]
        public async Task Test_Create_Delete_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                Product balloon = new Product
                {
                    ProductTypeId = 1,
                    CustomerId = 1,
                    Price = 55,
                    Title = "Balloony",
                    Description = "Floaty Thing",
                    Quantity = 100
                };
                var balloonAsJSON = JsonConvert.SerializeObject(balloon);

                var response = await client.PostAsync(
                    "api/Product",
                    new StringContent(balloonAsJSON, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();
                var newballoon = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(1, newballoon.ProductTypeId);
                Assert.Equal(1, newballoon.CustomerId);
                Assert.Equal(55, newballoon.Price);
                Assert.Equal("Balloony", newballoon.Title);
                Assert.Equal("Floaty Thing", newballoon.Description);
                Assert.Equal(100, newballoon.Quantity);

                var deleteResponse = await client.DeleteAsync($"api/Product/{newballoon.Id}");
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }
        //Test to make sure when a nonexistant product id is entered to be deleted correct exception is thrown.
        [Fact]
        public async Task Test_Delete_NonExistent_Product_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("api/Product/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }
        //Test for PUT on product works for editing existing products
        [Fact]
        public async Task Test_Modify_Product()
        {
          
            int newPrice = 222;

            using (var client = new APIClientProvider().Client)
            {
                
                Product modifiedVaccuum = new Product
                {
                    ProductTypeId = 1,
                    CustomerId = 1,
                    Price = newPrice,
                    Title = "Vaccuum of Destiny",
                    Description = "Sucks you into your destiny. Youll prolly die oops.",
                    Quantity = 2
                };
                var modifiedVaccuumAsJSON = JsonConvert.SerializeObject(modifiedVaccuum);

                var response = await client.PutAsync(
                    "api/Product/1",
                    new StringContent(modifiedVaccuumAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                
                var getVaccuum = await client.GetAsync("api/Product/1");
                getVaccuum.EnsureSuccessStatusCode();

                string getVaccuumBody = await getVaccuum.Content.ReadAsStringAsync();
                Product newVaccuum = JsonConvert.DeserializeObject<Product>(getVaccuumBody);

                Assert.Equal(HttpStatusCode.OK, getVaccuum.StatusCode);
                Assert.Equal(newPrice, newVaccuum.Price);
            }
        }
        //Test to make sure that correct exception is thrown if you try to modify a non existant product
        [Fact]
        public async Task Test_Edit_NonExistent_Product_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                Product thing = new Product()
                {
                    CustomerId = 7,
                    ProductTypeId = 22,
                    Price = 3,
                    Title = "NotRealThing",
                    Description = "Not real thing test",
                    Quantity = 2
                };
                var thingASJSON = JsonConvert.SerializeObject(thing);
                var editResponse = await client.PutAsync("api/Product/600000", new StringContent(thingASJSON, Encoding.UTF8, "application/json"));

                Assert.False(editResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, editResponse.StatusCode);
            }
        }

    }
}
