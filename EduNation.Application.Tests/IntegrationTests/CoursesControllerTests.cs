using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using EduNation.Application.Courses;
using EduNation.Application.Tests.Infrastructure;

namespace EduNation.Application.Tests.Integration
{
    public class CoursesControllerTests : IDisposable
    {
        private readonly CustomWebApplicationFactory _factory = new();
        private readonly HttpClient _client;

        public CoursesControllerTests()
        {
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Fact]
        public async Task CourseEndpoints_ShouldCreateReadUpdateAndDeactivateCourse()
        {
            var createResponse = await _client.PostAsJsonAsync("/api/courses", new CourseRequest
            {
                Code = "cs-101", Name = "Computer Science", Description = "Intro course", Credits = 3
            });

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var created = await createResponse.Content.ReadFromJsonAsync<CourseResponse>();
            Assert.NotNull(created);
            Assert.Equal("CS-101", created.Code);

            var getResponse = await _client.GetAsync($"/api/courses/{created.Id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var retrieved = await getResponse.Content.ReadFromJsonAsync<CourseResponse>();
            Assert.Equal(created.Id, retrieved?.Id);

            var updateResponse = await _client.PutAsJsonAsync($"/api/courses/{created.Id}", new CourseRequest
            {
                Code = "cs-102", Name = "Computer Science II", Description = null, Credits = 4
            });
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            var updated = await updateResponse.Content.ReadFromJsonAsync<CourseResponse>();
            Assert.Equal("CS-102", updated?.Code);
            Assert.Equal(4, updated?.Credits);

            var deactivateResponse = await _client.DeleteAsync($"/api/courses/{created.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"/api/courses/{created.Id}")).StatusCode);
        }

        [Fact]
        public async Task GetAllCourses_ShouldReturnOnlyActiveCourses()
        {
            var first = await CreateCourseAsync("CS-101");
            var second = await CreateCourseAsync("MATH-101");
            Assert.Equal(HttpStatusCode.NoContent, (await _client.DeleteAsync($"/api/courses/{first.Id}")).StatusCode);

            var response = await _client.GetAsync("/api/courses");
            var courses = await response.Content.ReadFromJsonAsync<List<CourseResponse>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(courses);
            Assert.Equal(second.Id, Assert.Single(courses).Id);
        }

        [Fact]
        public async Task CreateCourse_ShouldReturnConflict_WhenCodeExistsIgnoringCase()
        {
            await CreateCourseAsync("CS-101");
            var response = await _client.PostAsJsonAsync("/api/courses", new CourseRequest
            {
                Code = " cs-101 ", Name = "Duplicate", Credits = 2
            });
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.Equal("A course with this code already exists.", problem?.Detail);
        }

        [Theory]
        [InlineData("", "Valid name", 3)]
        [InlineData("CS-101", "", 3)]
        [InlineData("CS-101", "Valid name", 0)]
        [InlineData("CS-101", "Valid name", 31)]
        public async Task CreateCourse_ShouldReturnBadRequest_WhenInputIsInvalid(string code, string name, int credits)
        {
            var response = await _client.PostAsJsonAsync("/api/courses", new CourseRequest
            {
                Code = code, Name = name, Credits = credits
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCourse_ShouldReturnConflict_WhenCodeBelongsToAnotherCourse()
        {
            var existing = await CreateCourseAsync("CS-101");
            var target = await CreateCourseAsync("MATH-101");

            var response = await _client.PutAsJsonAsync($"/api/courses/{target.Id}", new CourseRequest
            {
                Code = existing.Code, Name = "Math", Credits = 3
            });

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        private async Task<CourseResponse> CreateCourseAsync(string code)
        {
            var response = await _client.PostAsJsonAsync("/api/courses", new CourseRequest
            {
                Code = code, Name = $"Course {code}", Credits = 3
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return Assert.IsType<CourseResponse>(await response.Content.ReadFromJsonAsync<CourseResponse>());
        }
    }
}
