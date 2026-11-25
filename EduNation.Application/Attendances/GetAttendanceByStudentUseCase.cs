using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.Attendances
{
    public class GetAttendanceByStudentUseCase
    {
        private readonly IAttendanceRepository _repository;

        public GetAttendanceByStudentUseCase(IAttendanceRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<AttendanceResponse>> ExecuteAsync(Guid studentId, CancellationToken cancellationToken)
        {
            var items = await _repository.GetByStudentIdAsync(studentId, cancellationToken);
            return items.Select(CreateAttendanceUseCase.ToResponse).ToList();
        }
    }
}
