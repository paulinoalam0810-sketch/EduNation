using Microsoft.AspNetCore.Mvc;
using EduNation.Application.Courses;

namespace EduNation.API.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly CreateCourseUseCase _createCourseUseCase;
        private readonly GetCourseByIdUseCase _getCourseByIdUseCase;
        private readonly GetAllCoursesUseCase _getAllCoursesUseCase;
        private readonly UpdateCourseUseCase _updateCourseUseCase;
        private readonly DeactivateCourseUseCase _deactivateCourseUseCase;

        public CoursesController(
            CreateCourseUseCase createCourseUseCase,
            GetCourseByIdUseCase getCourseByIdUseCase,
            GetAllCoursesUseCase getAllCoursesUseCase,
            UpdateCourseUseCase updateCourseUseCase,
            DeactivateCourseUseCase deactivateCourseUseCase)
        {
            _createCourseUseCase = createCourseUseCase;
            _getCourseByIdUseCase = getCourseByIdUseCase;
            _getAllCoursesUseCase = getAllCoursesUseCase;
            _updateCourseUseCase = updateCourseUseCase;
            _deactivateCourseUseCase = deactivateCourseUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse(CourseRequest request, CancellationToken cancellationToken)
        {
            var course = await _createCourseUseCase.ExecuteAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCourseById(Guid id, CancellationToken cancellationToken)
        {
            var course = await _getCourseByIdUseCase.ExecuteAsync(id, cancellationToken);
            return Ok(course);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourses(CancellationToken cancellationToken)
        {
            var courses = await _getAllCoursesUseCase.ExecuteAsync(cancellationToken);
            return Ok(courses);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCourse(Guid id, CourseRequest request, CancellationToken cancellationToken)
        {
            var course = await _updateCourseUseCase.ExecuteAsync(id, request, cancellationToken);
            return Ok(course);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeactivateCourse(Guid id, CancellationToken cancellationToken)
        {
            await _deactivateCourseUseCase.ExecuteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
