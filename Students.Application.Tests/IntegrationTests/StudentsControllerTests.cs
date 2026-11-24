using Microsoft.AspNetCore.Mvc;
using Students.Application.CreateStudent;
using Students.Application.Tests.Infrastructure;
using Students.Application.Responses;
using Students.Application.UpdateStudent;
using System.Net;
using System.Net.Http.Json;

namespace Students.Application.Tests.Integration
{
    public class StudentsControllerTests : IDisposable
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public StudentsControllerTests()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Fact]
        public async Task CreateStudent_ShouldReturn201Created_WhenRequestIsValid()
        {
            var request = new CreateStudentRequest
            {
                FirstName = "Alam",
                LastName = "Paulino",
                Email = $"alam-{Guid.NewGuid():N}@example.com",
                DateOfBirth = new DateOnly(2008, 10, 8)
            };

            var response = await _client.PostAsJsonAsync("/api/students", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetAllStudents_ShouldReturn200OK_WhenStudentsExist()
        {
            var request = new CreateStudentRequest
            {
                FirstName = "Dariel",
                LastName = "Sanchez",
                Email = $"dariel-{Guid.NewGuid():N}@example.com",
                DateOfBirth = new DateOnly(2008, 10, 8)
            };

            var createResponse = await _client.PostAsJsonAsync("/api/students", request);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var response = await _client.GetAsync("/api/students");
            var students = await response.Content.ReadFromJsonAsync<List<StudentResponse>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(students);
            var student = Assert.Single(students);
            Assert.Equal(request.Email, student.Email);
            Assert.Equal(request.FirstName, student.FirstName);
            Assert.Equal(request.LastName, student.LastName);
            Assert.Equal(request.DateOfBirth, student.DateOfBirth);
            Assert.True(student.IsActive);

        }

        [Fact]
        public async Task GetStudentById_ShouldReturn200OK_WhenStudentExists()
        {
            var request = new CreateStudentRequest
            {
                FirstName = "Alam",
                LastName = "Paulino",
                Email = $"alamp-{Guid.NewGuid():N}@example.com",
                DateOfBirth = new DateOnly(2008, 10, 8)
            };

            var createResponse = await _client.PostAsJsonAsync("/api/students", request);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdStudent = await createResponse.Content.ReadFromJsonAsync<CreateStudentResponse>();
            Assert.NotNull(createdStudent);

            var response = await _client.GetAsync($"/api/students/{createdStudent.Id}");
            var student = await response.Content.ReadFromJsonAsync<StudentResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(student);
            Assert.Equal(request.Email, student.Email);
            Assert.Equal(request.FirstName, student.FirstName);
            Assert.Equal(request.LastName, student.LastName);
            Assert.Equal(request.DateOfBirth, student.DateOfBirth);
            Assert.True(student.IsActive);
        }

        [Fact]
        public async Task UpdateStudent_ShouldReturn200OK_WhenRequestIsValid()
        {
            var request = new CreateStudentRequest
            {
                FirstName = "Samil",
                LastName = "Paulino",
                Email = $"samil-{Guid.NewGuid():N}@example.com",
                DateOfBirth = new DateOnly(2008, 10, 8)
            };

            var createResponse = await _client.PostAsJsonAsync("/api/students", request);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdStudent = await createResponse.Content.ReadFromJsonAsync<CreateStudentResponse>();
            Assert.NotNull(createdStudent);

            var update = new UpdateStudentRequest
            {
                FirstName = "Carlos",
                LastName = "Martinez",
                Email = $"carlos-{Guid.NewGuid():N}@example.com",
                DateOfBirth = new DateOnly(2000, 5, 15)
            };

            var updateResponse = await _client.PutAsJsonAsync(
                $"/api/students/{createdStudent.Id}",
                update);
            var response = await _client.GetAsync($"/api/students/{createdStudent.Id}");
            var updatedStudent = await updateResponse.Content.ReadFromJsonAsync<StudentResponse>();
            var studentFromGet = await response.Content.ReadFromJsonAsync<StudentResponse>();

            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(updatedStudent);
            Assert.NotNull(studentFromGet);
            Assert.Equal(createdStudent.Id, updatedStudent.Id);
            Assert.Equal("Carlos", updatedStudent.FirstName);
            Assert.Equal("Martinez", updatedStudent.LastName);
            Assert.Equal(update.Email, updatedStudent.Email);
            Assert.Equal(update.DateOfBirth, updatedStudent.DateOfBirth);
            Assert.True(updatedStudent.IsActive);
            Assert.Equal(updatedStudent.Email, studentFromGet.Email);
            Assert.Equal(updatedStudent.IsActive, studentFromGet.IsActive);
        }

        [Fact]
        public async Task DeactivateStudent_ShouldReturn204NoContent_WhenStudentExists()
        {
            var request = new CreateStudentRequest
            {
                FirstName = "Francisco",
                LastName = "Paulino",
                Email = $"francisco-{Guid.NewGuid():N}@example.com",
                DateOfBirth = new DateOnly(2008, 10, 8)
            };

            var createResponse = await _client.PostAsJsonAsync("/api/students", request);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdStudent = await createResponse.Content.ReadFromJsonAsync<CreateStudentResponse>();
            Assert.NotNull(createdStudent);

            var deactivateResponse = await _client.DeleteAsync($"/api/students/{createdStudent.Id}");
            var getResponse = await _client.GetAsync($"/api/students/{createdStudent.Id}");
            var getAllResponse = await _client.GetAsync("/api/students");
            var students = await getAllResponse.Content.ReadFromJsonAsync<List<StudentResponse>>();

            Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);
            Assert.NotNull(students);
            Assert.DoesNotContain(students, student => student.Email == request.Email);
        }

        [Fact]
        public async Task CreateStudent_ShouldReturn409Conflict_WhenEmailAlreadyExists()
        {
            var email = $"duplicate-{Guid.NewGuid():N}@example.com";
            var request = new CreateStudentRequest
            {
                FirstName = "Alam",
                LastName = "Paulino",
                Email = email,
                DateOfBirth = new DateOnly(2008, 10, 8)
            };

            var firstResponse = await _client.PostAsJsonAsync("/api/students", request);
            var duplicateResponse = await _client.PostAsJsonAsync("/api/students", request);
            var responseBody = await duplicateResponse.Content.ReadAsStringAsync();
            var problemDetails = await duplicateResponse.Content.ReadFromJsonAsync<ProblemDetails>();

            Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
            Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
            Assert.Equal("application/json", duplicateResponse.Content.Headers.ContentType?.MediaType);
            Assert.NotNull(problemDetails);
            Assert.Equal((int)HttpStatusCode.Conflict, problemDetails.Status);
            Assert.Equal("Conflict", problemDetails.Title);
            Assert.Equal("A student with this email already exists.", problemDetails.Detail);
            Assert.DoesNotContain("SqlException", responseBody, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("SQL Server", responseBody, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("StudentsDB", responseBody, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Violation of UNIQUE KEY", responseBody, StringComparison.OrdinalIgnoreCase);
        }
    }
}