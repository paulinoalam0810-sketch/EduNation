using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.Enrollments
{
    public class GetEnrollmentsByCourseSectionUseCase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public GetEnrollmentsByCourseSectionUseCase(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IReadOnlyList<EnrollmentResponse>> ExecuteAsync(Guid courseSectionId, CancellationToken cancellationToken)
        {
            var enrollments = await _enrollmentRepository.GetByCourseSectionIdAsync(courseSectionId, cancellationToken);
            return enrollments.Select(CreateEnrollmentUseCase.ToResponse).ToList();
        }
    }
}
