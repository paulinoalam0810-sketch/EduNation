using Microsoft.AspNetCore.Mvc;
using EduNation.API.Models;
using EduNation.Application.Grades;

namespace EduNation.API.Controllers
{
    [Route("api/grades")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        private readonly CreateGradeUseCase _createUseCase;
        private readonly GetGradeByIdUseCase _getByIdUseCase;
        private readonly GetGradesByStudentUseCase _getByStudentUseCase;
        private readonly GetGradesByEvaluationUseCase _getByEvaluationUseCase;
        private readonly GetGradesByCourseSectionUseCase _getBySectionUseCase;
        private readonly UpdateGradeUseCase _updateUseCase;
        private readonly SearchGradesUseCase _searchUseCase;
        private readonly GetStudentSectionResultUseCase _resultUseCase;

        public GradesController(
            CreateGradeUseCase createUseCase,
            GetGradeByIdUseCase getByIdUseCase,
            GetGradesByStudentUseCase getByStudentUseCase,
            GetGradesByEvaluationUseCase getByEvaluationUseCase,
            GetGradesByCourseSectionUseCase getBySectionUseCase,
            UpdateGradeUseCase updateUseCase,
            SearchGradesUseCase searchUseCase,
            GetStudentSectionResultUseCase resultUseCase)
        {
            _createUseCase = createUseCase;
            _getByIdUseCase = getByIdUseCase;
            _getByStudentUseCase = getByStudentUseCase;
            _getByEvaluationUseCase = getByEvaluationUseCase;
            _getBySectionUseCase = getBySectionUseCase;
            _updateUseCase = updateUseCase;
            _searchUseCase = searchUseCase;
            _resultUseCase = resultUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGrade(GradeRequest request, CancellationToken cancellationToken)
        {
            var grade = await _createUseCase.ExecuteAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetGradeById), new { id = grade.Id }, grade);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetGradeById(Guid id, CancellationToken cancellationToken)
        {
            var grade = await _getByIdUseCase.ExecuteAsync(id, cancellationToken);
            return Ok(grade);
        }

        [HttpGet("student/{studentId:guid}")]
        public async Task<IActionResult> GetGradesByStudent(Guid studentId, CancellationToken cancellationToken)
        {
            var grades = await _getByStudentUseCase.ExecuteAsync(studentId, cancellationToken);
            return Ok(grades);
        }

        [HttpGet("evaluation/{evaluationId:guid}")]
        public async Task<IActionResult> GetGradesByEvaluation(Guid evaluationId, CancellationToken cancellationToken)
        {
            var grades = await _getByEvaluationUseCase.ExecuteAsync(evaluationId, cancellationToken);
            return Ok(grades);
        }

        [HttpGet("course-section/{courseSectionId:guid}")]
        public async Task<IActionResult> GetGradesByCourseSection(Guid courseSectionId, CancellationToken cancellationToken)
        {
            var grades = await _getBySectionUseCase.ExecuteAsync(courseSectionId, cancellationToken);
            return Ok(grades);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchGrades([FromQuery] GradeSearchRequest request, CancellationToken cancellationToken)
        {
            var criteria = new GradeSearchCriteria
            {
                EvaluationId = request.EvaluationId,
                EnrollmentId = request.EnrollmentId,
                StudentId = request.StudentId,
                CourseSectionId = request.CourseSectionId,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var result = await _searchUseCase.ExecuteAsync(criteria, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateGrade(Guid id, UpdateGradeRequest request, CancellationToken cancellationToken)
        {
            var grade = await _updateUseCase.ExecuteAsync(id, request, cancellationToken);
            return Ok(grade);
        }

        [HttpGet("results/student/{studentId:guid}/course-section/{courseSectionId:guid}")]
        public async Task<IActionResult> GetStudentSectionResult(Guid studentId, Guid courseSectionId, CancellationToken cancellationToken)
        {
            var result = await _resultUseCase.ExecuteAsync(studentId, courseSectionId, cancellationToken);
            return Ok(result);
        }
    }
}
