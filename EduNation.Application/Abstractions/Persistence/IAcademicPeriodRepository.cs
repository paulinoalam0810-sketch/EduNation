using EduNation.Domain.Entities;

namespace EduNation.Application.Abstractions.Persistence
{
    public interface IAcademicPeriodRepository
    {
        Task<AcademicPeriod> AddAsync(AcademicPeriod academicPeriod, CancellationToken cancellationToken);
        Task<AcademicPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<AcademicPeriod>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> ExistsByCodeAsync(string code, Guid? excludedId, CancellationToken cancellationToken);
        Task UpdateAsync(AcademicPeriod academicPeriod, CancellationToken cancellationToken);
    }
}
