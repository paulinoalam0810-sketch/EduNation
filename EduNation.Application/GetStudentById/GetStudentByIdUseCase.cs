using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Application.Responses;

namespace EduNation.Application.GetStudentById
{
    public class GetStudentByIdUseCase
    {
        private readonly IStudentRepository _studentRepository;

        public GetStudentByIdUseCase(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<StudentResponse> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(id, cancellationToken);

            if (student == null)
            {
                throw new NotFoundException("Student not found.");
            }

            return new StudentResponse
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                CreatedAt = student.CreatedAt,
                IsActive = student.IsActive
            };
        }
    }
}
