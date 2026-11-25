using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;
using EduNation.Infrastructure.Persistence;

namespace EduNation.Infrastructure.Persistence.Repositories
{
    public class CourseSectionRepository : ICourseSectionRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CourseSectionRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<CourseSection> AddAsync(CourseSection courseSection, CancellationToken cancellationToken)
        {
            await _applicationDbContext.CourseSections.AddAsync(courseSection, cancellationToken);
            try
            {
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
            {
                throw new ConflictException("A course section with this code already exists for the course and period.");
            }

            return courseSection;
        }

        public Task<CourseSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _applicationDbContext.CourseSections
                .FirstOrDefaultAsync(courseSection => courseSection.Id == id && courseSection.IsActive, cancellationToken);
        }

        public async Task<IReadOnlyList<CourseSection>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _applicationDbContext.CourseSections
                .AsNoTracking()
                .Where(courseSection => courseSection.IsActive)
                .OrderBy(courseSection => courseSection.Code)
                .ThenBy(courseSection => courseSection.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<CourseSection>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.CourseSections
                .AsNoTracking()
                .Where(courseSection => courseSection.CourseId == courseId && courseSection.IsActive)
                .OrderBy(courseSection => courseSection.Code)
                .ThenBy(courseSection => courseSection.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<CourseSection>> GetByAcademicPeriodIdAsync(Guid academicPeriodId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.CourseSections
                .AsNoTracking()
                .Where(courseSection => courseSection.AcademicPeriodId == academicPeriodId && courseSection.IsActive)
                .OrderBy(courseSection => courseSection.Code)
                .ThenBy(courseSection => courseSection.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<CourseSection>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.CourseSections
                .AsNoTracking()
                .Where(courseSection => courseSection.TeacherId == teacherId && courseSection.IsActive)
                .OrderBy(courseSection => courseSection.Code)
                .ThenBy(courseSection => courseSection.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<bool> ExistsByCodeAsync(string code, Guid courseId, Guid academicPeriodId, Guid? excludedId, CancellationToken cancellationToken)
        {
            return _applicationDbContext.CourseSections.AnyAsync(
                courseSection => courseSection.Code == code
                    && courseSection.CourseId == courseId
                    && courseSection.AcademicPeriodId == academicPeriodId
                    && (!excludedId.HasValue || courseSection.Id != excludedId.Value),
                cancellationToken);
        }

        public async Task UpdateAsync(CourseSection courseSection, CancellationToken cancellationToken)
        {
            try
            {
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
            {
                throw new ConflictException("A course section with this code already exists for the course and period.");
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
