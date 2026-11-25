using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.Attendances
{
    public class GetAttendanceByIdUseCase
    {
        private readonly IAttendanceRepository _repository;

        public GetAttendanceByIdUseCase(IAttendanceRepository repository)
        {
            _repository = repository;
        }

        public async Task<AttendanceResponse> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var attendance = await _repository.GetByIdAsync(id, cancellationToken);
            if (attendance is null)
                throw new NotFoundException("Attendance not found.");

            return CreateAttendanceUseCase.ToResponse(attendance);
        }
    }
}
