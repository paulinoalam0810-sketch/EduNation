using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EduNation.Application.CreateStudent;
using EduNation.Application.Tests.Infrastructure;
using EduNation.Application.Responses;
using EduNation.Application.SearchStudents;
using EduNation.Application.UpdateStudent;
using EduNation.Infrastructure.Persistence;
using System.Net;
using System.Net.Http.Json;

namespace EduNation.Application.Tests.Integration
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

        [Fact]
        public async Task SearchStudents_ShouldMatchFirstNameLastNameAndEmailPartially()
        {
            var token = Guid.NewGuid().ToString("N");
            var student = await CreateStudentAsync("Alexandra", "Johnson", $"search-{token}@example.com");

            var firstNameResult = await SearchAsync("Alex");
            var lastNameResult = await SearchAsync("son");
            var emailResult = await SearchAsync(token[..8]);

            Assert.Equal(student.Id, Assert.Single(firstNameResult.Items).Id);
            Assert.Equal(student.Id, Assert.Single(lastNameResult.Items).Id);
            Assert.Equal(student.Id, Assert.Single(emailResult.Items).Id);
        }

        [Fact]
        public async Task SearchStudents_ShouldReturnEmptyPage_WhenThereAreNoMatches()
        {
            var result = await SearchAsync($"missing-{Guid.NewGuid():N}");

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
        }

        [Fact]
        public async Task SearchStudents_ShouldExcludeDeactivatedStudents()
        {
            var student = await CreateStudentAsync("InactiveNeedle", "Student", $"inactive-{Guid.NewGuid():N}@example.com");
            var deactivateResponse = await _client.DeleteAsync($"/api/students/{student.Id}");
            var result = await SearchAsync("InactiveNeedle");

            Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task SearchStudents_ShouldUseDefaultPageNumberAndPageSize()
        {
            await CreateStudentAsync("First", "Alpha", $"first-{Guid.NewGuid():N}@example.com");
            await CreateStudentAsync("Second", "Beta", $"second-{Guid.NewGuid():N}@example.com");

            var result = await SearchAsync();

            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task SearchStudents_ShouldUseDefaultsAndReturnTotalCount_WhenNoQueryParametersAreProvided()
        {
            await CreateStudentAsync("First", "Alpha", $"first-{Guid.NewGuid():N}@example.com");
            await CreateStudentAsync("Second", "Beta", $"second-{Guid.NewGuid():N}@example.com");

            var response = await _client.GetAsync("/api/students/search");
            var result = await response.Content.ReadFromJsonAsync<PagedResult<StudentResponse>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task SearchStudents_ShouldNavigateBetweenPages()
        {
            await CreateStudentAsync("One", "Alpha", $"one-{Guid.NewGuid():N}@example.com");
            await CreateStudentAsync("Two", "Beta", $"two-{Guid.NewGuid():N}@example.com");
            await CreateStudentAsync("Three", "Gamma", $"three-{Guid.NewGuid():N}@example.com");

            var page = await SearchAsync(pageNumber: 2, pageSize: 1);

            Assert.Equal(2, page.PageNumber);
            Assert.Equal(1, page.PageSize);
            Assert.Equal(3, page.TotalCount);
            Assert.Equal("Beta", Assert.Single(page.Items).LastName);
        }

        [Fact]
        public async Task SearchStudents_ShouldLimitResultsToOneHundred_WhenMoreAreAvailable()
        {
            for (var index = 0; index < 101; index++)
            {
                await CreateStudentAsync(
                    $"Student{index:000}",
                    "PageLimit",
                    $"page-limit-{Guid.NewGuid():N}@example.com");
            }

            var result = await SearchAsync(pageSize: 100);

            Assert.Equal(100, result.PageSize);
            Assert.Equal(101, result.TotalCount);
            Assert.Equal(100, result.Items.Count);
            Assert.True(result.Items.Count <= 100);
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(-1, 10)]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        [InlineData(1, 101)]
        public async Task SearchStudents_ShouldReturnBadRequestForInvalidPagination(int pageNumber, int pageSize)
        {
            var response = await _client.GetAsync(
                $"/api/students/search?pageNumber={pageNumber}&pageSize={pageSize}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("pageNumber=not-a-number")]
        [InlineData("pageSize=not-a-number")]
        public async Task SearchStudents_ShouldReturnBadRequestForNonNumericPagination(string queryParameter)
        {
            var response = await _client.GetAsync($"/api/students/search?{queryParameter}");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchStudents_ShouldTreatWhitespaceOnlySearchAsNoFilter()
        {
            await CreateStudentAsync("First", "Alpha", $"whitespace1-{Guid.NewGuid():N}@example.com");
            await CreateStudentAsync("Second", "Beta", $"whitespace2-{Guid.NewGuid():N}@example.com");

            var result = await SearchAsync("   ");

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count);
        }

        [Fact]
        public async Task SearchStudents_ShouldUseStableLastNameFirstNameAndIdOrdering()
        {
            await CreateStudentAsync("Amy", "Baker", $"tie1-{Guid.NewGuid():N}@example.com");
            await CreateStudentAsync("Amy", "Baker", $"tie2-{Guid.NewGuid():N}@example.com");
            await CreateStudentAsync("Ada", "Zeta", $"later-{Guid.NewGuid():N}@example.com");

            var result = await SearchAsync();
            var actualIds = result.Items.Select(student => student.Id).ToList();
            var expectedIds = await GetStudentIdsInDatabaseOrderAsync();

            Assert.Equal(expectedIds, actualIds);
        }

        [Fact]
        public async Task SearchStudents_ShouldCountOnlyFilteredStudentsAcrossPages()
        {
            await CreateStudentAsync("FilterKey One", "Alpha", $"match1-{Guid.NewGuid():N}@example.com");
            await CreateStudentAsync("FilterKey Two", "Beta", $"match2-{Guid.NewGuid():N}@example.com");
            await CreateStudentAsync("Other", "Gamma", $"other-{Guid.NewGuid():N}@example.com");

            var result = await SearchAsync("FilterKey", pageNumber: 2, pageSize: 1);

            Assert.Equal(2, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal(2, result.PageNumber);
            Assert.Equal("FilterKey Two", result.Items[0].FirstName);
        }

        private async Task<CreateStudentResponse> CreateStudentAsync(string firstName, string lastName, string email)
        {
            var request = new CreateStudentRequest
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                DateOfBirth = new DateOnly(2000, 1, 1)
            };

            var response = await _client.PostAsJsonAsync("/api/students", request);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var student = await response.Content.ReadFromJsonAsync<CreateStudentResponse>();
            return Assert.IsType<CreateStudentResponse>(student);
        }

        private async Task<PagedResult<StudentResponse>> SearchAsync(
            string? search = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = new List<string>();
            if (search is not null)
            {
                query.Add($"search={Uri.EscapeDataString(search)}");
            }

            query.Add($"pageNumber={pageNumber}");
            query.Add($"pageSize={pageSize}");

            var response = await _client.GetAsync($"/api/students/search?{string.Join("&", query)}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<PagedResult<StudentResponse>>();
            return Assert.IsType<PagedResult<StudentResponse>>(result);
        }

        private async Task<List<Guid>> GetStudentIdsInDatabaseOrderAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            return await dbContext.Students
                .AsNoTracking()
                .Where(student => student.IsActive)
                .OrderBy(student => student.LastName)
                .ThenBy(student => student.FirstName)
                .ThenBy(student => student.Id)
                .Select(student => student.Id)
                .ToListAsync();
        }

    }
}
