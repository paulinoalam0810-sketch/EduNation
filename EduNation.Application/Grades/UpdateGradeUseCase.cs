using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.Grades
{
    public class UpdateGradeUseCase
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly IEvaluationRepository _evaluationRepository;

        public UpdateGradeUseCase(IGradeRepository gradeRepository, IEvaluationRepository evaluationRepository)
        {
            _gradeRepository = gradeRepository;
            _evaluationRepository = evaluationRepository;
        }

        public Task<GradeResponse> ExecuteAsync(Guid id, UpdateGradeRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            return ExecuteAsync(id, request.Score, request.Comments, cancellationToken);
        }

        public async Task<GradeResponse> ExecuteAsync(Guid id, decimal score, string? comments, CancellationToken cancellationToken)
        {
            var grade = await _gradeRepository.GetByIdAsync(id, cancellationToken);
            if (grade is null)
                throw new NotFoundException("Grade not found.");

            var evaluation = await _evaluationRepository.GetByIdAsync(grade.EvaluationId, cancellationToken);
            if (evaluation is null)
                throw new NotFoundException("Evaluation not found or inactive.");

            if (score < 0 || score > evaluation.MaxScore)
                throw new ArgumentOutOfRangeException(nameof(score), $"Score must be between 0 and {evaluation.MaxScore}.");

            grade.Update(score, comments);
            await _gradeRepository.UpdateAsync(grade, cancellationToken);

            return CreateGradeUseCase.ToResponse(grade);
        }
    }
}
