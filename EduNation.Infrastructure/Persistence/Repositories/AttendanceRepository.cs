using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Application.SearchAttendances;
using EduNation.Domain.Entities;
using EduNation.Infrastructure.Persistence;

namespace EduNation.Infrastructure.Persistence.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public AttendanceRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Attendance> AddAsync(Attendance attendance, CancellationToken cancellationToken)
        {
            await _applicationDbContext.Attendances.AddAsync(attendance, cancellationToken);
            try
            {
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
            {
                throw new ConflictException("An attendance record for this enrollment and date already exists.");
            }

            return attendance;
        }

        public Task<Attendance?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _applicationDbContext.Attendances
                .FirstOrDefaultAsync(attendance => attendance.Id == id, cancellationToken);
        }

        public async Task<PagedResult<Attendance>> SearchAsync(AttendanceSearchCriteria criteria, CancellationToken cancellationToken)
        {
            var query = _applicationDbContext.Attendances
                .AsNoTracking()
                .Join(_applicationDbContext.Enrollments,
                    attendance => attendance.EnrollmentId,
                    enrollment => enrollment.Id,
                    (attendance, enrollment) => new { attendance, enrollment })
                .AsQueryable();

            if (criteria.EnrollmentId.HasValue)
                query = query.Where(x => x.attendance.EnrollmentId == criteria.EnrollmentId.Value);

            if (criteria.StudentId.HasValue)
                query = query.Where(x => x.enrollment.StudentId == criteria.StudentId.Value);

            if (criteria.CourseSectionId.HasValue)
                query = query.Where(x => x.enrollment.CourseSectionId == criteria.CourseSectionId.Value);

            if (criteria.Date.HasValue)
                query = query.Where(x => x.attendance.Date == criteria.Date.Value);

            if (criteria.Status.HasValue)
                query = query.Where(x => x.attendance.Status == criteria.Status.Value);

            var totalCount = await query.CountAsync(cancellationToken);
            var skip = (int)Math.Min(((long)criteria.PageNumber - 1) * criteria.PageSize, int.MaxValue);
            var items = await query
                .OrderBy(x => x.attendance.Date)
                .ThenBy(x => x.attendance.EnrollmentId)
                .ThenBy(x => x.attendance.Id)
                .Skip(skip)
                .Take(criteria.PageSize)
                .Select(x => x.attendance)
                .ToListAsync(cancellationToken);

            return new PagedResult<Attendance>
            {
                Items = items,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<IReadOnlyList<Attendance>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Attendances
                .AsNoTracking()
                .Where(attendance => attendance.EnrollmentId == enrollmentId)
                .OrderBy(attendance => attendance.Date)
                .ThenBy(attendance => attendance.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Attendance>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Attendances
                .AsNoTracking()
                .Join(_applicationDbContext.Enrollments,
                    attendance => attendance.EnrollmentId,
                    enrollment => enrollment.Id,
                    (attendance, enrollment) => new { attendance, enrollment })
                .Where(x => x.enrollment.StudentId == studentId)
                .Select(x => x.attendance)
                .OrderBy(attendance => attendance.Date)
                .ThenBy(attendance => attendance.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Attendance>> GetByCourseSectionAndDateAsync(Guid courseSectionId, DateOnly date, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Attendances
                .AsNoTracking()
                .Join(_applicationDbContext.Enrollments,
                    attendance => attendance.EnrollmentId,
                    enrollment => enrollment.Id,
                    (attendance, enrollment) => new { attendance, enrollment })
                .Where(x => x.enrollment.CourseSectionId == courseSectionId && x.attendance.Date == date)
                .Select(x => x.attendance)
                .OrderBy(attendance => attendance.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(Attendance attendance, CancellationToken cancellationToken)
        {
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
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
