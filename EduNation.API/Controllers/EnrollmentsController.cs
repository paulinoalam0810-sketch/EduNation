using Microsoft.AspNetCore.Mvc;
using EduNation.Application.Enrollments;

namespace EduNation.API.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly CreateEnrollmentUseCase _createEnrollmentUseCase;
        private readonly GetEnrollmentByIdUseCase _getEnrollmentByIdUseCase;
        private readonly GetAllEnrollmentsUseCase _getAllEnrollmentsUseCase;
        private readonly GetEnrollmentsByStudentUseCase _getEnrollmentsByStudentUseCase;
        private readonly GetEnrollmentsByStudentIdUseCase _getEnrollmentsByStudentIdUseCase;
        private readonly GetEnrollmentsByCourseSectionUseCase _getEnrollmentsByCourseSectionUseCase;
        private readonly GetEnrollmentsByCourseSectionIdUseCase _getEnrollmentsByCourseSectionIdUseCase;
        private readonly CancelEnrollmentUseCase _cancelEnrollmentUseCase;

        public EnrollmentsController(
            CreateEnrollmentUseCase createEnrollmentUseCase,
            GetEnrollmentByIdUseCase getEnrollmentByIdUseCase,
            GetAllEnrollmentsUseCase getAllEnrollmentsUseCase,
            GetEnrollmentsByStudentUseCase getEnrollmentsByStudentUseCase,
            GetEnrollmentsByStudentIdUseCase getEnrollmentsByStudentIdUseCase,
            GetEnrollmentsByCourseSectionUseCase getEnrollmentsByCourseSectionUseCase,
            GetEnrollmentsByCourseSectionIdUseCase getEnrollmentsByCourseSectionIdUseCase,
            CancelEnrollmentUseCase cancelEnrollmentUseCase)
        {
            _createEnrollmentUseCase = createEnrollmentUseCase;
            _getEnrollmentByIdUseCase = getEnrollmentByIdUseCase;
            _getAllEnrollmentsUseCase = getAllEnrollmentsUseCase;
            _getEnrollmentsByStudentUseCase = getEnrollmentsByStudentUseCase;
            _getEnrollmentsByStudentIdUseCase = getEnrollmentsByStudentIdUseCase;
            _getEnrollmentsByCourseSectionUseCase = getEnrollmentsByCourseSectionUseCase;
            _getEnrollmentsByCourseSectionIdUseCase = getEnrollmentsByCourseSectionIdUseCase;
            _cancelEnrollmentUseCase = cancelEnrollmentUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnrollment(EnrollmentRequest request, CancellationToken cancellationToken)
        {
            var enrollment = await _createEnrollmentUseCase.ExecuteAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetEnrollmentById), new { id = enrollment.Id }, enrollment);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetEnrollmentById(Guid id, CancellationToken cancellationToken)
        {
            var enrollment = await _getEnrollmentByIdUseCase.ExecuteAsync(id, cancellationToken);
            return Ok(enrollment);
        }

        [HttpGet]
        public async Task<IActionResult> GetEnrollments(
            [FromQuery] Guid? studentId,
            [FromQuery] Guid? student,
            [FromQuery] Guid? courseSectionId,
            [FromQuery] Guid? courseSection,
            CancellationToken cancellationToken)
        {
            if (studentId.HasValue || student.HasValue)
            {
                var id = studentId ?? student!.Value;
                var enrollments = await _getEnrollmentsByStudentUseCase.ExecuteAsync(id, cancellationToken);
                return Ok(enrollments);
            }

            if (courseSectionId.HasValue || courseSection.HasValue)
            {
                var id = courseSectionId ?? courseSection!.Value;
                var enrollments = await _getEnrollmentsByCourseSectionUseCase.ExecuteAsync(id, cancellationToken);
                return Ok(enrollments);
            }

            var all = await _getAllEnrollmentsUseCase.ExecuteAsync(cancellationToken);
            return Ok(all);
        }

        [HttpGet("student/{studentId:guid}")]
        [HttpGet("students/{studentId:guid}")]
        [HttpGet("by-student/{studentId:guid}")]
        public async Task<IActionResult> GetEnrollmentsByStudent(Guid studentId, CancellationToken cancellationToken)
        {
            var enrollments = await _getEnrollmentsByStudentUseCase.ExecuteAsync(studentId, cancellationToken);
            return Ok(enrollments);
        }

        [HttpGet("course-section/{courseSectionId:guid}")]
        [HttpGet("course-sections/{courseSectionId:guid}")]
        [HttpGet("by-course-section/{courseSectionId:guid}")]
        public async Task<IActionResult> GetEnrollmentsByCourseSection(Guid courseSectionId, CancellationToken cancellationToken)
        {
            var enrollments = await _getEnrollmentsByCourseSectionUseCase.ExecuteAsync(courseSectionId, cancellationToken);
            return Ok(enrollments);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> CancelEnrollment(Guid id, CancellationToken cancellationToken)
        {
            await _cancelEnrollmentUseCase.ExecuteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
