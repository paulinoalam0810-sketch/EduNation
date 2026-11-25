using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.Grades
{
    public class GetGradesByCourseSectionUseCase
    {
        private readonly IGradeRepository _repository;

        public GetGradesByCourseSectionUseCase(IGradeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<GradeResponse>> ExecuteAsync(Guid courseSectionId, CancellationToken cancellationToken)
        {
            var grades = await _repository.GetByCourseSectionIdAsync(courseSectionId, cancellationToken);
            return grades.Select(CreateGradeUseCase.ToResponse).ToList();
        }
    }
}
