namespace EduNation.Domain.Entities
{
    public class Attendance
    {
        public Guid Id { get; private set; }
        public Guid EnrollmentId { get; private set; }
        public DateOnly Date { get; private set; }
        public AttendanceStatus Status { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public Guid? InstitutionId { get; private set; }

        public Attendance(Guid enrollmentId, DateOnly date, AttendanceStatus status)
        {
            if (enrollmentId == Guid.Empty)
                throw new ArgumentException("Enrollment id cannot be empty.", nameof(enrollmentId));

            if (date == DateOnly.MinValue)
                throw new ArgumentException("Attendance date cannot be empty.", nameof(date));

            Id = Guid.NewGuid();
            EnrollmentId = enrollmentId;
            Date = date;
            Status = status;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public void UpdateStatus(AttendanceStatus status)
        {
            Status = status;
        }

        private Attendance()
        {
        }
    }
}
