using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.Grades
{
    public class GetEvaluationByIdUseCase
    {
        private readonly IEvaluationRepository _repository;

        public GetEvaluationByIdUseCase(IEvaluationRepository repository)
        {
            _repository = repository;
        }

        public async Task<EvaluationResponse> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var evaluation = await _repository.GetByIdAsync(id, cancellationToken);
            if (evaluation is null)
                throw new NotFoundException("Evaluation not found.");

            return CreateEvaluationUseCase.ToResponse(evaluation);
        }
    }
}
