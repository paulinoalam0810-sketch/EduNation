using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.Grades
{
    public class CreateEvaluationUseCase
    {
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly ICourseSectionRepository _courseSectionRepository;

        public CreateEvaluationUseCase(IEvaluationRepository evaluationRepository, ICourseSectionRepository courseSectionRepository)
        {
            _evaluationRepository = evaluationRepository;
            _courseSectionRepository = courseSectionRepository;
        }

        public async Task<EvaluationResponse> ExecuteAsync(EvaluationRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var courseSection = await _courseSectionRepository.GetByIdAsync(request.CourseSectionId, cancellationToken);
            if (courseSection is null)
                throw new NotFoundException("Course section not found or inactive.");

            var evaluation = new Evaluation(request.CourseSectionId, request.Name, request.Description, request.MaxScore, request.Weight, request.EvaluationDate);
            await _evaluationRepository.AddAsync(evaluation, cancellationToken);

            return ToResponse(evaluation);
        }

        internal static EvaluationResponse ToResponse(Evaluation evaluation)
        {
            return new EvaluationResponse
            {
                Id = evaluation.Id,
                CourseSectionId = evaluation.CourseSectionId,
                Name = evaluation.Name,
                Description = evaluation.Description,
                MaxScore = evaluation.MaxScore,
                Weight = evaluation.Weight,
                EvaluationDate = evaluation.EvaluationDate,
                CreatedAt = evaluation.CreatedAt,
                IsActive = evaluation.IsActive
            };
        }
    }
}
