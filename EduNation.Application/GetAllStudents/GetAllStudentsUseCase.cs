using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Responses;

namespace EduNation.Application.GetAllStudents
{
    public class GetAllStudentsUseCase
    {
        private readonly IStudentRepository _studentRepository;

        public GetAllStudentsUseCase(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<IReadOnlyList<StudentResponse>> ExecuteAsync(CancellationToken cancellationToken)
        {
            var students = await _studentRepository.GetAllAsync(cancellationToken);

            return students.Select(student => new StudentResponse
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                CreatedAt = student.CreatedAt,
                IsActive = student.IsActive
            }).ToList();
        }
    }
}
