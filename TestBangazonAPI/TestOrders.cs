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
    public class TestOrders
    {   // Test to check functionality of the get all Order controller
        [Fact]
        public async Task Test_Get_All_Orders()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/Order");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
            }
        }
        // Test to check functionality of the get one Order by Id controller
        [Fact]
        public async Task Test_Get_Single_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Order/1");

                string responseBody = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<Order>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(1, order.CustomerId);
                Assert.NotNull(order);
            }
        }
        // Test to check attempt to get a non existent order entry
        [Fact]
        public async Task Test_Get_NonExitant_Order_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/order/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        // Test to check the post and delete order controllers 
        [Fact]
        public async Task Test_Create_Delete_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                Order order = new Order
                {
                    CustomerId = 1,
                    PaymentTypeId = 2
                };
                var orderAsJSON = JsonConvert.SerializeObject(order);


                var response = await client.PostAsync(
                    "api/order/",
                    new StringContent(orderAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                var newOrder = JsonConvert.DeserializeObject<Order>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(1, newOrder.CustomerId);
                Assert.Equal(2, newOrder.PaymentTypeId);

                var deleteResponse = await client.DeleteAsync($"api/order/{newOrder.Id}");
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }
        // Test to check for attempt to delete a non existent order 
        [Fact]
        public async Task Test_Delete_NonExistent_Order_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/api/order/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }
        // Test to check the modify order controller
        [Fact]
        public async Task Test_Modify_Order()
        {
            // New paymentTypeId to change to and test
            int newPaymentTypeId = 2;

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                Order modifiedPaymentType = new Order
                {
                    PaymentTypeId = newPaymentTypeId
                };
                var modifiedPaymentTypeAsJSON = JsonConvert.SerializeObject(modifiedPaymentType);

                var response = await client.PutAsync(
                    "api/order/1",
                    new StringContent(modifiedPaymentTypeAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getPaymentType = await client.GetAsync("api/order/1");
                getPaymentType.EnsureSuccessStatusCode();

                string getPaymentTypeBody = await getPaymentType.Content.ReadAsStringAsync();
                Order newPaymentType = JsonConvert.DeserializeObject<Order>(getPaymentTypeBody);

                Assert.Equal(HttpStatusCode.OK, getPaymentType.StatusCode);
                Assert.Equal(newPaymentTypeId, newPaymentType.PaymentTypeId);
            }
        }
        // Test to check for attempt to modify a non existent order 
        [Fact]
        public async Task Test_Modify_NonExistent_Order_Fails()
        {
            int newPaymentTypeId = 1;
            using (var client = new APIClientProvider().Client)
            {
                Order modifiedPaymentType = new Order
                {
                    PaymentTypeId = newPaymentTypeId
                };
                var modifiedPaymentTypeAsJSON = JsonConvert.SerializeObject(modifiedPaymentType);
                var modifyResponse = await client.PutAsync("/api/order/600000",
                    new StringContent(modifiedPaymentTypeAsJSON, Encoding.UTF8, "application/json"));

                Assert.False(modifyResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, modifyResponse.StatusCode);
            }
        }
    }
}
