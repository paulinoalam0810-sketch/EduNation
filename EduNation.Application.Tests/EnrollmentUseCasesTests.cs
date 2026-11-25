using Moq;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Enrollments;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class EnrollmentUseCasesTests
    {
        private readonly Mock<IEnrollmentRepository> _enrollmentRepository = new();
        private readonly Mock<IStudentRepository> _studentRepository = new();
        private readonly Mock<ICourseSectionRepository> _courseSectionRepository = new();

        private CreateEnrollmentUseCase CreateUseCase() =>
            new(_enrollmentRepository.Object, _studentRepository.Object, _courseSectionRepository.Object);

        [Fact]
        public async Task Create_ShouldPersistEnrollment_WhenReferencesAreValid()
        {
            var student = new Student("Alam", "Paulino", "alam@example.com", new DateOnly(2008, 10, 8));
            var courseSection = new CourseSection(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "SEC-1", "Morning", 30);

            _studentRepository.Setup(r => r.GetByIdAsync(student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(student);
            _courseSectionRepository.Setup(r => r.GetByIdAsync(courseSection.Id, It.IsAny<CancellationToken>())).ReturnsAsync(courseSection);
            _enrollmentRepository.Setup(r => r.AddAsync(It.IsAny<Enrollment>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Enrollment enrollment, CancellationToken _) => enrollment);

            var result = await CreateUseCase().ExecuteAsync(new EnrollmentRequest
            {
                StudentId = student.Id,
                CourseSectionId = courseSection.Id
            }, CancellationToken.None);

            Assert.Equal(student.Id, result.StudentId);
            Assert.Equal(courseSection.Id, result.CourseSectionId);
            Assert.Equal(EnrollmentStatus.Active, result.Status);
            _enrollmentRepository.Verify(r => r.AddAsync(It.IsAny<Enrollment>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldThrowNotFound_WhenStudentIsMissing()
        {
            var courseSection = new CourseSection(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "SEC-1", "Morning", 30);

            _studentRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Student?)null);
            _courseSectionRepository.Setup(r => r.GetByIdAsync(courseSection.Id, It.IsAny<CancellationToken>())).ReturnsAsync(courseSection);

            await Assert.ThrowsAsync<NotFoundException>(() => CreateUseCase().ExecuteAsync(new EnrollmentRequest
            {
                StudentId = Guid.NewGuid(),
                CourseSectionId = courseSection.Id
            }, CancellationToken.None));
        }

        [Fact]
        public async Task Create_ShouldThrowNotFound_WhenCourseSectionIsMissing()
        {
            var student = new Student("Alam", "Paulino", "alam@example.com", new DateOnly(2008, 10, 8));

            _studentRepository.Setup(r => r.GetByIdAsync(student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(student);
            _courseSectionRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((CourseSection?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => CreateUseCase().ExecuteAsync(new EnrollmentRequest
            {
                StudentId = student.Id,
                CourseSectionId = Guid.NewGuid()
            }, CancellationToken.None));
        }

        [Fact]
        public async Task Create_ShouldPropagateConflict_WhenRepositoryThrows()
        {
            var student = new Student("Alam", "Paulino", "alam@example.com", new DateOnly(2008, 10, 8));
            var courseSection = new CourseSection(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "SEC-1", "Morning", 30);

            _studentRepository.Setup(r => r.GetByIdAsync(student.Id, It.IsAny<CancellationToken>())).ReturnsAsync(student);
            _courseSectionRepository.Setup(r => r.GetByIdAsync(courseSection.Id, It.IsAny<CancellationToken>())).ReturnsAsync(courseSection);
            _enrollmentRepository.Setup(r => r.AddAsync(It.IsAny<Enrollment>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConflictException("An enrollment for this student and course section already exists."));

            await Assert.ThrowsAsync<ConflictException>(() => CreateUseCase().ExecuteAsync(new EnrollmentRequest
            {
                StudentId = student.Id,
                CourseSectionId = courseSection.Id
            }, CancellationToken.None));
        }

        [Fact]
        public async Task GetById_ShouldThrowNotFound_WhenMissing()
        {
            _enrollmentRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Enrollment?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => new GetEnrollmentByIdUseCase(_enrollmentRepository.Object)
                .ExecuteAsync(Guid.NewGuid(), CancellationToken.None));
        }

        [Fact]
        public async Task Cancel_ShouldThrowConflict_WhenAlreadyCancelled()
        {
            var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());
            enrollment.Cancel();
            _enrollmentRepository.Setup(r => r.GetByIdAsync(enrollment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);

            await Assert.ThrowsAsync<ConflictException>(() => new CancelEnrollmentUseCase(_enrollmentRepository.Object)
                .ExecuteAsync(enrollment.Id, CancellationToken.None));

            _enrollmentRepository.Verify(r => r.UpdateAsync(It.IsAny<Enrollment>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Cancel_ShouldSetCancelledAndSave()
        {
            var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());
            _enrollmentRepository.Setup(r => r.GetByIdAsync(enrollment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);

            await new CancelEnrollmentUseCase(_enrollmentRepository.Object).ExecuteAsync(enrollment.Id, CancellationToken.None);

            Assert.Equal(EnrollmentStatus.Cancelled, enrollment.Status);
            _enrollmentRepository.Verify(r => r.UpdateAsync(enrollment, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
