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
                Assert.Equal("20190618 10:34:09 AM", trainingProgram.StartDate);
                Assert.Equal("20190618 10:34:09 AM", trainingProgram.EndDate);
                //Assert.Equal("Firey", employees[0].FirstName);
                Assert.Equal(2, trainingProgram[0].MaxAttendees);
                Assert.NotNull(trainingProgram);
            }
        }

        [Fact]
        public async Task Test_Get_NonExitant_TrainingProgram_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/trainingprogram/9999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}
