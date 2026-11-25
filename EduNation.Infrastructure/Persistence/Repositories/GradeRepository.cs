using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Application.Grades;
using EduNation.Domain.Entities;
using EduNation.Infrastructure.Persistence;

namespace EduNation.Infrastructure.Persistence.Repositories
{
    public class GradeRepository : IGradeRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public GradeRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Grade> AddAsync(Grade grade, CancellationToken cancellationToken)
        {
            await _applicationDbContext.Grades.AddAsync(grade, cancellationToken);
            try
            {
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
            {
                throw new ConflictException("A grade for this evaluation and enrollment already exists.");
            }

            return grade;
        }

        public Task<Grade?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _applicationDbContext.Grades
                .FirstOrDefaultAsync(grade => grade.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Grade>> GetByEvaluationIdAsync(Guid evaluationId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Grades
                .AsNoTracking()
                .Where(grade => grade.EvaluationId == evaluationId)
                .OrderBy(grade => grade.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Grade>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Grades
                .AsNoTracking()
                .Where(grade => grade.EnrollmentId == enrollmentId)
                .OrderBy(grade => grade.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Grade>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Grades
                .AsNoTracking()
                .Join(_applicationDbContext.Enrollments,
                    grade => grade.EnrollmentId,
                    enrollment => enrollment.Id,
                    (grade, enrollment) => new { grade, enrollment })
                .Where(x => x.enrollment.StudentId == studentId)
                .Select(x => x.grade)
                .OrderBy(grade => grade.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Grade>> GetByCourseSectionIdAsync(Guid courseSectionId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Grades
                .AsNoTracking()
                .Join(_applicationDbContext.Enrollments,
                    grade => grade.EnrollmentId,
                    enrollment => enrollment.Id,
                    (grade, enrollment) => new { grade, enrollment })
                .Where(x => x.enrollment.CourseSectionId == courseSectionId)
                .Select(x => x.grade)
                .OrderBy(grade => grade.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<Grade>> SearchAsync(GradeSearchCriteria criteria, CancellationToken cancellationToken)
        {
            var query = _applicationDbContext.Grades
                .AsNoTracking()
                .Join(_applicationDbContext.Enrollments,
                    grade => grade.EnrollmentId,
                    enrollment => enrollment.Id,
                    (grade, enrollment) => new { grade, enrollment })
                .Join(_applicationDbContext.Evaluations,
                    x => x.grade.EvaluationId,
                    evaluation => evaluation.Id,
                    (x, evaluation) => new { x.grade, x.enrollment, evaluation })
                .AsQueryable();

            if (criteria.EvaluationId.HasValue)
                query = query.Where(x => x.grade.EvaluationId == criteria.EvaluationId.Value);

            if (criteria.EnrollmentId.HasValue)
                query = query.Where(x => x.grade.EnrollmentId == criteria.EnrollmentId.Value);

            if (criteria.StudentId.HasValue)
                query = query.Where(x => x.enrollment.StudentId == criteria.StudentId.Value);

            if (criteria.CourseSectionId.HasValue)
                query = query.Where(x => x.enrollment.CourseSectionId == criteria.CourseSectionId.Value);

            var totalCount = await query.CountAsync(cancellationToken);
            var skip = (int)Math.Min(((long)criteria.PageNumber - 1) * criteria.PageSize, int.MaxValue);
            var items = await query
                .OrderBy(x => x.grade.Id)
                .Skip(skip)
                .Take(criteria.PageSize)
                .Select(x => x.grade)
                .ToListAsync(cancellationToken);

            return new PagedResult<Grade>
            {
                Items = items,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task UpdateAsync(Grade grade, CancellationToken cancellationToken)
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
