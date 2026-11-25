using Microsoft.EntityFrameworkCore;
using Students.Application.Abstractions.Persistence;
using Students.Application.SearchStudents;
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

        public async Task<PagedResult<Student>> SearchAsync(
            StudentSearchCriteria criteria,
            CancellationToken cancellationToken)
        {
            var query = _applicationDbContext.Students
                .AsNoTracking()
                .Where(student => student.IsActive);

            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search;
                query = query.Where(student =>
                    student.FirstName.Contains(search) ||
                    student.LastName.Contains(search) ||
                    student.Email.Contains(search));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var skip = (int)Math.Min(((long)criteria.PageNumber - 1) * criteria.PageSize, int.MaxValue);
            var students = await query
                .OrderBy(student => student.LastName)
                .ThenBy(student => student.FirstName)
                .ThenBy(student => student.Id)
                .Skip(skip)
                .Take(criteria.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Student>
            {
                Items = students,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task UpdateAsync(Student student, CancellationToken cancellationToken)
        {
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
