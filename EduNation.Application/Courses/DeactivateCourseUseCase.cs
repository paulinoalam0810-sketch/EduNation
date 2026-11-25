using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.Courses
{
    public class DeactivateCourseUseCase
    {
        private readonly ICourseRepository _courseRepository;

        public DeactivateCourseUseCase(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
            if (course is null)
                throw new NotFoundException("Course not found.");

            course.Deactivate();
            await _courseRepository.UpdateAsync(course, cancellationToken);
        }
    }
}
