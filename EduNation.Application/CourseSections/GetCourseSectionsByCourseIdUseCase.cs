using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.CourseSections
{
    public class GetCourseSectionsByCourseIdUseCase
    {
        private readonly ICourseSectionRepository _repository;

        public GetCourseSectionsByCourseIdUseCase(ICourseSectionRepository repository)
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
