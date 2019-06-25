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
    public class TestPaymentTypes
    {
        //test for get all data. 

        [Fact]
        public async Task Test_Get_All_PaymentTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/PaymentType");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentTypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymentTypes.Count > 0);
            }
        }

        //test to see if paymenttype with id 1 is equal to the values of 123456, visa, and 1. if not, test will fail

        [Fact]
        public async Task Test_Get_Single_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/PaymentType/1");

                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentType = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(123456, paymentType.AcctNumber);
                Assert.Equal("Visa", paymentType.Name);
                Assert.Equal(1, paymentType.CustomerId);
                Assert.NotNull(paymentType);
            }
        }

        //tests to see if paymenttype with id 9999 exists. this id does not exist, so if it does, test will fail 

        [Fact]
        public async Task Test_Get_NonExitant_PaymentType_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/paymenttype/9999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        //test will create an object, insert it into the paymenttype table, then delete it. test will fail if post or delete doesnt work

        [Fact]
        public async Task Test_Create_and_Delete_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                PaymentType test = new PaymentType()
                {
                    AcctNumber = 00000,
                    Name = "test",
                    CustomerId = 1
                };
                var testAsJson = JsonConvert.SerializeObject(test);

                var response = await client.PostAsync("api/paymenttype", new StringContent(testAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();
                var newTest = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(00000, newTest.AcctNumber);
                Assert.Equal("test", newTest.Name);
                Assert.Equal(1, newTest.CustomerId);

                var deleteResponse = await client.DeleteAsync($"api/paymenttype/{newTest.Id}");
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        //test tries to delete an item that doesn't exist. test should fail because it can't delete an item that doesn't exist

        [Fact]
        public async Task Test_Delete_NonExistent_PaymentType_Fail()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("api/paymenttype/9999");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        //test will update the first object with the same data. if status codes return are valid then test will work

        [Fact]
        public async Task Test_Modify_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                PaymentType test = new PaymentType()
                {
                    AcctNumber = 123456,
                    Name = "Visa",
                    CustomerId = 1
                };

                var testAsJson = JsonConvert.SerializeObject(test);

                var response = await client.PutAsync("api/paymenttype/1", new StringContent(testAsJson, Encoding.UTF8, "application/json"));
                string responseBody = await response.Content.ReadAsStringAsync();
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getTest = await client.GetAsync("api/paymenttype/1");
                getTest.EnsureSuccessStatusCode();

                string getTestBody = await getTest.Content.ReadAsStringAsync();
                PaymentType newTest = JsonConvert.DeserializeObject<PaymentType>(getTestBody);

                Assert.Equal(HttpStatusCode.OK, getTest.StatusCode);
                Assert.Equal("Visa", newTest.Name);
                

            }
        }

        //test tries to update an item that doesn't exist. test works if the update fails. 

        [Fact]
        public async Task Test_Edit_NonExistent_PaymentType_Fails ()
        {
            using (var client = new APIClientProvider().Client)
            {
                PaymentType test = new PaymentType()
                {
                    AcctNumber = 123456,
                    Name = "Visa",
                    CustomerId = 1
                };
                var testAsJson = JsonConvert.SerializeObject(test);
                var editResponse = await client.PutAsync("api/paymenttype/9999", new StringContent(testAsJson, Encoding.UTF8, "application/json"));

                Assert.False(editResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, editResponse.StatusCode);
            }
        }
        

    }
}
