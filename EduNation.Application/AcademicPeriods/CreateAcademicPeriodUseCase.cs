using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.AcademicPeriods
{
    public class CreateAcademicPeriodUseCase
    {
        private readonly IAcademicPeriodRepository _academicPeriodRepository;

        public CreateAcademicPeriodUseCase(IAcademicPeriodRepository academicPeriodRepository)
        {
            _academicPeriodRepository = academicPeriodRepository;
        }

        public async Task<AcademicPeriodResponse> ExecuteAsync(AcademicPeriodRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var academicPeriod = new AcademicPeriod(request.Name, request.Code, request.StartDate, request.EndDate);

            if (await _academicPeriodRepository.ExistsByCodeAsync(academicPeriod.Code, null, cancellationToken))
                throw new ConflictException("An academic period with this code already exists.");

            await _academicPeriodRepository.AddAsync(academicPeriod, cancellationToken);
            return AcademicPeriodMapper.ToResponse(academicPeriod);
        }
    }
}
