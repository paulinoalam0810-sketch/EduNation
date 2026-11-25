using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.Grades
{
    public class GetGradesByStudentUseCase
    {
        private readonly IGradeRepository _repository;

        public GetGradesByStudentUseCase(IGradeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<GradeResponse>> ExecuteAsync(Guid studentId, CancellationToken cancellationToken)
        {
            var grades = await _repository.GetByStudentIdAsync(studentId, cancellationToken);
            return grades.Select(CreateGradeUseCase.ToResponse).ToList();
        }
    }
}
