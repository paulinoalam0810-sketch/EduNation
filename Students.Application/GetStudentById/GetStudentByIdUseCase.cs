using Students.Application.Abstractions.Persistence;
using Students.Application.Exceptions;
using Students.Domain.Entities;


namespace Students.Application.GetStudentById
{
    public class GetStudentByIdUseCase
    {
        private readonly IStudentRepository _studentRepository;

        public GetStudentByIdUseCase(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;        
        }

        public async Task<Student> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(id, cancellationToken);

            if (student == null)
                throw new NotFoundException("Student not found.");

            return student;
        }
    }
}
