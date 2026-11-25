using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.DeactivateStudent
{
    public class DeactivateStudentUseCase
    {
        private readonly IStudentRepository _studentRepository;

        public DeactivateStudentUseCase(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(id, cancellationToken);

            if (student == null)
                throw new NotFoundException("Student not found.");

            student.Deactivate();

            await _studentRepository.UpdateAsync(student, cancellationToken);
        }
    }
}
