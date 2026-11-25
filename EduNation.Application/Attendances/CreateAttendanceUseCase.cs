using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.Attendances
{
    public class CreateAttendanceUseCase
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public CreateAttendanceUseCase(IAttendanceRepository attendanceRepository, IEnrollmentRepository enrollmentRepository)
        {
            _attendanceRepository = attendanceRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<AttendanceResponse> ExecuteAsync(AttendanceRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var enrollment = await _enrollmentRepository.GetByIdAsync(request.EnrollmentId, cancellationToken);
            if (enrollment is null || enrollment.Status != EnrollmentStatus.Active)
                throw new NotFoundException("Enrollment not found or inactive.");

            var attendance = new Attendance(request.EnrollmentId, request.Date, request.Status);
            await _attendanceRepository.AddAsync(attendance, cancellationToken);

            return ToResponse(attendance);
        }

        internal static AttendanceResponse ToResponse(Attendance attendance)
        {
            return new AttendanceResponse
            {
                Id = attendance.Id,
                EnrollmentId = attendance.EnrollmentId,
                Date = attendance.Date,
                Status = attendance.Status,
                CreatedAt = attendance.CreatedAt
            };
        }
    }
}
