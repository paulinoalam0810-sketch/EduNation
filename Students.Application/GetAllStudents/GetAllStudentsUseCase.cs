using Students.Application.Abstractions.Persistence;
using Students.Domain.Entities;

namespace Students.Application.GetAllStudents
{
    public class GetAllStudentsUseCase
    {
        private readonly IStudentRepository _studentRepository;

        public GetAllStudentsUseCase(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<IReadOnlyList<Student>> ExecuteAsync(CancellationToken cancellationToken)
        {
            var students = await _studentRepository.GetAllAsync(cancellationToken);

            return students;
        }
    }
}
