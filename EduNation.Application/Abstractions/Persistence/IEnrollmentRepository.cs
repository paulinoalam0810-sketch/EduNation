using EduNation.Domain.Entities;

namespace EduNation.Application.Abstractions.Persistence
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment> AddAsync(Enrollment enrollment, CancellationToken cancellationToken);
        Task<Enrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<Enrollment>> GetAllAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Enrollment>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Enrollment>> GetByCourseSectionIdAsync(Guid courseSectionId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Enrollment>> GetByCourseSectionAsync(Guid courseSectionId, CancellationToken cancellationToken);
        Task<bool> ExistsActiveAsync(Guid studentId, Guid courseSectionId, CancellationToken cancellationToken);
        Task UpdateAsync(Enrollment enrollment, CancellationToken cancellationToken);
    }
}
