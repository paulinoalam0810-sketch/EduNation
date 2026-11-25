using EduNation.Domain.Entities;

namespace EduNation.Application.Attendances
{
    public class AttendanceRequest
    {
        public Guid EnrollmentId { get; set; }
        public DateOnly Date { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}
