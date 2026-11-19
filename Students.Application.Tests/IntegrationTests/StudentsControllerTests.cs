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

        [Fact]
        public async Task GetAllStudents_ShouldReturn200OK_WhenStudentsExists()
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
            var createResponse = await _client.PostAsJsonAsync(
                "/api/students",
                request);

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var response = await _client.GetAsync(
                "/api/students");

            var responseBody = await response.Content.ReadAsStringAsync();

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("alamp@example.com", responseBody);
        }

        [Fact]
        public async Task GetStudentById_ShouldReturn200OK_WhenStudentExist()
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
            var createResponse = await _client.PostAsJsonAsync(
                "/api/students", 
                request);

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdStudent = await createResponse.Content
                .ReadFromJsonAsync<CreateStudentResponse>();

            Assert.NotNull(createdStudent);

            var response = await _client.GetAsync(
                $"/api/students/{createdStudent.Id}");

            var responseBody = await response.Content.ReadAsStringAsync();

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("alamp@example.com", responseBody);
        }
    }
}
