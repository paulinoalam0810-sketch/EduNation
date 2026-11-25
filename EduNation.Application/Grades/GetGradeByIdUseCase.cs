using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.Grades
{
    public class GetGradeByIdUseCase
    {
        private readonly IGradeRepository _repository;

        public GetGradeByIdUseCase(IGradeRepository repository)
        {
            _repository = repository;
        }

        public async Task<GradeResponse> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var grade = await _repository.GetByIdAsync(id, cancellationToken);
            if (grade is null)
                throw new NotFoundException("Grade not found.");

            return CreateGradeUseCase.ToResponse(grade);
        }
    }
}
