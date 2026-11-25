using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.CourseSections
{
    public class DeactivateCourseSectionUseCase
    {
        private readonly ICourseSectionRepository _repository;

        public DeactivateCourseSectionUseCase(ICourseSectionRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var courseSection = await _repository.GetByIdAsync(id, cancellationToken);
            if (courseSection is null)
                throw new NotFoundException("Course section not found.");

            courseSection.Deactivate();
            await _repository.UpdateAsync(courseSection, cancellationToken);
        }
    }
}
