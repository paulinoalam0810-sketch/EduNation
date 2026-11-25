using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.AcademicPeriods
{
    public class GetAcademicPeriodByIdUseCase
    {
        private readonly IAcademicPeriodRepository _academicPeriodRepository;

        public GetAcademicPeriodByIdUseCase(IAcademicPeriodRepository academicPeriodRepository)
        {
            _academicPeriodRepository = academicPeriodRepository;
        }

        public async Task<AcademicPeriodResponse> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var academicPeriod = await _academicPeriodRepository.GetByIdAsync(id, cancellationToken);
            if (academicPeriod is null)
                throw new NotFoundException("Academic period not found.");

            return AcademicPeriodMapper.ToResponse(academicPeriod);
        }
    }
}
