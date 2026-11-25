using EduNation.Application.Abstractions.Persistence;
using EduNation.Domain.Entities;
using System.Security.Cryptography.X509Certificates;

namespace EduNation.Application.CreateStudent
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
