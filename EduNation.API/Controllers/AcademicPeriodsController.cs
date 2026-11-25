using Microsoft.AspNetCore.Mvc;
using EduNation.Application.AcademicPeriods;

namespace EduNation.API.Controllers
{
    [Route("api/academicperiods")]
    [Route("api/academic-periods")]
    [ApiController]
    public class AcademicPeriodsController : ControllerBase
    {
        private readonly CreateAcademicPeriodUseCase _createAcademicPeriodUseCase;
        private readonly GetAcademicPeriodByIdUseCase _getAcademicPeriodByIdUseCase;
        private readonly GetAllAcademicPeriodsUseCase _getAllAcademicPeriodsUseCase;
        private readonly UpdateAcademicPeriodUseCase _updateAcademicPeriodUseCase;
        private readonly DeactivateAcademicPeriodUseCase _deactivateAcademicPeriodUseCase;

        public AcademicPeriodsController(
            CreateAcademicPeriodUseCase createAcademicPeriodUseCase,
            GetAcademicPeriodByIdUseCase getAcademicPeriodByIdUseCase,
            GetAllAcademicPeriodsUseCase getAllAcademicPeriodsUseCase,
            UpdateAcademicPeriodUseCase updateAcademicPeriodUseCase,
            DeactivateAcademicPeriodUseCase deactivateAcademicPeriodUseCase)
        {
            _createAcademicPeriodUseCase = createAcademicPeriodUseCase;
            _getAcademicPeriodByIdUseCase = getAcademicPeriodByIdUseCase;
            _getAllAcademicPeriodsUseCase = getAllAcademicPeriodsUseCase;
            _updateAcademicPeriodUseCase = updateAcademicPeriodUseCase;
            _deactivateAcademicPeriodUseCase = deactivateAcademicPeriodUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAcademicPeriod(AcademicPeriodRequest request, CancellationToken cancellationToken)
        {
            var academicPeriod = await _createAcademicPeriodUseCase.ExecuteAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetAcademicPeriodById), new { id = academicPeriod.Id }, academicPeriod);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetAcademicPeriodById(Guid id, CancellationToken cancellationToken)
        {
            var academicPeriod = await _getAcademicPeriodByIdUseCase.ExecuteAsync(id, cancellationToken);
            return Ok(academicPeriod);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAcademicPeriods(CancellationToken cancellationToken)
        {
            var academicPeriods = await _getAllAcademicPeriodsUseCase.ExecuteAsync(cancellationToken);
            return Ok(academicPeriods);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAcademicPeriod(Guid id, AcademicPeriodRequest request, CancellationToken cancellationToken)
        {
            var academicPeriod = await _updateAcademicPeriodUseCase.ExecuteAsync(id, request, cancellationToken);
            return Ok(academicPeriod);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeactivateAcademicPeriod(Guid id, CancellationToken cancellationToken)
        {
            await _deactivateAcademicPeriodUseCase.ExecuteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
