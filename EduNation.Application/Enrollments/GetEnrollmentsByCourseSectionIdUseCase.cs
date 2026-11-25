using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.Enrollments
{
    public class GetEnrollmentsByCourseSectionIdUseCase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public GetEnrollmentsByCourseSectionIdUseCase(IEnrollmentRepository enrollmentRepository)
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
