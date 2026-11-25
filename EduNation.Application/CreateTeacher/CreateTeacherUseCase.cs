using EduNation.Application.Abstractions.Persistence;
using EduNation.Domain.Entities;

namespace EduNation.Application.CreateTeacher
{
    public class CreateTeacherUseCase
    {
        private readonly ITeacherRepository _teacherRepository;

        public CreateTeacherUseCase(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task<CreateTeacherResponse> ExecuteAsync(CreateTeacherRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var teacher = new Teacher(request.FirstName, request.LastName, request.Email, request.PhoneNumber);
            await _teacherRepository.AddAsync(teacher, cancellationToken);

            return new CreateTeacherResponse
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email,
                PhoneNumber = teacher.PhoneNumber,
                CreatedAt = teacher.CreatedAt,
                IsActive = teacher.IsActive
            };
        }
    }
}
