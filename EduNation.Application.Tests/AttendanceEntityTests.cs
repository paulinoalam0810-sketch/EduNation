using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class AttendanceEntityTests
    {
        [Fact]
        public void Constructor_ShouldSetDefaults()
        {
            var enrollmentId = Guid.NewGuid();
            var date = new DateOnly(2025, 9, 1);

            var attendance = new Attendance(enrollmentId, date, AttendanceStatus.Present);

            Assert.Equal(enrollmentId, attendance.EnrollmentId);
            Assert.Equal(date, attendance.Date);
            Assert.Equal(AttendanceStatus.Present, attendance.Status);
            Assert.NotEqual(Guid.Empty, attendance.Id);
            Assert.True(attendance.CreatedAt <= DateTimeOffset.UtcNow);
        }

        [Theory]
        [InlineData(true)]
        public void Constructor_ShouldRejectEmptyEnrollmentId(bool _)
        {
            Assert.Throws<ArgumentException>(() =>
                new Attendance(Guid.Empty, new DateOnly(2025, 9, 1), AttendanceStatus.Present));
        }

        [Fact]
        public void Constructor_ShouldRejectEmptyDate()
        {
            Assert.Throws<ArgumentException>(() =>
                new Attendance(Guid.NewGuid(), DateOnly.MinValue, AttendanceStatus.Present));
        }

        [Fact]
        public void UpdateStatus_ShouldChangeStatus()
        {
            var attendance = new Attendance(Guid.NewGuid(), new DateOnly(2025, 9, 1), AttendanceStatus.Present);

            attendance.UpdateStatus(AttendanceStatus.Late);

            Assert.Equal(AttendanceStatus.Late, attendance.Status);
        }
    }
}
