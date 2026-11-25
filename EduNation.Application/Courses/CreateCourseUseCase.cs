using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.Courses
{
    public class CreateCourseUseCase
    {
        private readonly ICourseRepository _courseRepository;

        public CreateCourseUseCase(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<CourseResponse> ExecuteAsync(CourseRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var course = new Course(request.Code, request.Name, request.Description, request.Credits);

            if (await _courseRepository.ExistsByCodeAsync(course.Code, null, cancellationToken))
                throw new ConflictException("A course with this code already exists.");

            await _courseRepository.AddAsync(course, cancellationToken);
            return CourseMapper.ToResponse(course);
        }
    }
}
