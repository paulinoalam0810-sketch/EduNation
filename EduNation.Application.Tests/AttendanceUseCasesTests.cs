using Moq;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Attendances;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class AttendanceUseCasesTests
    {
        private readonly Mock<IAttendanceRepository> _attendanceRepository = new();
        private readonly Mock<IEnrollmentRepository> _enrollmentRepository = new();

        [Fact]
        public async Task Create_ShouldPersistAttendance_WhenEnrollmentIsActive()
        {
            var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());
            _enrollmentRepository.Setup(r => r.GetByIdAsync(enrollment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);
            _attendanceRepository.Setup(r => r.AddAsync(It.IsAny<Attendance>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Attendance attendance, CancellationToken _) => attendance);

            var result = await new CreateAttendanceUseCase(_attendanceRepository.Object, _enrollmentRepository.Object).ExecuteAsync(
                new AttendanceRequest { EnrollmentId = enrollment.Id, Date = new DateOnly(2025, 9, 1), Status = AttendanceStatus.Present },
                CancellationToken.None);

            Assert.Equal(enrollment.Id, result.EnrollmentId);
            Assert.Equal(AttendanceStatus.Present, result.Status);
            _attendanceRepository.Verify(r => r.AddAsync(It.IsAny<Attendance>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldThrowNotFound_WhenEnrollmentIsMissing()
        {
            _enrollmentRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Enrollment?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => new CreateAttendanceUseCase(_attendanceRepository.Object, _enrollmentRepository.Object).ExecuteAsync(
                new AttendanceRequest { EnrollmentId = Guid.NewGuid(), Date = new DateOnly(2025, 9, 1), Status = AttendanceStatus.Present },
                CancellationToken.None));
        }

        [Fact]
        public async Task Create_ShouldThrowNotFound_WhenEnrollmentIsCancelled()
        {
            var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());
            enrollment.Cancel();
            _enrollmentRepository.Setup(r => r.GetByIdAsync(enrollment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);

            await Assert.ThrowsAsync<NotFoundException>(() => new CreateAttendanceUseCase(_attendanceRepository.Object, _enrollmentRepository.Object).ExecuteAsync(
                new AttendanceRequest { EnrollmentId = enrollment.Id, Date = new DateOnly(2025, 9, 1), Status = AttendanceStatus.Present },
                CancellationToken.None));
        }

        [Fact]
        public async Task Create_ShouldPropagateConflict_WhenDuplicateExists()
        {
            var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());
            _enrollmentRepository.Setup(r => r.GetByIdAsync(enrollment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);
            _attendanceRepository.Setup(r => r.AddAsync(It.IsAny<Attendance>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConflictException("An attendance record for this enrollment and date already exists."));

            await Assert.ThrowsAsync<ConflictException>(() => new CreateAttendanceUseCase(_attendanceRepository.Object, _enrollmentRepository.Object).ExecuteAsync(
                new AttendanceRequest { EnrollmentId = enrollment.Id, Date = new DateOnly(2025, 9, 1), Status = AttendanceStatus.Present },
                CancellationToken.None));
        }

        [Fact]
        public async Task GetById_ShouldThrowNotFound_WhenMissing()
        {
            _attendanceRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Attendance?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => new GetAttendanceByIdUseCase(_attendanceRepository.Object)
                .ExecuteAsync(Guid.NewGuid(), CancellationToken.None));
        }

        [Fact]
        public async Task Update_ShouldChangeStatusAndSave()
        {
            var attendance = new Attendance(Guid.NewGuid(), new DateOnly(2025, 9, 1), AttendanceStatus.Present);
            _attendanceRepository.Setup(r => r.GetByIdAsync(attendance.Id, It.IsAny<CancellationToken>())).ReturnsAsync(attendance);

            var result = await new UpdateAttendanceUseCase(_attendanceRepository.Object).ExecuteAsync(attendance.Id, AttendanceStatus.Excused, CancellationToken.None);

            Assert.Equal(AttendanceStatus.Excused, result.Status);
            _attendanceRepository.Verify(r => r.UpdateAsync(attendance, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
