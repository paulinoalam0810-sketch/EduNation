using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.Grades
{
    public class GetStudentSectionResultUseCase
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public GetStudentSectionResultUseCase(
            IGradeRepository gradeRepository,
            IEvaluationRepository evaluationRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _gradeRepository = gradeRepository;
            _evaluationRepository = evaluationRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<StudentSectionResultResponse> ExecuteAsync(Guid studentId, Guid courseSectionId, CancellationToken cancellationToken)
        {
            var grades = await _gradeRepository.GetByStudentIdAsync(studentId, cancellationToken);

            decimal weightedSum = 0;
            decimal totalWeight = 0;
            var gradesCount = 0;

            foreach (var grade in grades)
            {
                var enrollment = await _enrollmentRepository.GetByIdAsync(grade.EnrollmentId, cancellationToken);
                if (enrollment is null || enrollment.CourseSectionId != courseSectionId)
                    continue;

                var evaluation = await _evaluationRepository.GetByIdAsync(grade.EvaluationId, cancellationToken);
                if (evaluation is null || evaluation.MaxScore <= 0)
                    continue;

                weightedSum += (grade.Score / evaluation.MaxScore) * evaluation.Weight;
                totalWeight += evaluation.Weight;
                gradesCount++;
            }

            decimal? weightedScore = totalWeight > 0 ? weightedSum / totalWeight * 100 : null;

            return new StudentSectionResultResponse
            {
                StudentId = studentId,
                CourseSectionId = courseSectionId,
                WeightedScore = weightedScore,
                TotalWeight = totalWeight,
                GradesCount = gradesCount
            };
        }
    }
}
