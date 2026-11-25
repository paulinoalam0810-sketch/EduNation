using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.Grades
{
    public class CreateGradeUseCase
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public CreateGradeUseCase(IGradeRepository gradeRepository, IEvaluationRepository evaluationRepository, IEnrollmentRepository enrollmentRepository)
        {
            _gradeRepository = gradeRepository;
            _evaluationRepository = evaluationRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<GradeResponse> ExecuteAsync(GradeRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var evaluation = await _evaluationRepository.GetByIdAsync(request.EvaluationId, cancellationToken);
            if (evaluation is null)
                throw new NotFoundException("Evaluation not found or inactive.");

            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
            if (enrollment is null || enrollment.Status != EnrollmentStatus.Active)
                throw new NotFoundException("Enrollment not found or inactive.");

            if (enrollment.CourseSectionId != evaluation.CourseSectionId)
                throw new ConflictException("Enrollment does not belong to the evaluation's course section.");

            if (request.Score < 0 || request.Score > evaluation.MaxScore)
                throw new ArgumentOutOfRangeException(nameof(request.Score), $"Score must be between 0 and {evaluation.MaxScore}.");

            var grade = new Grade(request.EvaluationId, request.EnrollmentId, request.Score, request.Comments);
            await _gradeRepository.AddAsync(grade, cancellationToken);

            return ToResponse(grade);
        }

        internal static GradeResponse ToResponse(Grade grade)
        {
            return new GradeResponse
            {
                Id = grade.Id,
                EvaluationId = grade.EvaluationId,
                EnrollmentId = grade.EnrollmentId,
                Score = grade.Score,
                Comments = grade.Comments,
                CreatedAt = grade.CreatedAt,
                UpdatedAt = grade.UpdatedAt
            };
        }
    }
}
