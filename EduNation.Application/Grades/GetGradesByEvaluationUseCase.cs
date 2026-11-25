using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.Grades
{
    public class GetGradesByEvaluationUseCase
    {
        private readonly IGradeRepository _repository;

        public GetGradesByEvaluationUseCase(IGradeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<GradeResponse>> ExecuteAsync(Guid evaluationId, CancellationToken cancellationToken)
        {
            var grades = await _repository.GetByEvaluationIdAsync(evaluationId, cancellationToken);
            return grades.Select(CreateGradeUseCase.ToResponse).ToList();
        }
    }
}
