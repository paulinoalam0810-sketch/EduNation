using Students.Application.Abstractions.Persistence;
using Students.Domain.Entities;
using System.Security.Cryptography.X509Certificates;

namespace Students.Application.CreateStudent
{
    public class CreateStudentUseCase
    {
        private readonly IStudentRepository _studentRepository;

        public CreateStudentUseCase(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<CreateStudentResponse> ExecuteAsync(CreateStudentRequest request, CancellationToken cancellationToken)
        {
             var student = new Student(request.FirstName, request.LastName, request.Email, request.DateOfBirth);

             await _studentRepository.AddAsync(student, cancellationToken);

             return new CreateStudentResponse
             {
                 Id = student.Id,
                 FirstName = student.FirstName,
                 LastName = student.LastName,
                 Email = student.Email,
                 DateOfBirth = student.DateOfBirth, 
                 CreatedAt = student.CreatedAt
             };

        }
    }
}
