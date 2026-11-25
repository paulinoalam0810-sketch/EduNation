using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.Enrollments
{
    public class GetEnrollmentByIdUseCase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public GetEnrollmentByIdUseCase(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<EnrollmentResponse> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(id, cancellationToken);
            if (enrollment is null)
                throw new NotFoundException("Enrollment not found.");

            return CreateEnrollmentUseCase.ToResponse(enrollment);
        }
    }
}
