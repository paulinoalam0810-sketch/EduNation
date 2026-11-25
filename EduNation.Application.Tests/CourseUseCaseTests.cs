using Moq;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Courses;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class CourseUseCaseTests
    {
        private readonly Mock<ICourseRepository> _repository = new();

        [Fact]
        public async Task Create_ShouldPersistNormalizedCourse_WhenCodeIsAvailable()
        {
            _repository.Setup(repository => repository.ExistsByCodeAsync("CS-101", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _repository.Setup(repository => repository.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Course course, CancellationToken _) => course);

            var result = await new CreateCourseUseCase(_repository.Object).ExecuteAsync(
                new CourseRequest { Code = "cs-101", Name = "Computer Science", Credits = 3 }, CancellationToken.None);

            Assert.Equal("CS-101", result.Code);
            Assert.True(result.IsActive);
            _repository.Verify(repository => repository.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldThrowConflict_WhenCodeAlreadyExists()
        {
            _repository.Setup(repository => repository.ExistsByCodeAsync("CS-101", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ConflictException>(() => new CreateCourseUseCase(_repository.Object).ExecuteAsync(
                new CourseRequest { Code = "cs-101", Name = "Computer Science", Credits = 3 }, CancellationToken.None));

            _repository.Verify(repository => repository.AddAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetById_ShouldThrowNotFound_WhenCourseIsMissing()
        {
            _repository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Course?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => new GetCourseByIdUseCase(_repository.Object)
                .ExecuteAsync(Guid.NewGuid(), CancellationToken.None));
        }

        [Fact]
        public async Task Update_ShouldExcludeCurrentCourseFromDuplicateCheck()
        {
            var course = new Course("CS-101", "Computer Science", null, 3);
            _repository.Setup(repository => repository.GetByIdAsync(course.Id, It.IsAny<CancellationToken>())).ReturnsAsync(course);
            _repository.Setup(repository => repository.ExistsByCodeAsync("CS-101", course.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var result = await new UpdateCourseUseCase(_repository.Object).ExecuteAsync(course.Id,
                new CourseRequest { Code = "cs-101", Name = "Computer Science I", Credits = 4 }, CancellationToken.None);

            Assert.Equal("Computer Science I", result.Name);
            _repository.Verify(repository => repository.UpdateAsync(course, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrowConflict_WhenAnotherCourseUsesCode()
        {
            var course = new Course("CS-101", "Computer Science", null, 3);
            _repository.Setup(repository => repository.GetByIdAsync(course.Id, It.IsAny<CancellationToken>())).ReturnsAsync(course);
            _repository.Setup(repository => repository.ExistsByCodeAsync("MATH-1", course.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<ConflictException>(() => new UpdateCourseUseCase(_repository.Object).ExecuteAsync(course.Id,
                new CourseRequest { Code = "MATH-1", Name = "Math", Credits = 3 }, CancellationToken.None));

            _repository.Verify(repository => repository.UpdateAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Deactivate_ShouldSetInactiveAndSave()
        {
            var course = new Course("CS-101", "Computer Science", null, 3);
            _repository.Setup(repository => repository.GetByIdAsync(course.Id, It.IsAny<CancellationToken>())).ReturnsAsync(course);

            await new DeactivateCourseUseCase(_repository.Object).ExecuteAsync(course.Id, CancellationToken.None);

            Assert.False(course.IsActive);
            _repository.Verify(repository => repository.UpdateAsync(course, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
