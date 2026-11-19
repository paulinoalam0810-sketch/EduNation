using Students.Application.CreateStudent;
using Students.Application.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace Students.Application.Tests.Integration
{
    public class StudentsControllerTests
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public StudentsControllerTests()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateStudent_ShouldReturn201Created_WheRequestIsValid()
        {
            //Arrange
            var request = new CreateStudentRequest
            {
                FirstName = "Alam",
                LastName = "Paulino",
                Email = "alamp@example.com",
                DateOfBirth = new DateOnly(2008, 10, 8)
            };

            //Act
            var response = await _client.PostAsJsonAsync(
                "/api/students",
                request);

            var responseBody = await response.Content.ReadAsStringAsync();

            Assert.True(
                response.StatusCode == HttpStatusCode.Created,
                $"Status: {response.StatusCode}. Body: {responseBody}");

            //Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
