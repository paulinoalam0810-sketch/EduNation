using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class EnrollmentEntityTests
    {
        [Fact]
        public void Constructor_ShouldSetDefaults()
        {
            var studentId = Guid.NewGuid();
            var courseSectionId = Guid.NewGuid();

            var enrollment = new Enrollment(studentId, courseSectionId);

            Assert.Equal(studentId, enrollment.StudentId);
            Assert.Equal(courseSectionId, enrollment.CourseSectionId);
            Assert.Equal(EnrollmentStatus.Active, enrollment.Status);
            Assert.NotEqual(Guid.Empty, enrollment.Id);
            Assert.True(enrollment.EnrolledAt <= DateTime.UtcNow);
            Assert.True(enrollment.CreatedAt <= DateTime.UtcNow);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void Constructor_ShouldRejectEmptyIds(bool emptyStudent, bool emptySection)
        {
            var studentId = emptyStudent ? Guid.Empty : Guid.NewGuid();
            var courseSectionId = emptySection ? Guid.Empty : Guid.NewGuid();

            Assert.Throws<ArgumentException>(() => new Enrollment(studentId, courseSectionId));
        }

        [Fact]
        public void Cancel_ShouldSetStatusToCancelled()
        {
            var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());

            enrollment.Cancel();

            Assert.Equal(EnrollmentStatus.Cancelled, enrollment.Status);
        }

        [Fact]
        public void Cancel_ShouldThrow_WhenAlreadyCancelled()
        {
            var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());
            enrollment.Cancel();

            Assert.Throws<InvalidOperationException>(() => enrollment.Cancel());
        }
    }
}
