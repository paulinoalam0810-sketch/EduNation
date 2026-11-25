namespace EduNation.Domain.Entities
{
    public class AcademicPeriod
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Code { get; private set; } = string.Empty;
        public DateOnly StartDate { get; private set; }
        public DateOnly EndDate { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public bool IsActive { get; private set; }
        public Guid? InstitutionId { get; private set; }

        private AcademicPeriod()
        {
        }

        public AcademicPeriod(string name, string code, DateOnly startDate, DateOnly endDate)
        {
            ValidateName(name);
            ValidateDateRange(startDate, endDate);

            Id = Guid.NewGuid();
            Name = name.Trim();
            Code = NormalizeCode(code);
            StartDate = startDate;
            EndDate = endDate;
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;
        }

        public void Update(string name, string code, DateOnly startDate, DateOnly endDate)
        {
            ValidateName(name);
            ValidateDateRange(startDate, endDate);

            Name = name.Trim();
            Code = NormalizeCode(code);
            StartDate = startDate;
            EndDate = endDate;
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
                throw new ArgumentException("Academic period code cannot be empty.", nameof(code));

            if (code.Trim().Length > 32)
                throw new ArgumentException("Academic period code cannot exceed 32 characters.", nameof(code));
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Academic period name cannot be empty.", nameof(name));

            if (name.Trim().Length > 150)
                throw new ArgumentException("Academic period name cannot exceed 150 characters.", nameof(name));
        }

        private static void ValidateDateRange(DateOnly startDate, DateOnly endDate)
        {
            if (startDate == DateOnly.MinValue)
                throw new ArgumentException("Start date cannot be empty.", nameof(startDate));

            if (endDate == DateOnly.MinValue)
                throw new ArgumentException("End date cannot be empty.", nameof(endDate));

            if (startDate >= endDate)
                throw new ArgumentException("Start date must be before end date.", nameof(startDate));
        }
    }
}
