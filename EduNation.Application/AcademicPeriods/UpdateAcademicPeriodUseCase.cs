using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.AcademicPeriods
{
    public class UpdateAcademicPeriodUseCase
    {
        private readonly IAcademicPeriodRepository _academicPeriodRepository;

        public UpdateAcademicPeriodUseCase(IAcademicPeriodRepository academicPeriodRepository)
        {
            _academicPeriodRepository = academicPeriodRepository;
        }

        public async Task<AcademicPeriodResponse> ExecuteAsync(Guid id, AcademicPeriodRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var academicPeriod = await _academicPeriodRepository.GetByIdAsync(id, cancellationToken);
            if (academicPeriod is null)
                throw new NotFoundException("Academic period not found.");

            academicPeriod.Update(request.Name, request.Code, request.StartDate, request.EndDate);
            if (await _academicPeriodRepository.ExistsByCodeAsync(academicPeriod.Code, id, cancellationToken))
                throw new ConflictException("An academic period with this code already exists.");

            await _academicPeriodRepository.UpdateAsync(academicPeriod, cancellationToken);
            return AcademicPeriodMapper.ToResponse(academicPeriod);
        }
    }
}
