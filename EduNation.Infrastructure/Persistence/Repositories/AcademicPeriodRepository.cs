using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;
using EduNation.Infrastructure.Persistence;

namespace EduNation.Infrastructure.Persistence.Repositories
{
    public class AcademicPeriodRepository : IAcademicPeriodRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public AcademicPeriodRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<AcademicPeriod> AddAsync(AcademicPeriod academicPeriod, CancellationToken cancellationToken)
        {
            await _applicationDbContext.AcademicPeriods.AddAsync(academicPeriod, cancellationToken);
            try
            {
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
            {
                throw new ConflictException("An academic period with this code already exists.");
            }

            return academicPeriod;
        }

        public Task<AcademicPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _applicationDbContext.AcademicPeriods
                .FirstOrDefaultAsync(academicPeriod => academicPeriod.Id == id && academicPeriod.IsActive, cancellationToken);
        }

        public async Task<IReadOnlyList<AcademicPeriod>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _applicationDbContext.AcademicPeriods
                .AsNoTracking()
                .Where(academicPeriod => academicPeriod.IsActive)
                .OrderBy(academicPeriod => academicPeriod.Code)
                .ThenBy(academicPeriod => academicPeriod.Id)
                .ToListAsync(cancellationToken);
        }

        public Task<bool> ExistsByCodeAsync(string code, Guid? excludedId, CancellationToken cancellationToken)
        {
            return _applicationDbContext.AcademicPeriods.AnyAsync(
                academicPeriod => academicPeriod.Code == code && (!excludedId.HasValue || academicPeriod.Id != excludedId.Value),
                cancellationToken);
        }

        public async Task UpdateAsync(AcademicPeriod academicPeriod, CancellationToken cancellationToken)
        {
            try
            {
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
            {
                throw new ConflictException("An academic period with this code already exists.");
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
