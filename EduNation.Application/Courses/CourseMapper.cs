using EduNation.Domain.Entities;

namespace EduNation.Application.Courses
{
    internal static class CourseMapper
    {
        public static CourseResponse ToResponse(Course course)
        {
            return new CourseResponse
            {
                Id = course.Id,
                Code = course.Code,
                Name = course.Name,
                Description = course.Description,
                Credits = course.Credits,
                CreatedAt = course.CreatedAt,
                IsActive = course.IsActive
            };
        }
    }
}
