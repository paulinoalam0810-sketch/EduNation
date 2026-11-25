using Microsoft.AspNetCore.Mvc;
using EduNation.Application.Grades;

namespace EduNation.API.Controllers
{
    [Route("api/evaluations")]
    [ApiController]
    public class EvaluationsController : ControllerBase
    {
        private readonly CreateEvaluationUseCase _createUseCase;
        private readonly GetEvaluationByIdUseCase _getByIdUseCase;
        private readonly GetEvaluationsByCourseSectionUseCase _getBySectionUseCase;
        private readonly UpdateEvaluationUseCase _updateUseCase;
        private readonly DeactivateEvaluationUseCase _deactivateUseCase;

        public EvaluationsController(
            CreateEvaluationUseCase createUseCase,
            GetEvaluationByIdUseCase getByIdUseCase,
            GetEvaluationsByCourseSectionUseCase getBySectionUseCase,
            UpdateEvaluationUseCase updateUseCase,
            DeactivateEvaluationUseCase deactivateUseCase)
        {
            _createUseCase = createUseCase;
            _getByIdUseCase = getByIdUseCase;
            _getBySectionUseCase = getBySectionUseCase;
            _updateUseCase = updateUseCase;
            _deactivateUseCase = deactivateUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvaluation(EvaluationRequest request, CancellationToken cancellationToken)
        {
            var evaluation = await _createUseCase.ExecuteAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetEvaluationById), new { id = evaluation.Id }, evaluation);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetEvaluationById(Guid id, CancellationToken cancellationToken)
        {
            var evaluation = await _getByIdUseCase.ExecuteAsync(id, cancellationToken);
            return Ok(evaluation);
        }

        [HttpGet("course-section/{courseSectionId:guid}")]
        public async Task<IActionResult> GetEvaluationsByCourseSection(Guid courseSectionId, CancellationToken cancellationToken)
        {
            var evaluations = await _getBySectionUseCase.ExecuteAsync(courseSectionId, cancellationToken);
            return Ok(evaluations);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEvaluation(Guid id, EvaluationRequest request, CancellationToken cancellationToken)
        {
            var evaluation = await _updateUseCase.ExecuteAsync(id, request, cancellationToken);
            return Ok(evaluation);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeactivateEvaluation(Guid id, CancellationToken cancellationToken)
        {
            await _deactivateUseCase.ExecuteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
