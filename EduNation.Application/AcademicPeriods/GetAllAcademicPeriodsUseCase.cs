using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.AcademicPeriods
{
    public class GetAllAcademicPeriodsUseCase
    {
        private readonly IAcademicPeriodRepository _academicPeriodRepository;

        public GetAllAcademicPeriodsUseCase(IAcademicPeriodRepository academicPeriodRepository)
        {
            _academicPeriodRepository = academicPeriodRepository;
        }

        public async Task<IReadOnlyList<AcademicPeriodResponse>> ExecuteAsync(CancellationToken cancellationToken)
        {
            var academicPeriods = await _academicPeriodRepository.GetAllAsync(cancellationToken);
            return academicPeriods.Select(AcademicPeriodMapper.ToResponse).ToList();
        }
    }
}
