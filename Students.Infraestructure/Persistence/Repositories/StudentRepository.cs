using Microsoft.EntityFrameworkCore;
using Students.Application.Abstractions.Persistence;
using Students.Domain.Entities;
using Students.Infrastructure.Persistence;

namespace Students.Infrastructure.Persistence.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public StudentRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Student> AddAsync(Student student, CancellationToken cancellationToken)
        {
            await _applicationDbContext.Students.AddAsync(student, cancellationToken);

            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return student;
        }

        public async Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var student = await _applicationDbContext.Students.FirstOrDefaultAsync(s => s.Id == id && s.IsActive, cancellationToken); 

            return student;
        }

        public async Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken cancellationToken)
        {
            var students = await _applicationDbContext.Students.Where(s => s.IsActive).ToListAsync(cancellationToken);

            return students;
        }

        public async Task UpdateAsync(Student student, CancellationToken cancellationToken)
        {
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
