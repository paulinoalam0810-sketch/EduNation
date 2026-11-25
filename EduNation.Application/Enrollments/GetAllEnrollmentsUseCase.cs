using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.Enrollments
{
    public class GetAllEnrollmentsUseCase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public GetAllEnrollmentsUseCase(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IReadOnlyList<EnrollmentResponse>> ExecuteAsync(CancellationToken cancellationToken)
        {
            var enrollments = await _enrollmentRepository.GetAllAsync(cancellationToken);
            return enrollments.Select(CreateEnrollmentUseCase.ToResponse).ToList();
        }
    }
}
