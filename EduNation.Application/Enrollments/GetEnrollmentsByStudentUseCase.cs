using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.Enrollments
{
    public class GetEnrollmentsByStudentUseCase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public GetEnrollmentsByStudentUseCase(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IReadOnlyList<EnrollmentResponse>> ExecuteAsync(Guid studentId, CancellationToken cancellationToken)
        {
            var enrollments = await _enrollmentRepository.GetByStudentIdAsync(studentId, cancellationToken);
            return enrollments.Select(CreateEnrollmentUseCase.ToResponse).ToList();
        }
    }
}
