using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using EduNation.Application.AcademicPeriods;
using EduNation.Application.CourseSections;
using EduNation.Application.Courses;
using EduNation.Application.Enrollments;
using EduNation.Application.Responses;
using EduNation.Application.Teachers;
using EduNation.Domain.Entities;
using EduNation.Application.Tests.Infrastructure;

namespace EduNation.Application.Tests.Integration
{
    public class EnrollmentsIntegrationTests : IDisposable
    {
        private readonly CustomWebApplicationFactory _factory = new();
        private readonly HttpClient _client;

        public EnrollmentsIntegrationTests()
        {
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Fact]
        public async Task EnrollmentEndpoints_ShouldCreateReadCancelAndFilter()
        {
            var student = await CreateStudentAsync();
            var courseSection = await CreateCourseSectionAsync();

            var createResponse = await _client.PostAsJsonAsync("/api/enrollments", new EnrollmentRequest
            {
                StudentId = student.Id,
                CourseSectionId = courseSection.Id
            });

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var created = await createResponse.Content.ReadFromJsonAsync<EnrollmentResponse>();
            Assert.NotNull(created);
            Assert.Equal(EnrollmentStatus.Active, created.Status);

            var getResponse = await _client.GetAsync($"/api/enrollments/{created.Id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var byStudentResponse = await _client.GetAsync($"/api/enrollments/student/{student.Id}");
            var byStudent = await byStudentResponse.Content.ReadFromJsonAsync<List<EnrollmentResponse>>();
            Assert.Contains(byStudent!, e => e.Id == created.Id);

            var bySectionResponse = await _client.GetAsync($"/api/enrollments/course-section/{courseSection.Id}");
            var bySection = await bySectionResponse.Content.ReadFromJsonAsync<List<EnrollmentResponse>>();
            Assert.Contains(bySection!, e => e.Id == created.Id);

            var cancelResponse = await _client.DeleteAsync($"/api/enrollments/{created.Id}");
            Assert.Equal(HttpStatusCode.NoContent, cancelResponse.StatusCode);

            var activeResponse = await _client.GetAsync("/api/enrollments");
            var active = await activeResponse.Content.ReadFromJsonAsync<List<EnrollmentResponse>>();
            Assert.DoesNotContain(active!, e => e.Id == created.Id);
        }

        [Fact]
        public async Task CreateEnrollment_ShouldReturnConflict_WhenDuplicateActive()
        {
            var student = await CreateStudentAsync();
            var courseSection = await CreateCourseSectionAsync();

            await CreateEnrollmentAsync(student.Id, courseSection.Id);

            var response = await _client.PostAsJsonAsync("/api/enrollments", new EnrollmentRequest
            {
                StudentId = student.Id,
                CourseSectionId = courseSection.Id
            });

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task CreateEnrollment_ShouldAllowNewEnrollment_AfterCancellation()
        {
            var student = await CreateStudentAsync();
            var courseSection = await CreateCourseSectionAsync();

            var first = await CreateEnrollmentAsync(student.Id, courseSection.Id);
            Assert.Equal(HttpStatusCode.NoContent, (await _client.DeleteAsync($"/api/enrollments/{first.Id}")).StatusCode);

            var response = await _client.PostAsJsonAsync("/api/enrollments", new EnrollmentRequest
            {
                StudentId = student.Id,
                CourseSectionId = courseSection.Id
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateEnrollment_ShouldReturnConflict_WhenCapacityIsReached()
        {
            var course = await CreateCourseAsync();
            var period = await CreatePeriodAsync();
            var teacher = await CreateTeacherAsync();
            var courseSection = await CreateCourseSectionAsync(course.Id, period.Id, teacher.Id, 1);

            var student1 = await CreateStudentAsync();
            var student2 = await CreateStudentAsync();

            await CreateEnrollmentAsync(student1.Id, courseSection.Id);

            var response = await _client.PostAsJsonAsync("/api/enrollments", new EnrollmentRequest
            {
                StudentId = student2.Id,
                CourseSectionId = courseSection.Id
            });

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task CreateEnrollment_ShouldReturnNotFound_WhenReferenceIsMissing()
        {
            var student = await CreateStudentAsync();

            var response = await _client.PostAsJsonAsync("/api/enrollments", new EnrollmentRequest
            {
                StudentId = student.Id,
                CourseSectionId = Guid.NewGuid()
            });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CancelEnrollment_ShouldReturnConflict_WhenAlreadyCancelled()
        {
            var student = await CreateStudentAsync();
            var courseSection = await CreateCourseSectionAsync();
            var enrollment = await CreateEnrollmentAsync(student.Id, courseSection.Id);

            Assert.Equal(HttpStatusCode.NoContent, (await _client.DeleteAsync($"/api/enrollments/{enrollment.Id}")).StatusCode);

            var response = await _client.DeleteAsync($"/api/enrollments/{enrollment.Id}");
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        private async Task<StudentResponse> CreateStudentAsync()
        {
            var response = await _client.PostAsJsonAsync("/api/students", new
            {
                FirstName = "Alam",
                LastName = "Paulino",
                Email = $"alam{Guid.NewGuid():N}@example.com",
                DateOfBirth = new DateOnly(2008, 10, 8)
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<StudentResponse>())!;
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

        private async Task<EduNation.Application.Teachers.TeacherResponse> CreateTeacherAsync()
        {
            var response = await _client.PostAsJsonAsync("/api/teachers", new TeacherRequest
            {
                FirstName = "Alam",
                LastName = "Paulino",
                Email = $"alam-{Guid.NewGuid():N}@example.com"
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<EduNation.Application.Teachers.TeacherResponse>())!;
        }

        private async Task<CourseSectionResponse> CreateCourseSectionAsync(Guid? courseId = null, Guid? periodId = null, Guid? teacherId = null, int capacity = 30)
        {
            if (courseId is null)
            {
                var course = await CreateCourseAsync();
                courseId = course.Id;
            }
            if (periodId is null)
            {
                var period = await CreatePeriodAsync();
                periodId = period.Id;
            }
            if (teacherId is null)
            {
                var teacher = await CreateTeacherAsync();
                teacherId = teacher.Id;
            }

            var response = await _client.PostAsJsonAsync("/api/coursesections", new CourseSectionRequest
            {
                CourseId = courseId.Value,
                AcademicPeriodId = periodId.Value,
                TeacherId = teacherId.Value,
                Code = $"SEC-{Guid.NewGuid():N}".Substring(0, 8),
                Capacity = capacity
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<CourseSectionResponse>())!;
        }

        private async Task<EnrollmentResponse> CreateEnrollmentAsync(Guid studentId, Guid courseSectionId)
        {
            var response = await _client.PostAsJsonAsync("/api/enrollments", new EnrollmentRequest
            {
                StudentId = studentId,
                CourseSectionId = courseSectionId
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<EnrollmentResponse>())!;
        }
    }
}
