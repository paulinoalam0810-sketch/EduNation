using EduNation.Domain.Entities;

namespace EduNation.Application.Abstractions.Persistence
{
    public interface IEvaluationRepository
    {
        Task<Evaluation> AddAsync(Evaluation evaluation, CancellationToken cancellationToken);
        Task<Evaluation?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<Evaluation>> GetByCourseSectionIdAsync(Guid courseSectionId, CancellationToken cancellationToken);
        Task UpdateAsync(Evaluation evaluation, CancellationToken cancellationToken);
    }
}
