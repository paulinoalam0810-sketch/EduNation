using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Responses;
using EduNation.Application.SearchTeachers;

namespace EduNation.Application.SearchTeachers
{
    public sealed class SearchTeachersUseCase
    {
        private readonly ITeacherRepository _teacherRepository;

        public SearchTeachersUseCase(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task<PagedResult<TeacherResponse>> ExecuteAsync(
            TeacherSearchCriteria criteria,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(criteria);

            if (criteria.PageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(criteria.PageNumber), "Page number must be at least 1.");

            if (criteria.PageSize is < 1 or > 100)
                throw new ArgumentOutOfRangeException(nameof(criteria.PageSize), "Page size must be between 1 and 100.");

            var normalizedCriteria = new TeacherSearchCriteria
            {
                Search = string.IsNullOrWhiteSpace(criteria.Search) ? null : criteria.Search.Trim(),
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize
            };

            var result = await _teacherRepository.SearchAsync(normalizedCriteria, cancellationToken);

            return new PagedResult<TeacherResponse>
            {
                Items = result.Items.Select(teacher => new TeacherResponse
                {
                    Id = teacher.Id,
                    FirstName = teacher.FirstName,
                    LastName = teacher.LastName,
                    Email = teacher.Email,
                    PhoneNumber = teacher.PhoneNumber,
                    CreatedAt = teacher.CreatedAt,
                    IsActive = teacher.IsActive
                }).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}
