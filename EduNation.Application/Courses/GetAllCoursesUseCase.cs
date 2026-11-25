using EduNation.Application.Abstractions.Persistence;

namespace EduNation.Application.Courses
{
    public class GetAllCoursesUseCase
    {
        private readonly ICourseRepository _courseRepository;

        public GetAllCoursesUseCase(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<IReadOnlyList<CourseResponse>> ExecuteAsync(CancellationToken cancellationToken)
        {
            var courses = await _courseRepository.GetAllAsync(cancellationToken);
            return courses.Select(CourseMapper.ToResponse).ToList();
        }
    }
}
