using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.Enrollments
{
    public class CreateEnrollmentUseCase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICourseSectionRepository _courseSectionRepository;

        public CreateEnrollmentUseCase(
            IEnrollmentRepository enrollmentRepository,
            IStudentRepository studentRepository,
            ICourseSectionRepository courseSectionRepository)
        {
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _courseSectionRepository = courseSectionRepository;
        }

        public async Task<EnrollmentResponse> ExecuteAsync(EnrollmentRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student is null)
                throw new NotFoundException("Student not found or inactive.");

            var courseSection = await _courseSectionRepository.GetByIdAsync(request.CourseSectionId, cancellationToken);
            if (courseSection is null)
                throw new NotFoundException("Course section not found or inactive.");

            var enrollment = new Enrollment(request.StudentId, request.CourseSectionId);

            await _enrollmentRepository.AddAsync(enrollment, cancellationToken);
            return ToResponse(enrollment);
        }

        internal static EnrollmentResponse ToResponse(Enrollment enrollment)
        {
            return new EnrollmentResponse
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                CourseSectionId = enrollment.CourseSectionId,
                EnrolledAt = enrollment.EnrolledAt,
                Status = enrollment.Status,
                CreatedAt = enrollment.CreatedAt
            };
        }
    }
}
