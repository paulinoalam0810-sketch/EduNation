namespace EduNation.Domain.Entities
{
    public class CourseSection
    {
        public Guid Id { get; private set; }
        public Guid CourseId { get; private set; }
        public Guid AcademicPeriodId { get; private set; }
        public Guid TeacherId { get; private set; }
        public string Code { get; private set; } = string.Empty;
        public string? Name { get; private set; }
        public int Capacity { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public bool IsActive { get; private set; }
        public Guid? InstitutionId { get; private set; }

        private CourseSection()
        {
        }

        public CourseSection(Guid courseId, Guid academicPeriodId, Guid teacherId, string code, string? name, int capacity)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("Course id cannot be empty.", nameof(courseId));

            if (academicPeriodId == Guid.Empty)
                throw new ArgumentException("Academic period id cannot be empty.", nameof(academicPeriodId));

            if (teacherId == Guid.Empty)
                throw new ArgumentException("Teacher id cannot be empty.", nameof(teacherId));

            ValidateCode(code);
            ValidateName(name);
            ValidateCapacity(capacity);

            Id = Guid.NewGuid();
            CourseId = courseId;
            AcademicPeriodId = academicPeriodId;
            TeacherId = teacherId;
            Code = code.Trim().ToUpperInvariant();
            Name = NormalizeName(name);
            Capacity = capacity;
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;
        }

        public void Update(Guid courseId, Guid academicPeriodId, Guid teacherId, string code, string? name, int capacity)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("Course id cannot be empty.", nameof(courseId));

            if (academicPeriodId == Guid.Empty)
                throw new ArgumentException("Academic period id cannot be empty.", nameof(academicPeriodId));

            if (teacherId == Guid.Empty)
                throw new ArgumentException("Teacher id cannot be empty.", nameof(teacherId));

            ValidateCode(code);
            ValidateName(name);
            ValidateCapacity(capacity);

            CourseId = courseId;
            AcademicPeriodId = academicPeriodId;
            TeacherId = teacherId;
            Code = code.Trim().ToUpperInvariant();
            Name = NormalizeName(name);
            Capacity = capacity;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        private static void ValidateCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Course section code cannot be empty.", nameof(code));

            if (code.Trim().Length > 32)
                throw new ArgumentException("Course section code cannot exceed 32 characters.", nameof(code));
        }

        private static void ValidateName(string? name)
        {
            if (name is not null && name.Trim().Length > 150)
                throw new ArgumentException("Course section name cannot exceed 150 characters.", nameof(name));
        }

        private static void ValidateCapacity(int capacity)
        {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Course section capacity must be at least 1.");
        }

        private static string? NormalizeName(string? name)
        {
            var normalized = name?.Trim();
            return string.IsNullOrEmpty(normalized) ? null : normalized;
        }
    }
}
