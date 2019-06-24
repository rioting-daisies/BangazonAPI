using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;

namespace TestBangazonAPI
{
    public class TestPaymentTypes
    {
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
    }
}

//[Fact]
//public async Task Test_Get_Single_Student()
//{

//    using (var client = new APIClientProvider().Client)
//    {
//        var response = await client.GetAsync("/student/1");


//        string responseBody = await response.Content.ReadAsStringAsync();
//        var student = JsonConvert.DeserializeObject<Student>(responseBody);

//        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//        Assert.Equal("Kate", student.FirstName);
//        Assert.Equal("Williams-Spradlin", student.LastName);
//        Assert.NotNull(student);
//    }
//}
