using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.Attendances
{
    public class UpdateAttendanceUseCase
    {
        private readonly IAttendanceRepository _repository;

        public UpdateAttendanceUseCase(IAttendanceRepository repository)
        {
            _repository = repository;
        }

        public async Task<AttendanceResponse> ExecuteAsync(Guid id, AttendanceStatus status, CancellationToken cancellationToken)
        {
            var attendance = await _repository.GetByIdAsync(id, cancellationToken);
            if (attendance is null)
                throw new NotFoundException("Attendance not found.");

            attendance.UpdateStatus(status);
            await _repository.UpdateAsync(attendance, cancellationToken);

            return CreateAttendanceUseCase.ToResponse(attendance);
        }
    }
}
