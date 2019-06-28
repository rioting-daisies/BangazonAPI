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
    public class TestComputer
    {
        //Testing to see if getting all the computers from the database works
        [Fact]
        public async Task Test_Get_All_Computers()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Computer");

                string responseBody = await response.Content.ReadAsStringAsync();
                var computers = JsonConvert.DeserializeObject<List<Computer>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(computers.Count > 0);
            }
        }
        //Testing to see if getting a single computer by id from the database works
        [Fact]
        public async Task Test_Get_Single_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Computer/1");

                string responseBody = await response.Content.ReadAsStringAsync();
                var computer = JsonConvert.DeserializeObject<Computer>(responseBody);


                Assert.Equal("Latitude", computer.Make);
                Assert.Equal("Dell", computer.Manufacturer);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                
            }
        }
        //Testing to see if correct exception is thrown if nonexistant computer id is searched
        [Fact]
        public async Task Test_Get_Nonexistant_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/Computer/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        //Test to see if the POST and DELETE methods works for Computer
        [Fact]
        public async Task Test_Create_Delete_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                Computer toshiba = new Computer
                {
                    PurchaseDate = new DateTime(2018, 6, 27),
                    DecomissionDate = new DateTime(2019, 6, 26),
                    Make = "Latitude",
                    Manufacturer = "Toshiba"
                };
                var toshibaAsJSON = JsonConvert.SerializeObject(toshiba);

                var response = await client.PostAsync(
                    "api/Computer",
                    new StringContent(toshibaAsJSON, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();
                var newToshiba = JsonConvert.DeserializeObject<Computer>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Latitude", toshiba.Make);
                Assert.Equal("Toshiba", toshiba.Manufacturer);
                

                var deleteResponse = await client.DeleteAsync($"api/Computer/{newToshiba.Id}");
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }
        //Test to make sure when a nonexistant Computer id is entered to be deleted correct exception is thrown.
        [Fact]
        public async Task Test_Delete_NonExistent_Computer_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("api/Computer/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }
        //Test for PUT on computer works for editing existing products
        [Fact]
        public async Task Test_Modify_Computer()
        {

            String newManufacturer = "Dell";

            using (var client = new APIClientProvider().Client)
            {

                Computer modifiedComputer = new Computer
                {
                    PurchaseDate = new DateTime(2018, 6, 27),
                    DecomissionDate = new DateTime(2019, 6, 26),
                    Make = "Latitude",
                    Manufacturer = newManufacturer
                };
                var modifiedComputerAsJSON = JsonConvert.SerializeObject(modifiedComputer);

                var response = await client.PutAsync(
                    "api/Computer/1",
                    new StringContent(modifiedComputerAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


                var getComputer = await client.GetAsync("api/Computer/1");
                getComputer.EnsureSuccessStatusCode();

                string getComputerBody = await getComputer.Content.ReadAsStringAsync();
                Computer newComputer = JsonConvert.DeserializeObject<Computer>(getComputerBody);

                Assert.Equal(HttpStatusCode.OK, getComputer.StatusCode);
                Assert.Equal(newManufacturer, newComputer.Manufacturer);
            }
        }
        //Test to make sure that correct exception is thrown if you try to modify a non existant Computer
        [Fact]
        public async Task Test_Edit_NonExistent_Computer_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                Computer thing = new Computer()
                {
                    PurchaseDate = new DateTime(2018, 6, 27),
                    DecomissionDate = new DateTime(2019, 6, 26),
                    Make = "Blah",
                    Manufacturer = "Blah"
                };
                var thingASJSON = JsonConvert.SerializeObject(thing);
                var editResponse = await client.PutAsync("api/Computer/600000", new StringContent(thingASJSON, Encoding.UTF8, "application/json"));

                Assert.False(editResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, editResponse.StatusCode);
            }
        }

    }
}
