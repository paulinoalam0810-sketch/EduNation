using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;

namespace EduNation.Application.Courses
{
    public class UpdateCourseUseCase
    {
        private readonly ICourseRepository _courseRepository;

        public UpdateCourseUseCase(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<CourseResponse> ExecuteAsync(Guid id, CourseRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var course = await _courseRepository.GetByIdAsync(id, cancellationToken);
            if (course is null)
                throw new NotFoundException("Course not found.");

            course.Update(request.Code, request.Name, request.Description, request.Credits);
            if (await _courseRepository.ExistsByCodeAsync(course.Code, id, cancellationToken))
                throw new ConflictException("A course with this code already exists.");

            await _courseRepository.UpdateAsync(course, cancellationToken);
            return CourseMapper.ToResponse(course);
        }
    }
}
