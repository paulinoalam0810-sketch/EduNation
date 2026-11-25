using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.Attendances
{
    public class GetAttendanceByCourseSectionAndDateUseCase
    {
        private readonly IAttendanceRepository _repository;

        public GetAttendanceByCourseSectionAndDateUseCase(IAttendanceRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<AttendanceResponse>> ExecuteAsync(Guid courseSectionId, DateOnly date, CancellationToken cancellationToken)
        {
            var items = await _repository.GetByCourseSectionAndDateAsync(courseSectionId, date, cancellationToken);
            return items.Select(CreateAttendanceUseCase.ToResponse).ToList();
        }
    }
}
