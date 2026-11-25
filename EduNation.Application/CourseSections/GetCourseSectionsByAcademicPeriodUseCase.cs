using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.CourseSections
{
    public class GetCourseSectionsByAcademicPeriodUseCase
    {
        private readonly ICourseSectionRepository _repository;

        public GetCourseSectionsByAcademicPeriodUseCase(ICourseSectionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<CourseSectionResponse>> ExecuteAsync(Guid academicPeriodId, CancellationToken cancellationToken)
        {
            var courseSections = await _repository.GetByAcademicPeriodIdAsync(academicPeriodId, cancellationToken);
            return courseSections.Select(CreateCourseSectionUseCase.ToResponse).ToList();
        }
    }
}
