using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.CourseSections
{
    public class GetCourseSectionsByTeacherUseCase
    {
        private readonly ICourseSectionRepository _repository;

        public GetCourseSectionsByTeacherUseCase(ICourseSectionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<CourseSectionResponse>> ExecuteAsync(Guid teacherId, CancellationToken cancellationToken)
        {
            var courseSections = await _repository.GetByTeacherIdAsync(teacherId, cancellationToken);
            return courseSections.Select(CreateCourseSectionUseCase.ToResponse).ToList();
        }
    }
}
