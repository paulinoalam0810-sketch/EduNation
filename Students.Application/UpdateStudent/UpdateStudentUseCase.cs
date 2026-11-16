using Students.Application.Abstractions.Persistence;
using Students.Domain.Entities;
using Students.Application.Exceptions;

namespace Students.Application.UpdateStudent
{
    public class UpdateStudentUseCase
    {
        private readonly IStudentRepository _studentRepository;

        public UpdateStudentUseCase(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<Student> ExecuteAsync(Guid id, UpdateStudentRequest request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(id, cancellationToken);

            if (student == null)
                 throw new NotFoundException("Student not found.");

            student.UpdatePersonalInfo
            (
                request.FirstName,
                request.LastName,
                request.Email,
                request.DateOfBirth
            );

            await _studentRepository.UpdateAsync(student, cancellationToken);

            return student;
        }
    }
}
