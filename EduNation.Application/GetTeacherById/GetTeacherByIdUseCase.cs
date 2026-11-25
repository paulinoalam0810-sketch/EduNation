using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Application.Responses;

namespace EduNation.Application.GetTeacherById
{
    public class GetTeacherByIdUseCase
    {
        private readonly ITeacherRepository _teacherRepository;

        public GetTeacherByIdUseCase(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task<TeacherResponse> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id, cancellationToken);
            if (teacher is null)
                throw new NotFoundException("Teacher not found.");

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
