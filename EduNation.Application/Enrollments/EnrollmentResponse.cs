using EduNation.Domain.Entities;

namespace EduNation.Application.Enrollments
{
    public class EnrollmentResponse
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid? CourseSectionId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
