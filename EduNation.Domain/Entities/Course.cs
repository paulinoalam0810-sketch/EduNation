namespace EduNation.Domain.Entities
{
    public class Course
    {
        public Guid Id { get; private set; }
        public string Code { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public int Credits { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public bool IsActive { get; private set; }
        public Guid? InstitutionId { get; private set; }

        private Course()
        {
        }

        public Course(string code, string name, string? description, int credits)
        {
            ValidateName(name);
            ValidateDescription(description);
            ValidateCredits(credits);

            Id = Guid.NewGuid();
            Code = NormalizeCode(code);
            Name = name.Trim();
            Description = NormalizeDescription(description);
            Credits = credits;
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;
        }

        public void Update(string code, string name, string? description, int credits)
        {
            ValidateName(name);
            ValidateDescription(description);
            ValidateCredits(credits);

            Code = NormalizeCode(code);
            Name = name.Trim();
            Description = NormalizeDescription(description);
            Credits = credits;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        private static string NormalizeCode(string code)
        {
            ValidateCode(code);
            return code.Trim().ToUpperInvariant();
        }

        private static void ValidateCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Course code cannot be empty.", nameof(code));

            if (code.Trim().Length > 32)
                throw new ArgumentException("Course code cannot exceed 32 characters.", nameof(code));
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Course name cannot be empty.", nameof(name));

            if (name.Trim().Length > 150)
                throw new ArgumentException("Course name cannot exceed 150 characters.", nameof(name));
        }

        private static void ValidateDescription(string? description)
        {
            if (description?.Length > 1000)
                throw new ArgumentException("Course description cannot exceed 1000 characters.", nameof(description));
        }

        private static void ValidateCredits(int credits)
        {
            if (credits is < 1 or > 30)
                throw new ArgumentOutOfRangeException(nameof(credits), "Course credits must be between 1 and 30.");
        }

        private static string? NormalizeDescription(string? description)
        {
            var normalized = description?.Trim();
            return string.IsNullOrEmpty(normalized) ? null : normalized;
        }
    }
}
