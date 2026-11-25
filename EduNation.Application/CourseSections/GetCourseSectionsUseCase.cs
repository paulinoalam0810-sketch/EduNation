using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.CourseSections
{
    public class GetCourseSectionsUseCase
    {
        private readonly ICourseSectionRepository _repository;

        public GetCourseSectionsUseCase(ICourseSectionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<CourseSectionResponse>> ExecuteAsync(CancellationToken cancellationToken)
        {
            var courseSections = await _repository.GetAllAsync(cancellationToken);
            return courseSections.Select(CreateCourseSectionUseCase.ToResponse).ToList();
        }
    }
}
