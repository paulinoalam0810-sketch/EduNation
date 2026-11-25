using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using EduNation.Application.AcademicPeriods;
using EduNation.Application.CourseSections;
using EduNation.Application.Courses;
using EduNation.Application.Enrollments;
using EduNation.Application.Grades;
using EduNation.Application.Responses;
using EduNation.Application.Teachers;
using EduNation.Domain.Entities;
using EduNation.Application.Tests.Infrastructure;

namespace EduNation.Application.Tests.Integration
{
    public class GradesIntegrationTests : IDisposable
    {
        private readonly CustomWebApplicationFactory _factory = new();
        private readonly HttpClient _client;

        public GradesIntegrationTests()
        {
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Fact]
        public async Task EvaluationAndGradeEndpoints_ShouldCreateReadUpdateAndDeactivate()
        {
            var courseSection = await CreateCourseSectionAsync();
            var enrollment = await CreateEnrollmentAsync(courseSection);

            var evalResponse = await _client.PostAsJsonAsync("/api/evaluations", new EvaluationRequest
            {
                CourseSectionId = courseSection.Id,
                Name = "Midterm",
                Description = "Midterm exam",
                MaxScore = 100,
                Weight = 30,
                EvaluationDate = new DateOnly(2025, 10, 1)
            });
            Assert.Equal(HttpStatusCode.Created, evalResponse.StatusCode);
            var evaluation = await evalResponse.Content.ReadFromJsonAsync<EvaluationResponse>();
            Assert.NotNull(evaluation);

            var gradeResponse = await _client.PostAsJsonAsync("/api/grades", new GradeRequest
            {
                EvaluationId = evaluation.Id,
                EnrollmentId = enrollment.Id,
                Score = 85,
                Comments = "Good"
            });
            Assert.Equal(HttpStatusCode.Created, gradeResponse.StatusCode);
            var grade = await gradeResponse.Content.ReadFromJsonAsync<GradeResponse>();
            Assert.NotNull(grade);

            var updateEvalResponse = await _client.PutAsJsonAsync($"/api/evaluations/{evaluation.Id}", new EvaluationRequest
            {
                CourseSectionId = courseSection.Id,
                Name = "Midterm Updated",
                MaxScore = 100,
                Weight = 40,
                EvaluationDate = new DateOnly(2025, 10, 2)
            });
            Assert.Equal(HttpStatusCode.OK, updateEvalResponse.StatusCode);

            var deactivateResponse = await _client.DeleteAsync($"/api/evaluations/{evaluation.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);
        }

        [Fact]
        public async Task CreateGrade_ShouldReturnConflict_WhenDuplicate()
        {
            var courseSection = await CreateCourseSectionAsync();
            var enrollment = await CreateEnrollmentAsync(courseSection);
            var evaluation = await CreateEvaluationAsync(courseSection.Id);

            await CreateGradeAsync(evaluation.Id, enrollment.Id, 80);

            var response = await _client.PostAsJsonAsync("/api/grades", new GradeRequest
            {
                EvaluationId = evaluation.Id,
                EnrollmentId = enrollment.Id,
                Score = 90
            });

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task CreateGrade_ShouldReturnBadRequest_WhenScoreExceedsMax()
        {
            var courseSection = await CreateCourseSectionAsync();
            var enrollment = await CreateEnrollmentAsync(courseSection);
            var evaluation = await CreateEvaluationAsync(courseSection.Id);

            var response = await _client.PostAsJsonAsync("/api/grades", new GradeRequest
            {
                EvaluationId = evaluation.Id,
                EnrollmentId = enrollment.Id,
                Score = 101
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateGrade_ShouldReturnConflict_WhenEnrollmentInDifferentSection()
        {
            var courseSection1 = await CreateCourseSectionAsync();
            var courseSection2 = await CreateCourseSectionAsync();
            var enrollment = await CreateEnrollmentAsync(courseSection2);
            var evaluation = await CreateEvaluationAsync(courseSection1.Id);

            var response = await _client.PostAsJsonAsync("/api/grades", new GradeRequest
            {
                EvaluationId = evaluation.Id,
                EnrollmentId = enrollment.Id,
                Score = 80
            });

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task CreateGrade_ShouldReturnNotFound_WhenEvaluationIsInactive()
        {
            var courseSection = await CreateCourseSectionAsync();
            var enrollment = await CreateEnrollmentAsync(courseSection);
            var evaluation = await CreateEvaluationAsync(courseSection.Id);

            await _client.DeleteAsync($"/api/evaluations/{evaluation.Id}");

            var response = await _client.PostAsJsonAsync("/api/grades", new GradeRequest
            {
                EvaluationId = evaluation.Id,
                EnrollmentId = enrollment.Id,
                Score = 80
            });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateGrade_ShouldUpdateScoreAndComments()
        {
            var courseSection = await CreateCourseSectionAsync();
            var enrollment = await CreateEnrollmentAsync(courseSection);
            var evaluation = await CreateEvaluationAsync(courseSection.Id);
            var grade = await CreateGradeAsync(evaluation.Id, enrollment.Id, 80);

            var response = await _client.PutAsJsonAsync($"/api/grades/{grade.Id}", new UpdateGradeRequest
            {
                Score = 90,
                Comments = "Improved"
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var updated = await response.Content.ReadFromJsonAsync<GradeResponse>();
            Assert.Equal(90, updated?.Score);
            Assert.Equal("Improved", updated?.Comments);
        }

        [Fact]
        public async Task SearchGrades_ShouldFilterByStudent()
        {
            var courseSection = await CreateCourseSectionAsync();
            var enrollment = await CreateEnrollmentAsync(courseSection);
            var evaluation = await CreateEvaluationAsync(courseSection.Id);
            var grade = await CreateGradeAsync(evaluation.Id, enrollment.Id, 80);

            var response = await _client.GetAsync($"/api/grades/search?studentId={enrollment.StudentId}");
            var result = await response.Content.ReadFromJsonAsync<PagedResult<GradeResponse>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Contains(result.Items, g => g.Id == grade.Id);
        }

        [Fact]
        public async Task GetStudentSectionResult_ShouldReturnWeightedScore()
        {
            var courseSection = await CreateCourseSectionAsync();
            var enrollment = await CreateEnrollmentAsync(courseSection);
            var evaluation = await CreateEvaluationAsync(courseSection.Id, 100, 50);
            await CreateGradeAsync(evaluation.Id, enrollment.Id, 80);

            var response = await _client.GetAsync($"/api/grades/results/student/{enrollment.StudentId}/course-section/{courseSection.Id}");
            var result = await response.Content.ReadFromJsonAsync<StudentSectionResultResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(80, result.WeightedScore);
            Assert.Equal(1, result.GradesCount);
        }

        private async Task<CourseSectionResponse> CreateCourseSectionAsync()
        {
            var course = await CreateCourseAsync();
            var period = await CreatePeriodAsync();
            var teacher = await CreateTeacherAsync();

            var response = await _client.PostAsJsonAsync("/api/coursesections", new CourseSectionRequest
            {
                CourseId = course.Id,
                AcademicPeriodId = period.Id,
                TeacherId = teacher.Id,
                Code = $"SEC-{Guid.NewGuid():N}".Substring(0, 8),
                Capacity = 30
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<CourseSectionResponse>())!;
        }

        private async Task<EnrollmentResponse> CreateEnrollmentAsync(CourseSectionResponse courseSection)
        {
            var studentResponse = await _client.PostAsJsonAsync("/api/students", new
            {
                FirstName = "Alam",
                LastName = "Paulino",
                Email = $"alam{Guid.NewGuid():N}@example.com",
                DateOfBirth = new DateOnly(2008, 10, 8)
            });
            var student = await studentResponse.Content.ReadFromJsonAsync<StudentResponse>();

            var enrollmentResponse = await _client.PostAsJsonAsync("/api/enrollments", new EnrollmentRequest
            {
                StudentId = student!.Id,
                CourseSectionId = courseSection.Id
            });
            Assert.Equal(HttpStatusCode.Created, enrollmentResponse.StatusCode);
            return (await enrollmentResponse.Content.ReadFromJsonAsync<EnrollmentResponse>())!;
        }

        private async Task<EvaluationResponse> CreateEvaluationAsync(Guid courseSectionId, decimal maxScore = 100, decimal weight = 30)
        {
            var response = await _client.PostAsJsonAsync("/api/evaluations", new EvaluationRequest
            {
                CourseSectionId = courseSectionId,
                Name = $"Eval-{Guid.NewGuid():N}".Substring(0, 8),
                MaxScore = maxScore,
                Weight = weight,
                EvaluationDate = new DateOnly(2025, 10, 1)
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<EvaluationResponse>())!;
        }

        private async Task<GradeResponse> CreateGradeAsync(Guid evaluationId, Guid enrollmentId, decimal score)
        {
            var response = await _client.PostAsJsonAsync("/api/grades", new GradeRequest
            {
                EvaluationId = evaluationId,
                EnrollmentId = enrollmentId,
                Score = score
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<GradeResponse>())!;
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
    }
}
