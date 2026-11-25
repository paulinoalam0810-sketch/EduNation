using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;
using EduNation.Infrastructure.Persistence;

namespace EduNation.Infrastructure.Persistence.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public EnrollmentRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Enrollment> AddAsync(Enrollment enrollment, CancellationToken cancellationToken)
        {
            await using var transaction = await _applicationDbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);
            try
            {
                if (enrollment.CourseSectionId is null)
                    throw new NotFoundException("Course section id cannot be empty.");

                var courseSectionId = enrollment.CourseSectionId.Value;

                var courseSection = await _applicationDbContext.CourseSections
                    .FirstOrDefaultAsync(section => section.Id == courseSectionId && section.IsActive, cancellationToken);

                if (courseSection is null)
                    throw new NotFoundException("Course section not found or inactive.");

                var student = await _applicationDbContext.Students
                    .FirstOrDefaultAsync(item => item.Id == enrollment.StudentId && item.IsActive, cancellationToken);

                if (student is null)
                    throw new NotFoundException("Student not found or inactive.");

                var duplicateExists = await _applicationDbContext.Enrollments.AnyAsync(
                    item => item.StudentId == enrollment.StudentId
                        && item.CourseSectionId == courseSectionId
                        && item.Status == EnrollmentStatus.Active,
                    cancellationToken);

                if (duplicateExists)
                    throw new ConflictException("An enrollment for this student and course section already exists.");

                var activeCount = await _applicationDbContext.Enrollments.CountAsync(
                    item => item.CourseSectionId == courseSectionId && item.Status == EnrollmentStatus.Active,
                    cancellationToken);

                if (activeCount >= courseSection.Capacity)
                    throw new ConflictException("Course section capacity has been reached.");

                await _applicationDbContext.Enrollments.AddAsync(enrollment, cancellationToken);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return enrollment;
            }
            catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ConflictException("An enrollment for this student and course section already exists.");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public Task<Enrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _applicationDbContext.Enrollments
                .FirstOrDefaultAsync(enrollment => enrollment.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Enrollment>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Enrollments
                .AsNoTracking()
                .Where(enrollment => enrollment.Status == EnrollmentStatus.Active)
                .OrderBy(enrollment => enrollment.EnrolledAt)
                .ThenBy(enrollment => enrollment.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Enrollments
                .AsNoTracking()
                .Where(enrollment => enrollment.StudentId == studentId)
                .OrderBy(enrollment => enrollment.EnrolledAt)
                .ThenBy(enrollment => enrollment.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<IReadOnlyList<Enrollment>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken)
        {
            return GetByStudentIdAsync(studentId, cancellationToken);
        }

        public async Task<IReadOnlyList<Enrollment>> GetByCourseSectionIdAsync(Guid courseSectionId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Enrollments
                .AsNoTracking()
                .Where(enrollment => enrollment.CourseSectionId == courseSectionId)
                .OrderBy(enrollment => enrollment.EnrolledAt)
                .ThenBy(enrollment => enrollment.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<IReadOnlyList<Enrollment>> GetByCourseSectionAsync(Guid courseSectionId, CancellationToken cancellationToken)
        {
            return GetByCourseSectionIdAsync(courseSectionId, cancellationToken);
        }

        public Task<bool> ExistsActiveAsync(Guid studentId, Guid courseSectionId, CancellationToken cancellationToken)
        {
            return _applicationDbContext.Enrollments.AnyAsync(
                enrollment => enrollment.StudentId == studentId
                    && enrollment.CourseSectionId == courseSectionId
                    && enrollment.Status == EnrollmentStatus.Active,
                cancellationToken);
        }

        public async Task UpdateAsync(Enrollment enrollment, CancellationToken cancellationToken)
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
