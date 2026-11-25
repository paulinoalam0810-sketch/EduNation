namespace EduNation.Domain.Entities
{
    public class Enrollment
    {
        public Guid Id { get; private set; }
        public Guid StudentId { get; private set; }
        public Guid? CourseSectionId { get; private set; }
        public Guid? CourseId { get; private set; }
        public Guid? AcademicPeriodId { get; private set; }
        public DateTime EnrolledAt { get; private set; }
        public EnrollmentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid? InstitutionId { get; private set; }

        private Enrollment()
        {
        }

        public Enrollment(Guid studentId, Guid courseSectionId)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("Student id cannot be empty.", nameof(studentId));

            if (courseSectionId == Guid.Empty)
                throw new ArgumentException("Course section id cannot be empty.", nameof(courseSectionId));

            Id = Guid.NewGuid();
            StudentId = studentId;
            CourseSectionId = courseSectionId;
            EnrolledAt = DateTime.UtcNow;
            Status = EnrollmentStatus.Active;
            CreatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == EnrollmentStatus.Cancelled)
                throw new InvalidOperationException("Enrollment is already cancelled.");

            Status = EnrollmentStatus.Cancelled;
        }
    }
}
