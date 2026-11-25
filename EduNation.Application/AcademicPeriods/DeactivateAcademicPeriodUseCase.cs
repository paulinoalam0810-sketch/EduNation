using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.AcademicPeriods
{
    public class DeactivateAcademicPeriodUseCase
    {
        private readonly IAcademicPeriodRepository _academicPeriodRepository;

        public DeactivateAcademicPeriodUseCase(IAcademicPeriodRepository academicPeriodRepository)
        {
            _academicPeriodRepository = academicPeriodRepository;
        }

        public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var academicPeriod = await _academicPeriodRepository.GetByIdAsync(id, cancellationToken);
            if (academicPeriod is null)
                throw new NotFoundException("Academic period not found.");

            academicPeriod.Deactivate();
            await _academicPeriodRepository.UpdateAsync(academicPeriod, cancellationToken);
        }
    }
}
