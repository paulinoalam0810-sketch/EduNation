using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.Teachers
{
    public class DeactivateTeacherUseCase
    {
        private readonly ITeacherRepository _teacherRepository;

        public DeactivateTeacherUseCase(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id, cancellationToken);
            if (teacher is null)
                throw new NotFoundException("Teacher not found.");

            teacher.Deactivate();
            await _teacherRepository.UpdateAsync(teacher, cancellationToken);
        }
    }
}
