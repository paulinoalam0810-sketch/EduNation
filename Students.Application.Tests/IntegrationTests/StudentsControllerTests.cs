using Students.Application.CreateStudent;
using Students.Application.Tests.Infrastructure;
using Students.Application.UpdateStudent;
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
        public async Task CreateStudent_ShouldReturn201Created_WhenRequestIsValid()
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

            //Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetAllStudents_ShouldReturn200OK_WhenStudentsExist()
        {
            //Arrange
            var request = new CreateStudentRequest
            {
                FirstName = "Dariel",
                LastName = "Sanchez",
                Email = "dariel@example.com",
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
            Assert.Contains("dariel@example.com", responseBody);
        }

        [Fact]
        public async Task GetStudentById_ShouldReturn200OK_WhenStudentExists()
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

        [Fact]
        public async Task UpdateStudent_ShouldReturn200OK_WhenRequestIsValid()
        {
            //Arrange
            var request = new CreateStudentRequest
            {
                FirstName = "Samil",
                LastName = "Paulino",
                Email = "samil@example.com",
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

            var update = new UpdateStudentRequest
            {
                FirstName = "Carlos",
                LastName = "Martinez",
                Email = "carlos@example.com",
                DateOfBirth = new DateOnly(2000, 5, 15)
            };

            var updateResponse = await _client.PutAsJsonAsync(
                $"/api/students/{createdStudent.Id}",
                update);

            var response = await _client.GetAsync(
                $"/api/students/{createdStudent.Id}");

            var updatedStudent = await response.Content
                .ReadFromJsonAsync<CreateStudentResponse>();

            //Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.NotNull(updatedStudent);

            Assert.Equal(createdStudent.Id, updatedStudent.Id);
            Assert.Equal("Carlos", updatedStudent.FirstName);
            Assert.Equal("Martinez", updatedStudent.LastName);
            Assert.Equal("carlos@example.com", updatedStudent.Email);
            Assert.Equal(new DateOnly(2000, 5, 15), updatedStudent.DateOfBirth);
        }

        [Fact]
        public async Task DeactivateStudent_ShouldReturn204NoContent_WhenStudentExists()
        {
            //Arrange 
            var request = new CreateStudentRequest
            {
                FirstName = "Francisco",
                LastName = "Paulino",
                Email = "francisco@example.com",
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

            var deactivateResponse = await _client.DeleteAsync(
                $"/api/students/{createdStudent.Id}");

            var getResponse = await _client.GetAsync(
                $"/api/students/{createdStudent.Id}");

            var getAllResponse = await _client.GetAsync(
                "/api/students");

            var responseBody = await getAllResponse.Content.ReadAsStringAsync();

            //Assert
            Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);
            Assert.DoesNotContain("francisco@example.com", responseBody);

        }
    }
}
