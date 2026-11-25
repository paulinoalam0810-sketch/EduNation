using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.Grades
{
    public sealed class SearchGradesUseCase
    {
        private readonly IGradeRepository _repository;

        public SearchGradesUseCase(IGradeRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<GradeResponse>> ExecuteAsync(
            GradeSearchCriteria criteria,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(criteria);

            if (criteria.PageNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(criteria.PageNumber), "Page number must be at least 1.");

            if (criteria.PageSize is < 1 or > 100)
                throw new ArgumentOutOfRangeException(nameof(criteria.PageSize), "Page size must be between 1 and 100.");

            var normalizedCriteria = new GradeSearchCriteria
            {
                EvaluationId = criteria.EvaluationId,
                EnrollmentId = criteria.EnrollmentId,
                StudentId = criteria.StudentId,
                CourseSectionId = criteria.CourseSectionId,
                PageNumber = criteria.PageNumber,
                PageSize = criteria.PageSize
            };

            var result = await _repository.SearchAsync(normalizedCriteria, cancellationToken);

            return new PagedResult<GradeResponse>
            {
                Items = result.Items.Select(CreateGradeUseCase.ToResponse).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}
