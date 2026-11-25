using EduNation.Domain.Entities;

namespace EduNation.Application.Abstractions.Persistence
{
    public interface ICourseSectionRepository
    {
        Task<CourseSection> AddAsync(CourseSection courseSection, CancellationToken cancellationToken);
        Task<CourseSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<CourseSection>> GetAllAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<CourseSection>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken);
        Task<IReadOnlyList<CourseSection>> GetByAcademicPeriodIdAsync(Guid academicPeriodId, CancellationToken cancellationToken);
        Task<IReadOnlyList<CourseSection>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken);
        Task<bool> ExistsByCodeAsync(string code, Guid courseId, Guid academicPeriodId, Guid? excludedId, CancellationToken cancellationToken);
        Task UpdateAsync(CourseSection courseSection, CancellationToken cancellationToken);
    }
}
