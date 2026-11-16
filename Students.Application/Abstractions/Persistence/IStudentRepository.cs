using Students.Domain.Entities;

namespace Students.Application.Abstractions.Persistence
{
    public interface IStudentRepository
    {
        Task<Student> AddAsync(Student student, CancellationToken cancellationToken);
        Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken cancellationToken);
        Task UpdateAsync(Student student, CancellationToken cancellationToken);
    }
}
