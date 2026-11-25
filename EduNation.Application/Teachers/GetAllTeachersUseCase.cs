using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.Teachers
{
    public class GetAllTeachersUseCase
    {
        private readonly ITeacherRepository _teacherRepository;

        public GetAllTeachersUseCase(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task<IReadOnlyList<TeacherResponse>> ExecuteAsync(CancellationToken cancellationToken)
        {
            var teachers = await _teacherRepository.GetAllAsync(cancellationToken);
            return teachers.Select(teacher => new TeacherResponse
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email,
                PhoneNumber = teacher.PhoneNumber,
                CreatedAt = teacher.CreatedAt,
                IsActive = teacher.IsActive
            }).ToList();
        }
    }
}
