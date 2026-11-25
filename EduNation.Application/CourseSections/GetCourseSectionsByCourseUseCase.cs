using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.CourseSections
{
    public class GetCourseSectionsByCourseUseCase
    {
        private readonly ICourseSectionRepository _repository;

        public GetCourseSectionsByCourseUseCase(ICourseSectionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<CourseSectionResponse>> ExecuteAsync(Guid courseId, CancellationToken cancellationToken)
        {
            var courseSections = await _repository.GetByCourseIdAsync(courseId, cancellationToken);
            return courseSections.Select(CreateCourseSectionUseCase.ToResponse).ToList();
        }
    }
}
