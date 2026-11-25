using Moq;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.CourseSections;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class CourseSectionUseCasesTests
    {
        private readonly Mock<ICourseSectionRepository> _sectionRepository = new();
        private readonly Mock<ICourseRepository> _courseRepository = new();
        private readonly Mock<IAcademicPeriodRepository> _periodRepository = new();
        private readonly Mock<ITeacherRepository> _teacherRepository = new();

        private CreateCourseSectionUseCase CreateUseCase() =>
            new(_sectionRepository.Object, _courseRepository.Object, _periodRepository.Object, _teacherRepository.Object);

        private UpdateCourseSectionUseCase UpdateUseCase() =>
            new(_sectionRepository.Object, _courseRepository.Object, _periodRepository.Object, _teacherRepository.Object);

        [Fact]
        public async Task Create_ShouldPersistSection_WhenReferencesAreValid()
        {
            var course = new Course("CS-101", "Computer Science", null, 3);
            var period = new AcademicPeriod("Fall 2025", "FALL-2025", new DateOnly(2025, 9, 1), new DateOnly(2025, 12, 15));
            var teacher = new Teacher("Alam", "Paulino", "alam@example.com");

            _courseRepository.Setup(r => r.GetByIdAsync(course.Id, It.IsAny<CancellationToken>())).ReturnsAsync(course);
            _periodRepository.Setup(r => r.GetByIdAsync(period.Id, It.IsAny<CancellationToken>())).ReturnsAsync(period);
            _teacherRepository.Setup(r => r.GetByIdAsync(teacher.Id, It.IsAny<CancellationToken>())).ReturnsAsync(teacher);
            _sectionRepository.Setup(r => r.ExistsByCodeAsync("SEC-1", course.Id, period.Id, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _sectionRepository.Setup(r => r.AddAsync(It.IsAny<CourseSection>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseSection section, CancellationToken _) => section);

            var result = await CreateUseCase().ExecuteAsync(new CourseSectionRequest
            {
                CourseId = course.Id,
                AcademicPeriodId = period.Id,
                TeacherId = teacher.Id,
                Code = "sec-1",
                Name = "Morning",
                Capacity = 30
            }, CancellationToken.None);

            Assert.Equal("SEC-1", result.Code);
            _sectionRepository.Verify(r => r.AddAsync(It.IsAny<CourseSection>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldThrowNotFound_WhenCourseIsMissing()
        {
            var period = new AcademicPeriod("Fall 2025", "FALL-2025", new DateOnly(2025, 9, 1), new DateOnly(2025, 12, 15));
            var teacher = new Teacher("Alam", "Paulino", "alam@example.com");

            _courseRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Course?)null);
            _periodRepository.Setup(r => r.GetByIdAsync(period.Id, It.IsAny<CancellationToken>())).ReturnsAsync(period);
            _teacherRepository.Setup(r => r.GetByIdAsync(teacher.Id, It.IsAny<CancellationToken>())).ReturnsAsync(teacher);

            await Assert.ThrowsAsync<NotFoundException>(() => CreateUseCase().ExecuteAsync(new CourseSectionRequest
            {
                CourseId = Guid.NewGuid(),
                AcademicPeriodId = period.Id,
                TeacherId = teacher.Id,
                Code = "SEC-1",
                Capacity = 30
            }, CancellationToken.None));
        }

        [Fact]
        public async Task Create_ShouldThrowConflict_WhenCodeAlreadyExists()
        {
            var course = new Course("CS-101", "Computer Science", null, 3);
            var period = new AcademicPeriod("Fall 2025", "FALL-2025", new DateOnly(2025, 9, 1), new DateOnly(2025, 12, 15));
            var teacher = new Teacher("Alam", "Paulino", "alam@example.com");

            _courseRepository.Setup(r => r.GetByIdAsync(course.Id, It.IsAny<CancellationToken>())).ReturnsAsync(course);
            _periodRepository.Setup(r => r.GetByIdAsync(period.Id, It.IsAny<CancellationToken>())).ReturnsAsync(period);
            _teacherRepository.Setup(r => r.GetByIdAsync(teacher.Id, It.IsAny<CancellationToken>())).ReturnsAsync(teacher);
            _sectionRepository.Setup(r => r.ExistsByCodeAsync("SEC-1", course.Id, period.Id, null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<ConflictException>(() => CreateUseCase().ExecuteAsync(new CourseSectionRequest
            {
                CourseId = course.Id,
                AcademicPeriodId = period.Id,
                TeacherId = teacher.Id,
                Code = "sec-1",
                Capacity = 30
            }, CancellationToken.None));

            _sectionRepository.Verify(r => r.AddAsync(It.IsAny<CourseSection>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetById_ShouldThrowNotFound_WhenMissing()
        {
            _sectionRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseSection?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => new GetCourseSectionByIdUseCase(_sectionRepository.Object)
                .ExecuteAsync(Guid.NewGuid(), CancellationToken.None));
        }

        [Fact]
        public async Task Update_ShouldThrowConflict_WhenAnotherSectionUsesCode()
        {
            var course = new Course("CS-101", "Computer Science", null, 3);
            var period = new AcademicPeriod("Fall 2025", "FALL-2025", new DateOnly(2025, 9, 1), new DateOnly(2025, 12, 15));
            var teacher = new Teacher("Alam", "Paulino", "alam@example.com");
            var section = new CourseSection(course.Id, period.Id, teacher.Id, "SEC-1", "Morning", 30);

            _sectionRepository.Setup(r => r.GetByIdAsync(section.Id, It.IsAny<CancellationToken>())).ReturnsAsync(section);
            _courseRepository.Setup(r => r.GetByIdAsync(course.Id, It.IsAny<CancellationToken>())).ReturnsAsync(course);
            _periodRepository.Setup(r => r.GetByIdAsync(period.Id, It.IsAny<CancellationToken>())).ReturnsAsync(period);
            _teacherRepository.Setup(r => r.GetByIdAsync(teacher.Id, It.IsAny<CancellationToken>())).ReturnsAsync(teacher);
            _sectionRepository.Setup(r => r.ExistsByCodeAsync("SEC-2", course.Id, period.Id, section.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<ConflictException>(() => UpdateUseCase().ExecuteAsync(section.Id, new CourseSectionRequest
            {
                CourseId = course.Id,
                AcademicPeriodId = period.Id,
                TeacherId = teacher.Id,
                Code = "sec-2",
                Capacity = 30
            }, CancellationToken.None));

            _sectionRepository.Verify(r => r.UpdateAsync(It.IsAny<CourseSection>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Deactivate_ShouldSetInactiveAndSave()
        {
            var section = new CourseSection(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "SEC-1", "Morning", 30);
            _sectionRepository.Setup(r => r.GetByIdAsync(section.Id, It.IsAny<CancellationToken>())).ReturnsAsync(section);

            await new DeactivateCourseSectionUseCase(_sectionRepository.Object).ExecuteAsync(section.Id, CancellationToken.None);

            Assert.False(section.IsActive);
            _sectionRepository.Verify(r => r.UpdateAsync(section, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
