using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using EduNation.Application.AcademicPeriods;
using EduNation.Application.CourseSections;
using EduNation.Application.Courses;
using EduNation.Application.Teachers;
using EduNation.Application.Tests.Infrastructure;

namespace EduNation.Application.Tests.Integration
{
    public class CourseSectionsIntegrationTests : IDisposable
    {
        private readonly CustomWebApplicationFactory _factory = new();
        private readonly HttpClient _client;

        public CourseSectionsIntegrationTests()
        {
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Fact]
        public async Task CourseSectionEndpoints_ShouldCreateReadUpdateAndDeactivate()
        {
            var course = await CreateCourseAsync();
            var period = await CreatePeriodAsync();
            var teacher = await CreateTeacherAsync();

            var createResponse = await _client.PostAsJsonAsync("/api/coursesections", new CourseSectionRequest
            {
                CourseId = course.Id,
                AcademicPeriodId = period.Id,
                TeacherId = teacher.Id,
                Code = "sec-1",
                Name = "Morning",
                Capacity = 30
            });

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var created = await createResponse.Content.ReadFromJsonAsync<CourseSectionResponse>();
            Assert.NotNull(created);
            Assert.Equal("SEC-1", created.Code);

            var getResponse = await _client.GetAsync($"/api/coursesections/{created.Id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var updateResponse = await _client.PutAsJsonAsync($"/api/coursesections/{created.Id}", new CourseSectionRequest
            {
                CourseId = course.Id,
                AcademicPeriodId = period.Id,
                TeacherId = teacher.Id,
                Code = "sec-2",
                Name = "Evening",
                Capacity = 40
            });
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            var updated = await updateResponse.Content.ReadFromJsonAsync<CourseSectionResponse>();
            Assert.Equal("SEC-2", updated?.Code);
            Assert.Equal(40, updated?.Capacity);

            var deactivateResponse = await _client.DeleteAsync($"/api/coursesections/{created.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"/api/coursesections/{created.Id}")).StatusCode);
        }

        [Fact]
        public async Task CreateCourseSection_ShouldReturnConflict_WhenCodeExistsForSameCourseAndPeriod()
        {
            var course = await CreateCourseAsync();
            var period = await CreatePeriodAsync();
            var teacher = await CreateTeacherAsync();

            await CreateSectionAsync(course.Id, period.Id, teacher.Id, "sec-1");

            var response = await _client.PostAsJsonAsync("/api/coursesections", new CourseSectionRequest
            {
                CourseId = course.Id,
                AcademicPeriodId = period.Id,
                TeacherId = teacher.Id,
                Code = " sec-1 ",
                Capacity = 30
            });

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task CreateCourseSection_ShouldReturnBadRequest_WhenCapacityIsInvalid()
        {
            var course = await CreateCourseAsync();
            var period = await CreatePeriodAsync();
            var teacher = await CreateTeacherAsync();

            var response = await _client.PostAsJsonAsync("/api/coursesections", new CourseSectionRequest
            {
                CourseId = course.Id,
                AcademicPeriodId = period.Id,
                TeacherId = teacher.Id,
                Code = "SEC-1",
                Capacity = 0
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateCourseSection_ShouldReturnNotFound_WhenReferenceIsMissing()
        {
            var course = await CreateCourseAsync();
            var period = await CreatePeriodAsync();
            var teacher = await CreateTeacherAsync();

            var response = await _client.PostAsJsonAsync("/api/coursesections", new CourseSectionRequest
            {
                CourseId = Guid.NewGuid(),
                AcademicPeriodId = period.Id,
                TeacherId = teacher.Id,
                Code = "SEC-1",
                Capacity = 30
            });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetCourseSectionsByCourse_ShouldReturnOnlyMatching()
        {
            var course1 = await CreateCourseAsync();
            var course2 = await CreateCourseAsync();
            var period = await CreatePeriodAsync();
            var teacher = await CreateTeacherAsync();

            var section = await CreateSectionAsync(course1.Id, period.Id, teacher.Id, "SEC-1");

            var response = await _client.GetAsync($"/api/coursesections/course/{course1.Id}");
            var sections = await response.Content.ReadFromJsonAsync<List<CourseSectionResponse>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains(sections!, s => s.Id == section.Id);

            var response2 = await _client.GetAsync($"/api/coursesections/course/{course2.Id}");
            var sections2 = await response2.Content.ReadFromJsonAsync<List<CourseSectionResponse>>();
            Assert.DoesNotContain(sections2!, s => s.Id == section.Id);
        }

        [Fact]
        public async Task GetCourseSectionsByAcademicPeriod_ShouldReturnOnlyMatching()
        {
            var course = await CreateCourseAsync();
            var period1 = await CreatePeriodAsync();
            var period2 = await CreatePeriodAsync();
            var teacher = await CreateTeacherAsync();

            var section = await CreateSectionAsync(course.Id, period1.Id, teacher.Id, "SEC-1");

            var response = await _client.GetAsync($"/api/coursesections/academic-period/{period1.Id}");
            var sections = await response.Content.ReadFromJsonAsync<List<CourseSectionResponse>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains(sections!, s => s.Id == section.Id);

            var response2 = await _client.GetAsync($"/api/coursesections/academic-period/{period2.Id}");
            var sections2 = await response2.Content.ReadFromJsonAsync<List<CourseSectionResponse>>();
            Assert.DoesNotContain(sections2!, s => s.Id == section.Id);
        }

        [Fact]
        public async Task GetCourseSectionsByTeacher_ShouldReturnOnlyMatching()
        {
            var course = await CreateCourseAsync();
            var period = await CreatePeriodAsync();
            var teacher1 = await CreateTeacherAsync();
            var teacher2 = await CreateTeacherAsync();

            var section = await CreateSectionAsync(course.Id, period.Id, teacher1.Id, "SEC-1");

            var response = await _client.GetAsync($"/api/coursesections/teacher/{teacher1.Id}");
            var sections = await response.Content.ReadFromJsonAsync<List<CourseSectionResponse>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains(sections!, s => s.Id == section.Id);

            var response2 = await _client.GetAsync($"/api/coursesections/teacher/{teacher2.Id}");
            var sections2 = await response2.Content.ReadFromJsonAsync<List<CourseSectionResponse>>();
            Assert.DoesNotContain(sections2!, s => s.Id == section.Id);
        }

        private async Task<CourseResponse> CreateCourseAsync()
        {
            var response = await _client.PostAsJsonAsync("/api/courses", new CourseRequest
            {
                Code = $"CS-{Guid.NewGuid():N}".Substring(0, 8),
                Name = "Computer Science",
                Credits = 3
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<CourseResponse>())!;
        }

        private async Task<AcademicPeriodResponse> CreatePeriodAsync()
        {
            var response = await _client.PostAsJsonAsync("/api/academicperiods", new AcademicPeriodRequest
            {
                Code = $"FALL-{Guid.NewGuid():N}".Substring(0, 10),
                Name = "Fall 2025",
                StartDate = new DateOnly(2025, 9, 1),
                EndDate = new DateOnly(2025, 12, 15)
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<AcademicPeriodResponse>())!;
        }

        private async Task<TeacherResponse> CreateTeacherAsync()
        {
            var response = await _client.PostAsJsonAsync("/api/teachers", new TeacherRequest
            {
                FirstName = "Alam",
                LastName = "Paulino",
                Email = $"alam-{Guid.NewGuid():N}@example.com"
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<TeacherResponse>())!;
        }

        private async Task<CourseSectionResponse> CreateSectionAsync(Guid courseId, Guid periodId, Guid teacherId, string code)
        {
            var response = await _client.PostAsJsonAsync("/api/coursesections", new CourseSectionRequest
            {
                CourseId = courseId,
                AcademicPeriodId = periodId,
                TeacherId = teacherId,
                Code = code,
                Capacity = 30
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<CourseSectionResponse>())!;
        }
    }
}
