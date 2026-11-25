using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.CourseSections
{
    public class CreateCourseSectionUseCase
    {
        private readonly ICourseSectionRepository _courseSectionRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IAcademicPeriodRepository _academicPeriodRepository;
        private readonly ITeacherRepository _teacherRepository;

        public CreateCourseSectionUseCase(
            ICourseSectionRepository courseSectionRepository,
            ICourseRepository courseRepository,
            IAcademicPeriodRepository academicPeriodRepository,
            ITeacherRepository teacherRepository)
        {
            _courseSectionRepository = courseSectionRepository;
            _courseRepository = courseRepository;
            _academicPeriodRepository = academicPeriodRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task<CourseSectionResponse> ExecuteAsync(CourseSectionRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course is null)
                throw new NotFoundException("Course not found or inactive.");

            var academicPeriod = await _academicPeriodRepository.GetByIdAsync(request.AcademicPeriodId, cancellationToken);
            if (academicPeriod is null)
                throw new NotFoundException("Academic period not found or inactive.");

            var teacher = await _teacherRepository.GetByIdAsync(request.TeacherId, cancellationToken);
            if (teacher is null)
                throw new NotFoundException("Teacher not found or inactive.");

            var courseSection = new CourseSection(request.CourseId, request.AcademicPeriodId, request.TeacherId, request.Code, request.Name, request.Capacity);

            if (await _courseSectionRepository.ExistsByCodeAsync(courseSection.Code, courseSection.CourseId, courseSection.AcademicPeriodId, null, cancellationToken))
                throw new ConflictException("A course section with this code already exists for the course and period.");

            await _courseSectionRepository.AddAsync(courseSection, cancellationToken);
            return ToResponse(courseSection);
        }

        internal static CourseSectionResponse ToResponse(CourseSection courseSection)
        {
            return new CourseSectionResponse
            {
                Id = courseSection.Id,
                CourseId = courseSection.CourseId,
                AcademicPeriodId = courseSection.AcademicPeriodId,
                TeacherId = courseSection.TeacherId,
                Code = courseSection.Code,
                Name = courseSection.Name,
                Capacity = courseSection.Capacity,
                CreatedAt = courseSection.CreatedAt,
                IsActive = courseSection.IsActive
            };
        }
    }
}
