using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.Grades
{
    public class GetEvaluationsByCourseSectionUseCase
    {
        private readonly IEvaluationRepository _repository;

        public GetEvaluationsByCourseSectionUseCase(IEvaluationRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<EvaluationResponse>> ExecuteAsync(Guid courseSectionId, CancellationToken cancellationToken)
        {
            var evaluations = await _repository.GetByCourseSectionIdAsync(courseSectionId, cancellationToken);
            return evaluations.Select(CreateEvaluationUseCase.ToResponse).ToList();
        }
    }
}
