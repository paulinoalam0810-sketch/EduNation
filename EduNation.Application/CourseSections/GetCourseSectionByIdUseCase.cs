using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.CourseSections
{
    public class GetCourseSectionByIdUseCase
    {
        private readonly ICourseSectionRepository _repository;

        public GetCourseSectionByIdUseCase(ICourseSectionRepository repository)
        {
            _repository = repository;
        }

        public async Task<CourseSectionResponse> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var courseSection = await _repository.GetByIdAsync(id, cancellationToken);
            if (courseSection is null)
                throw new NotFoundException("Course section not found.");

            return CreateCourseSectionUseCase.ToResponse(courseSection);
        }
    }
}
