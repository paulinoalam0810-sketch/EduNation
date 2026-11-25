using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.Grades
{
    public class UpdateEvaluationUseCase
    {
        private readonly IEvaluationRepository _repository;
        private readonly ICourseSectionRepository _courseSectionRepository;

        public UpdateEvaluationUseCase(IEvaluationRepository repository, ICourseSectionRepository courseSectionRepository)
        {
            _repository = repository;
            _courseSectionRepository = courseSectionRepository;
        }

        public async Task<EvaluationResponse> ExecuteAsync(Guid id, EvaluationRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var evaluation = await _repository.GetByIdAsync(id, cancellationToken);
            if (evaluation is null)
                throw new NotFoundException("Evaluation not found.");

            var courseSection = await _courseSectionRepository.GetByIdAsync(request.CourseSectionId, cancellationToken);
            if (courseSection is null)
                throw new NotFoundException("Course section not found or inactive.");

            evaluation.Update(request.Name, request.Description, request.MaxScore, request.Weight, request.EvaluationDate);
            await _repository.UpdateAsync(evaluation, cancellationToken);

            return CreateEvaluationUseCase.ToResponse(evaluation);
        }
    }
}
