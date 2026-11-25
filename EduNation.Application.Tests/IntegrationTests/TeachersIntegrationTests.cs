using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using EduNation.Application.SearchTeachers;
using EduNation.Application.Teachers;
using EduNation.Application.Tests.Infrastructure;

namespace EduNation.Application.Tests.Integration
{
    public class TeachersIntegrationTests : IDisposable
    {
        private readonly CustomWebApplicationFactory _factory = new();
        private readonly HttpClient _client;

        public TeachersIntegrationTests()
        {
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Fact]
        public async Task TeacherEndpoints_ShouldCreateReadUpdateAndDeactivate()
        {
            var createResponse = await _client.PostAsJsonAsync("/api/teachers", new TeacherRequest
            {
                FirstName = "Alam",
                LastName = "Paulino",
                Email = $"alam-{Guid.NewGuid():N}@example.com",
                PhoneNumber = "123"
            });

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var created = await createResponse.Content.ReadFromJsonAsync<TeacherResponse>();
            Assert.NotNull(created);

            var getResponse = await _client.GetAsync($"/api/teachers/{created.Id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var updateResponse = await _client.PutAsJsonAsync($"/api/teachers/{created.Id}", new TeacherRequest
            {
                FirstName = "Carlos",
                LastName = "Martinez",
                Email = $"carlos-{Guid.NewGuid():N}@example.com",
                PhoneNumber = "999"
            });
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            var updated = await updateResponse.Content.ReadFromJsonAsync<TeacherResponse>();
            Assert.Equal("Carlos", updated?.FirstName);

            var deactivateResponse = await _client.DeleteAsync($"/api/teachers/{created.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"/api/teachers/{created.Id}")).StatusCode);
        }

        [Fact]
        public async Task CreateTeacher_ShouldReturnConflict_WhenEmailExistsIgnoringCase()
        {
            var email = $"duplicate-{Guid.NewGuid():N}@example.com";
            await _client.PostAsJsonAsync("/api/teachers", new TeacherRequest
            {
                FirstName = "Alam",
                LastName = "Paulino",
                Email = email
            });

            var response = await _client.PostAsJsonAsync("/api/teachers", new TeacherRequest
            {
                FirstName = "Other",
                LastName = "Person",
                Email = email.ToUpperInvariant()
            });
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.Equal("A teacher with this email already exists.", problem?.Detail);
        }

        [Fact]
        public async Task CreateTeacher_ShouldReturnBadRequest_WhenInputIsInvalid()
        {
            var response = await _client.PostAsJsonAsync("/api/teachers", new TeacherRequest
            {
                FirstName = "",
                LastName = "Paulino",
                Email = "alam@example.com"
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchTeachers_ShouldMatchPartialFields()
        {
            var token = Guid.NewGuid().ToString("N");
            await CreateTeacherAsync("Alexandra", "Johnson", $"search-{token}@example.com");

            var firstNameResult = await SearchAsync("Alex");
            var lastNameResult = await SearchAsync("son");
            var emailResult = await SearchAsync(token[..8]);

            Assert.Equal(1, firstNameResult.TotalCount);
            Assert.Equal(1, lastNameResult.TotalCount);
            Assert.Equal(1, emailResult.TotalCount);
        }

        [Fact]
        public async Task SearchTeachers_ShouldExcludeDeactivatedTeachers()
        {
            var teacher = await CreateTeacherAsync("InactiveNeedle", "Teacher", $"inactive-{Guid.NewGuid():N}@example.com");
            await _client.DeleteAsync($"/api/teachers/{teacher.Id}");

            var result = await SearchAsync("InactiveNeedle");

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task SearchTeachers_ShouldPaginate()
        {
            await CreateTeacherAsync("One", "Alpha", $"one-{Guid.NewGuid():N}@example.com");
            await CreateTeacherAsync("Two", "Beta", $"two-{Guid.NewGuid():N}@example.com");
            await CreateTeacherAsync("Three", "Gamma", $"three-{Guid.NewGuid():N}@example.com");

            var page = await SearchAsync(pageNumber: 2, pageSize: 1);

            Assert.Equal(2, page.PageNumber);
            Assert.Equal(1, page.PageSize);
            Assert.Equal(3, page.TotalCount);
            Assert.Single(page.Items);
        }

        private async Task<TeacherResponse> CreateTeacherAsync(string firstName, string lastName, string email)
        {
            var response = await _client.PostAsJsonAsync("/api/teachers", new TeacherRequest
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<TeacherResponse>())!;
        }

        private async Task<PagedResult<TeacherResponse>> SearchAsync(
            string? search = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = new List<string>();
            if (search is not null)
                query.Add($"search={Uri.EscapeDataString(search)}");
            query.Add($"pageNumber={pageNumber}");
            query.Add($"pageSize={pageSize}");

            var response = await _client.GetAsync($"/api/teachers/search?{string.Join("&", query)}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<PagedResult<TeacherResponse>>();
            return result!;
        }
    }
}
