using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.CourseSections
{
    public class GetCourseSectionsByAcademicPeriodIdUseCase
    {
        private readonly ICourseSectionRepository _repository;

        public GetCourseSectionsByAcademicPeriodIdUseCase(ICourseSectionRepository repository)
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
