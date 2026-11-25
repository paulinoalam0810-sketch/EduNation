using EduNation.Application.SearchTeachers;
using EduNation.Domain.Entities;

namespace EduNation.Application.Abstractions.Persistence
{
    public interface ITeacherRepository
    {
        Task<Teacher> AddAsync(Teacher teacher, CancellationToken cancellationToken);
        Task<Teacher?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<Teacher>> GetAllAsync(CancellationToken cancellationToken);
        Task<PagedResult<Teacher>> SearchAsync(TeacherSearchCriteria criteria, CancellationToken cancellationToken);
        Task UpdateAsync(Teacher teacher, CancellationToken cancellationToken);
    }
}
