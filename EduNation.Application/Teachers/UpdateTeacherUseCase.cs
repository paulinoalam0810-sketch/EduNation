using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.Teachers
{
    public class UpdateTeacherUseCase
    {
        private readonly ITeacherRepository _teacherRepository;

        public UpdateTeacherUseCase(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task<TeacherResponse> ExecuteAsync(Guid id, TeacherRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var teacher = await _teacherRepository.GetByIdAsync(id, cancellationToken);
            if (teacher is null)
                throw new NotFoundException("Teacher not found.");

            teacher.UpdatePersonalInfo(request.FirstName, request.LastName, request.Email, request.PhoneNumber);
            await _teacherRepository.UpdateAsync(teacher, cancellationToken);

            return new TeacherResponse
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
