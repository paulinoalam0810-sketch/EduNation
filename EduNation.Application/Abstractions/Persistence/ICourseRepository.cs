using EduNation.Domain.Entities;

namespace EduNation.Application.Abstractions.Persistence
{
    public interface ICourseRepository
    {
        Task<Course> AddAsync(Course course, CancellationToken cancellationToken);
        Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> ExistsByCodeAsync(string code, Guid? excludedId, CancellationToken cancellationToken);
        Task UpdateAsync(Course course, CancellationToken cancellationToken);
    }
}
