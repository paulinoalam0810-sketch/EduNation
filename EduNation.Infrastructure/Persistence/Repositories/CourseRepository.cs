using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;
using EduNation.Infrastructure.Persistence;

namespace EduNation.Infrastructure.Persistence.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CourseRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Course> AddAsync(Course course, CancellationToken cancellationToken)
        {
            await _applicationDbContext.Courses.AddAsync(course, cancellationToken);
            try
            {
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
            {
                throw new ConflictException("A course with this code already exists.");
            }

            return course;
        }

        public Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _applicationDbContext.Courses
                .FirstOrDefaultAsync(course => course.Id == id && course.IsActive, cancellationToken);
        }

        public async Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Courses
                .AsNoTracking()
                .Where(course => course.IsActive)
                .OrderBy(course => course.Code)
                .ThenBy(course => course.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<bool> ExistsByCodeAsync(string code, Guid? excludedId, CancellationToken cancellationToken)
        {
            return _applicationDbContext.Courses.AnyAsync(
                course => course.Code == code && (!excludedId.HasValue || course.Id != excludedId.Value),
                cancellationToken);
        }

        public async Task UpdateAsync(Course course, CancellationToken cancellationToken)
        {
            try
            {
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
            {
                throw new ConflictException("A course with this code already exists.");
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
