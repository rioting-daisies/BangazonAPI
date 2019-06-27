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
    public class TestTrainingPrograms
    {

        // test get all, makes sure status codes match. because trainingprograms contain a list of employees, line 26 is type var with list<trainingProgram>

        [Fact]
        public async Task Test_Get_All_TrainingPrograms()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/trainingprogram");

                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingPrograms = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(trainingPrograms.Count > 0);
            }
        }

        //tests get single program by id.  again because they can contain employees, have to use var on line 46 and also use list<trainingprogram>

        [Fact]
        public async Task Test_Get_Single_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/trainingprogram/1");

                string responseBody = await response.Content.ReadAsStringAsync();

                //var employees = JsonConvert.DeserializeObject<List<Employee>>(responseBody);

                var trainingProgram = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);

                //var dictionary = new Dictionary<int, TrainingProgram>();
                //dictionary.Add(trainingProgram.Id, trainingProgram);

                //var employeeDictionary = new Dictionary<int, Employee>();
                //employeeDictionary.Add(1, trainingProgram.employees[0]);



                //var trainingProgram = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);
                //var trainingProgram = JsonConvert.DeserializeObject<List<RetrieveMultipleResponse>>(responseBody);

                //Dragon Training','20190618 10:34:09 AM', '20190618 10:34:09 AM', 2

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Dragon Training", trainingProgram[0].Name);
                //Assert.Equal("20190618 10:34:09 AM", trainingProgram.StartDate);
                //Assert.Equal("20190618 10:34:09 AM", trainingProgram.EndDate);
                //Assert.Equal("Firey", employees[0].FirstName);
                Assert.Equal(2, trainingProgram[0].MaxAttendees);
                Assert.NotNull(trainingProgram);
            }
        }

        // test to make sure you can't get a trainingprogram taht doesn't exist. id 9999 doesn't exist so status code notfound should match response.statuscode

        [Fact]
        public async Task Test_Get_NonExitant_TrainingProgram_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/trainingprogram/9999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        //test will create a new entry then delete it again. will make sure status codes match to know actions performed well

        [Fact]
        public async Task Test_Create_and_Delete_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                TrainingProgram test = new TrainingProgram()
                {
                    Name = "test",
                    StartDate = new DateTime(2000, 7, 7),
                    EndDate = new DateTime(2000, 7, 7),
                    MaxAttendees = 5
                };
                var testAsJson = JsonConvert.SerializeObject(test);

                var response = await client.PostAsync("api/trainingprogram", new StringContent(testAsJson, Encoding.UTF8, "application/json"));

                string responseBody = await response.Content.ReadAsStringAsync();
                var newTest = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("test", newTest.Name);
                Assert.Equal(new DateTime(2000, 7, 7), newTest.StartDate);
                Assert.Equal(new DateTime(2000, 7, 7), newTest.EndDate);
                Assert.Equal(5, newTest.MaxAttendees);

                var deleteResponse = await client.DeleteAsync($"api/trainingprogram/{newTest.Id}");
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        //tests to see if you can delete a program that doesn't exist

        [Fact]
        public async Task Test_Delete_NonExistent_TrainingProgram_Fail()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("api/trainingprogram/9999");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        //test the PUT method by updating id=1 with the same data and making sure status codes match

        [Fact]
        public async Task Test_Modify_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                //20190618 10:34:09 AM
                TrainingProgram test = new TrainingProgram()
                {
                    Name = "Dragon Training",
                    StartDate = new DateTime(2019, 06, 18, 10, 34, 09),
                    EndDate = new DateTime(2019, 06, 18, 10, 34, 09),
                    MaxAttendees = 2

                };

                var testAsJson = JsonConvert.SerializeObject(test);

                var response = await client.PutAsync("api/trainingprogram/1", new StringContent(testAsJson, Encoding.UTF8, "application/json"));
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                var getTest = await client.GetAsync("api/trainingprogram/1");
                getTest.EnsureSuccessStatusCode();

                string getTestBody = await getTest.Content.ReadAsStringAsync();

                var newTest = JsonConvert.DeserializeObject<List<TrainingProgram>>(getTestBody);

                Assert.Equal(HttpStatusCode.OK, getTest.StatusCode);
                Assert.Equal("Dragon Training", newTest[0].Name);
            }
        }

        //tries to edit an id that doesn't exist, again makes sure status codes match after the fail

        [Fact]
        public async Task Test_Edit_NonExistent_TrainingProgram_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                TrainingProgram test = new TrainingProgram()
                {
                    Name = "test",
                    StartDate = new DateTime(2019, 1, 1),
                    EndDate = new DateTime(2019, 1, 1),
                    MaxAttendees = 2
                };

                var testAsJson = JsonConvert.SerializeObject(test);
                var editResponse = await client.PutAsync("api/trainingprogram/9999", new StringContent(testAsJson, Encoding.UTF8, "application/json"));

                Assert.False(editResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, editResponse.StatusCode);
            }
        }

        //test query for complete=true.  if user uses endpoint ?completed=true, it should return Name="Dragon Training" and MaxAttendees=2

        [Fact]
        public async Task Test_Get_TrainingProgram_Complete_True()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/trainingprogram?completed=true");

                string responseBody = await response.Content.ReadAsStringAsync();

                var trainingProgram = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Dragon Training", trainingProgram[0].Name);
                Assert.Equal(2, trainingProgram[0].MaxAttendees);
                Assert.NotNull(trainingProgram[0]);
            }
        }
    }
}

//created by Alex, tests all methods and query as well