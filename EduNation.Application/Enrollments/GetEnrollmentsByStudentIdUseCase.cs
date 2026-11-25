using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.Enrollments
{
    public class GetEnrollmentsByStudentIdUseCase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public GetEnrollmentsByStudentIdUseCase(IEnrollmentRepository enrollmentRepository)
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
