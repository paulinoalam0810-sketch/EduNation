using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Responses;

namespace EduNation.Application.SearchStudents
{
    public sealed class SearchStudentsUseCase
    {
        private readonly IStudentRepository _studentRepository;

        public SearchStudentsUseCase(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<PagedResult<StudentResponse>> ExecuteAsync(
            StudentSearchCriteria criteria,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(criteria);

            if (criteria.PageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(criteria.PageNumber), "Page number must be at least 1.");
            }

            if (criteria.PageSize is < 1 or > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(criteria.PageSize), "Page size must be between 1 and 100.");
            }

            var normalizedCriteria = new StudentSearchCriteria
            {
                Search = string.IsNullOrWhiteSpace(criteria.Search) ? null : criteria.Search.Trim(),
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize
            };

            var result = await _studentRepository.SearchAsync(normalizedCriteria, cancellationToken);

            return new PagedResult<StudentResponse>
            {
                Items = result.Items.Select(student => new StudentResponse
                {
                    Id = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    Email = student.Email,
                    DateOfBirth = student.DateOfBirth,
                    CreatedAt = student.CreatedAt,
                    IsActive = student.IsActive
                }).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}
