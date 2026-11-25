using Microsoft.AspNetCore.Mvc;
using EduNation.Application.CourseSections;

namespace EduNation.API.Controllers
{
    [Route("api/coursesections")]
    [Route("api/course-sections")]
    [ApiController]
    public class CourseSectionsController : ControllerBase
    {
        private readonly CreateCourseSectionUseCase _createUseCase;
        private readonly GetCourseSectionByIdUseCase _getByIdUseCase;
        private readonly GetAllCourseSectionsUseCase _getAllUseCase;
        private readonly GetCourseSectionsByCourseUseCase _getByCourseUseCase;
        private readonly GetCourseSectionsByCourseIdUseCase _getByCourseIdUseCase;
        private readonly GetCourseSectionsByAcademicPeriodUseCase _getByPeriodUseCase;
        private readonly GetCourseSectionsByAcademicPeriodIdUseCase _getByPeriodIdUseCase;
        private readonly GetCourseSectionsByTeacherUseCase _getByTeacherUseCase;
        private readonly GetCourseSectionsByTeacherIdUseCase _getByTeacherIdUseCase;
        private readonly UpdateCourseSectionUseCase _updateUseCase;
        private readonly DeactivateCourseSectionUseCase _deactivateUseCase;

        public CourseSectionsController(
            CreateCourseSectionUseCase createUseCase,
            GetCourseSectionByIdUseCase getByIdUseCase,
            GetAllCourseSectionsUseCase getAllUseCase,
            GetCourseSectionsByCourseUseCase getByCourseUseCase,
            GetCourseSectionsByCourseIdUseCase getByCourseIdUseCase,
            GetCourseSectionsByAcademicPeriodUseCase getByPeriodUseCase,
            GetCourseSectionsByAcademicPeriodIdUseCase getByPeriodIdUseCase,
            GetCourseSectionsByTeacherUseCase getByTeacherUseCase,
            GetCourseSectionsByTeacherIdUseCase getByTeacherIdUseCase,
            UpdateCourseSectionUseCase updateUseCase,
            DeactivateCourseSectionUseCase deactivateUseCase)
        {
            _createUseCase = createUseCase;
            _getByIdUseCase = getByIdUseCase;
            _getAllUseCase = getAllUseCase;
            _getByCourseUseCase = getByCourseUseCase;
            _getByCourseIdUseCase = getByCourseIdUseCase;
            _getByPeriodUseCase = getByPeriodUseCase;
            _getByPeriodIdUseCase = getByPeriodIdUseCase;
            _getByTeacherUseCase = getByTeacherUseCase;
            _getByTeacherIdUseCase = getByTeacherIdUseCase;
            _updateUseCase = updateUseCase;
            _deactivateUseCase = deactivateUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseSectionRequest request, CancellationToken cancellationToken)
        {
            var result = await _createUseCase.ExecuteAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _getByIdUseCase.ExecuteAsync(id, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] Guid? courseId,
            [FromQuery] Guid? academicPeriodId,
            [FromQuery] Guid? teacherId,
            CancellationToken cancellationToken)
        {
            if (courseId.HasValue)
                return Ok(await _getByCourseUseCase.ExecuteAsync(courseId.Value, cancellationToken));

            if (academicPeriodId.HasValue)
                return Ok(await _getByPeriodUseCase.ExecuteAsync(academicPeriodId.Value, cancellationToken));

            if (teacherId.HasValue)
                return Ok(await _getByTeacherUseCase.ExecuteAsync(teacherId.Value, cancellationToken));

            return Ok(await _getAllUseCase.ExecuteAsync(cancellationToken));
        }

        [HttpGet("course/{courseId:guid}")]
        [HttpGet("courses/{courseId:guid}")]
        [HttpGet("by-course/{courseId:guid}")]
        public async Task<IActionResult> GetByCourse(Guid courseId, CancellationToken cancellationToken)
        {
            return Ok(await _getByCourseUseCase.ExecuteAsync(courseId, cancellationToken));
        }

        [HttpGet("academic-period/{academicPeriodId:guid}")]
        [HttpGet("period/{academicPeriodId:guid}")]
        [HttpGet("by-academic-period/{academicPeriodId:guid}")]
        [HttpGet("by-period/{academicPeriodId:guid}")]
        public async Task<IActionResult> GetByAcademicPeriod(Guid academicPeriodId, CancellationToken cancellationToken)
        {
            return Ok(await _getByPeriodUseCase.ExecuteAsync(academicPeriodId, cancellationToken));
        }

        [HttpGet("teacher/{teacherId:guid}")]
        [HttpGet("teachers/{teacherId:guid}")]
        [HttpGet("by-teacher/{teacherId:guid}")]
        public async Task<IActionResult> GetByTeacher(Guid teacherId, CancellationToken cancellationToken)
        {
            return Ok(await _getByTeacherUseCase.ExecuteAsync(teacherId, cancellationToken));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, CourseSectionRequest request, CancellationToken cancellationToken)
        {
            var result = await _updateUseCase.ExecuteAsync(id, request, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
        {
            await _deactivateUseCase.ExecuteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
