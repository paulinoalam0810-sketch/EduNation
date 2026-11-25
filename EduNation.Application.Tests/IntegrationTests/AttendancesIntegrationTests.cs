using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using EduNation.Application.AcademicPeriods;
using EduNation.Application.Attendances;
using EduNation.Application.CourseSections;
using EduNation.Application.Courses;
using EduNation.Application.Enrollments;
using EduNation.Application.Responses;
using EduNation.Application.SearchAttendances;
using EduNation.Application.Teachers;
using EduNation.Domain.Entities;
using EduNation.Application.Tests.Infrastructure;

namespace EduNation.Application.Tests.Integration
{
    public class AttendancesIntegrationTests : IDisposable
    {
        private readonly CustomWebApplicationFactory _factory = new();
        private readonly HttpClient _client;

        public AttendancesIntegrationTests()
        {
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Fact]
        public async Task AttendanceEndpoints_ShouldCreateReadAndUpdate()
        {
            var enrollment = await CreateEnrollmentAsync();

            var createResponse = await _client.PostAsJsonAsync("/api/attendances", new AttendanceRequest
            {
                EnrollmentId = enrollment.Id,
                Date = new DateOnly(2025, 9, 1),
                Status = AttendanceStatus.Present
            });

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var created = await createResponse.Content.ReadFromJsonAsync<AttendanceResponse>();
            Assert.NotNull(created);

            var getResponse = await _client.GetAsync($"/api/attendances/{created.Id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var updateResponse = await _client.PutAsJsonAsync($"/api/attendances/{created.Id}", (int)AttendanceStatus.Late);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            var updated = await updateResponse.Content.ReadFromJsonAsync<AttendanceResponse>();
            Assert.Equal(AttendanceStatus.Late, updated?.Status);
        }

        [Fact]
        public async Task CreateAttendance_ShouldReturnConflict_WhenDuplicate()
        {
            var enrollment = await CreateEnrollmentAsync();
            var date = new DateOnly(2025, 9, 1);

            await CreateAttendanceAsync(enrollment.Id, date, AttendanceStatus.Present);

            var response = await _client.PostAsJsonAsync("/api/attendances", new AttendanceRequest
            {
                EnrollmentId = enrollment.Id,
                Date = date,
                Status = AttendanceStatus.Absent
            });

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task CreateAttendance_ShouldReturnNotFound_WhenEnrollmentIsMissing()
        {
            var response = await _client.PostAsJsonAsync("/api/attendances", new AttendanceRequest
            {
                EnrollmentId = Guid.NewGuid(),
                Date = new DateOnly(2025, 9, 1),
                Status = AttendanceStatus.Present
            });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateAttendance_ShouldReturnNotFound_WhenEnrollmentIsCancelled()
        {
            var enrollment = await CreateEnrollmentAsync();
            await _client.DeleteAsync($"/api/enrollments/{enrollment.Id}");

            var response = await _client.PostAsJsonAsync("/api/attendances", new AttendanceRequest
            {
                EnrollmentId = enrollment.Id,
                Date = new DateOnly(2025, 9, 1),
                Status = AttendanceStatus.Present
            });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAttendanceByStudent_ShouldReturnMatchingRecords()
        {
            var enrollment = await CreateEnrollmentAsync();
            var studentId = enrollment.StudentId;
            var attendance = await CreateAttendanceAsync(enrollment.Id, new DateOnly(2025, 9, 1), AttendanceStatus.Present);

            var response = await _client.GetAsync($"/api/attendances/student/{studentId}");
            var attendances = await response.Content.ReadFromJsonAsync<List<AttendanceResponse>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains(attendances!, a => a.Id == attendance.Id);
        }

        [Fact]
        public async Task GetAttendanceByCourseSectionAndDate_ShouldReturnMatchingRecords()
        {
            var enrollment = await CreateEnrollmentAsync();
            var courseSectionId = enrollment.CourseSectionId!.Value;
            var date = new DateOnly(2025, 9, 1);
            var attendance = await CreateAttendanceAsync(enrollment.Id, date, AttendanceStatus.Present);

            var response = await _client.GetAsync($"/api/attendances/course-section/{courseSectionId}/date/{date:yyyy-MM-dd}");
            var attendances = await response.Content.ReadFromJsonAsync<List<AttendanceResponse>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains(attendances!, a => a.Id == attendance.Id);
        }

        [Fact]
        public async Task SearchAttendances_ShouldFilterByStatusAndPaginate()
        {
            var enrollment = await CreateEnrollmentAsync();
            await CreateAttendanceAsync(enrollment.Id, new DateOnly(2025, 9, 1), AttendanceStatus.Present);
            await CreateAttendanceAsync(enrollment.Id, new DateOnly(2025, 9, 2), AttendanceStatus.Absent);

            var response = await _client.GetAsync($"/api/attendances/search?status=0&pageNumber=1&pageSize=10");
            var result = await response.Content.ReadFromJsonAsync<PagedResult<AttendanceResponse>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.True(result.TotalCount >= 1);
            Assert.All(result.Items, a => Assert.Equal(AttendanceStatus.Present, a.Status));
        }

        private async Task<EnrollmentResponse> CreateEnrollmentAsync()
        {
            var studentResponse = await _client.PostAsJsonAsync("/api/students", new
            {
                FirstName = "Alam",
                LastName = "Paulino",
                Email = $"alam{Guid.NewGuid():N}@example.com",
                DateOfBirth = new DateOnly(2008, 10, 8)
            });
            var student = await studentResponse.Content.ReadFromJsonAsync<StudentResponse>();

            var courseResponse = await _client.PostAsJsonAsync("/api/courses", new CourseRequest
            {
                Code = $"CS-{Guid.NewGuid():N}".Substring(0, 8),
                Name = "Computer Science",
                Credits = 3
            });
            var course = await courseResponse.Content.ReadFromJsonAsync<CourseResponse>();

            var periodResponse = await _client.PostAsJsonAsync("/api/academicperiods", new AcademicPeriodRequest
            {
                Code = $"FALL-{Guid.NewGuid():N}".Substring(0, 10),
                Name = "Fall 2025",
                StartDate = new DateOnly(2025, 9, 1),
                EndDate = new DateOnly(2025, 12, 15)
            });
            var period = await periodResponse.Content.ReadFromJsonAsync<AcademicPeriodResponse>();

            var teacherResponse = await _client.PostAsJsonAsync("/api/teachers", new TeacherRequest
            {
                FirstName = "Alam",
                LastName = "Paulino",
                Email = $"alam-{Guid.NewGuid():N}@example.com"
            });
            var teacher = await teacherResponse.Content.ReadFromJsonAsync<EduNation.Application.Teachers.TeacherResponse>();

            var sectionResponse = await _client.PostAsJsonAsync("/api/coursesections", new CourseSectionRequest
            {
                CourseId = course!.Id,
                AcademicPeriodId = period!.Id,
                TeacherId = teacher!.Id,
                Code = $"SEC-{Guid.NewGuid():N}".Substring(0, 8),
                Capacity = 30
            });
            var section = await sectionResponse.Content.ReadFromJsonAsync<CourseSectionResponse>();

            var enrollmentResponse = await _client.PostAsJsonAsync("/api/enrollments", new EnrollmentRequest
            {
                StudentId = student!.Id,
                CourseSectionId = section!.Id
            });
            Assert.Equal(HttpStatusCode.Created, enrollmentResponse.StatusCode);
            return (await enrollmentResponse.Content.ReadFromJsonAsync<EnrollmentResponse>())!;
        }

        private async Task<AttendanceResponse> CreateAttendanceAsync(Guid enrollmentId, DateOnly date, AttendanceStatus status)
        {
            var response = await _client.PostAsJsonAsync("/api/attendances", new AttendanceRequest
            {
                EnrollmentId = enrollmentId,
                Date = date,
                Status = status
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<AttendanceResponse>())!;
        }
    }
}
