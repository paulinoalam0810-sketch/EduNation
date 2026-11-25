using EduNation.Application.Grades;
using EduNation.Domain.Entities;

namespace EduNation.Application.Abstractions.Persistence
{
    public interface IGradeRepository
    {
        Task<Grade> AddAsync(Grade grade, CancellationToken cancellationToken);
        Task<Grade?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<Grade>> GetByEvaluationIdAsync(Guid evaluationId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Grade>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Grade>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Grade>> GetByCourseSectionIdAsync(Guid courseSectionId, CancellationToken cancellationToken);
        Task<PagedResult<Grade>> SearchAsync(GradeSearchCriteria criteria, CancellationToken cancellationToken);
        Task UpdateAsync(Grade grade, CancellationToken cancellationToken);
    }
}
