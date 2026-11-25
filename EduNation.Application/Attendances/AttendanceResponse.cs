using EduNation.Domain.Entities;

namespace EduNation.Application.Attendances
{
    public class AttendanceResponse
    {
        public Guid Id { get; set; }
        public Guid EnrollmentId { get; set; }
        public DateOnly Date { get; set; }
        public AttendanceStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
