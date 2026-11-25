using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Application.Responses;

namespace EduNation.Application.UpdateStudent
{
    public class UpdateStudentUseCase
    {
        private readonly IStudentRepository _studentRepository;

        public UpdateStudentUseCase(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<StudentResponse> ExecuteAsync(Guid id, UpdateStudentRequest request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(id, cancellationToken);

            if (student == null)
            {
                throw new NotFoundException("Student not found.");
            }

            student.UpdatePersonalInfo(
                request.FirstName,
                request.LastName,
                request.Email,
                request.DateOfBirth);

            await _studentRepository.UpdateAsync(student, cancellationToken);

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
