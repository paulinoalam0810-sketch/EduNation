using EduNation.Application.SearchAttendances;
using EduNation.Domain.Entities;

namespace EduNation.Application.Abstractions.Persistence
{
    public interface IAttendanceRepository
    {
        Task<Attendance> AddAsync(Attendance attendance, CancellationToken cancellationToken);
        Task<Attendance?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<PagedResult<Attendance>> SearchAsync(AttendanceSearchCriteria criteria, CancellationToken cancellationToken);
        Task<IReadOnlyList<Attendance>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Attendance>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Attendance>> GetByCourseSectionAndDateAsync(Guid courseSectionId, DateOnly date, CancellationToken cancellationToken);
        Task UpdateAsync(Attendance attendance, CancellationToken cancellationToken);
    }
}
