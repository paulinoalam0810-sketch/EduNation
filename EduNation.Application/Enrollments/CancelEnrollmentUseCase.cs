using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.Enrollments
{
    public class CancelEnrollmentUseCase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public CancelEnrollmentUseCase(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(id, cancellationToken);
            if (enrollment is null)
                throw new NotFoundException("Enrollment not found.");

            if (enrollment.Status == EnrollmentStatus.Cancelled)
                throw new ConflictException("Enrollment is already cancelled.");

            enrollment.Cancel();
            await _enrollmentRepository.UpdateAsync(enrollment, cancellationToken);
        }
    }
}
