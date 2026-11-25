using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.SearchAttendances;

namespace EduNation.Application.Attendances
{
    public sealed class SearchAttendancesUseCase
    {
        private readonly IAttendanceRepository _repository;

        public SearchAttendancesUseCase(IAttendanceRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<AttendanceResponse>> ExecuteAsync(
            AttendanceSearchCriteria criteria,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(criteria);

            if (criteria.PageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(criteria.PageNumber), "Page number must be at least 1.");

            if (criteria.PageSize is < 1 or > 100)
                throw new ArgumentOutOfRangeException(nameof(criteria.PageSize), "Page size must be between 1 and 100.");

            var normalizedCriteria = new AttendanceSearchCriteria
            {
                EnrollmentId = criteria.EnrollmentId,
                StudentId = criteria.StudentId,
                CourseSectionId = criteria.CourseSectionId,
                Date = criteria.Date,
                Status = criteria.Status,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize
            };

            var result = await _repository.SearchAsync(normalizedCriteria, cancellationToken);

            return new PagedResult<AttendanceResponse>
            {
                Items = result.Items.Select(CreateAttendanceUseCase.ToResponse).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}
