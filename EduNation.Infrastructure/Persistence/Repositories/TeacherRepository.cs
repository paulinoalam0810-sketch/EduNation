using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Application.SearchTeachers;
using EduNation.Domain.Entities;
using EduNation.Infrastructure.Persistence;

namespace EduNation.Infrastructure.Persistence.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public TeacherRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Teacher> AddAsync(Teacher teacher, CancellationToken cancellationToken)
        {
            await _applicationDbContext.Teachers.AddAsync(teacher, cancellationToken);
            try
            {
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
            {
                throw new ConflictException("A teacher with this email already exists.");
            }

            return teacher;
        }

        public Task<Teacher?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _applicationDbContext.Teachers
                .FirstOrDefaultAsync(teacher => teacher.Id == id && teacher.IsActive, cancellationToken);
        }

        public async Task<IReadOnlyList<Teacher>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Teachers
                .AsNoTracking()
                .Where(teacher => teacher.IsActive)
                .OrderBy(teacher => teacher.LastName)
                .ThenBy(teacher => teacher.FirstName)
                .ThenBy(teacher => teacher.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<Teacher>> SearchAsync(TeacherSearchCriteria criteria, CancellationToken cancellationToken)
        {
            var query = _applicationDbContext.Teachers
                .AsNoTracking()
                .Where(teacher => teacher.IsActive);

            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search;
                query = query.Where(teacher =>
                    teacher.FirstName.Contains(search) ||
                    teacher.LastName.Contains(search) ||
                    teacher.Email.Contains(search));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var skip = (int)Math.Min(((long)criteria.PageNumber - 1) * criteria.PageSize, int.MaxValue);
            var teachers = await query
                .OrderBy(teacher => teacher.LastName)
                .ThenBy(teacher => teacher.FirstName)
                .ThenBy(teacher => teacher.Id)
                .Skip(skip)
                .Take(criteria.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Teacher>
            {
                Items = teachers,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task UpdateAsync(Teacher teacher, CancellationToken cancellationToken)
        {
            try
            {
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
            {
                throw new ConflictException("A teacher with this email already exists.");
            }
        }

        private static bool IsUniqueConstraintViolation(Exception exception)
        {
            var providerException = exception.GetBaseException();
            if (providerException is SqlException sqlException)
                return sqlException.Number is 2601 or 2627;

            if (providerException.GetType().FullName != "Microsoft.Data.Sqlite.SqliteException")
                return false;

            var exceptionType = providerException.GetType();
            var errorCode = exceptionType.GetProperty("SqliteErrorCode")?.GetValue(providerException);
            var extendedErrorCode = exceptionType.GetProperty("SqliteExtendedErrorCode")?.GetValue(providerException);
            return errorCode is int code && code == 19
                && extendedErrorCode is int extended && extended is 1555 or 2067;
        }
    }
}
