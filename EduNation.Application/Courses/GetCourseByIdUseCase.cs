using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.Courses
{
    public class GetCourseByIdUseCase
    {
        private readonly ICourseRepository _courseRepository;

        public GetCourseByIdUseCase(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<CourseResponse> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
            if (course is null)
                throw new NotFoundException("Course not found.");

            return CourseMapper.ToResponse(course);
        }
    }
}
