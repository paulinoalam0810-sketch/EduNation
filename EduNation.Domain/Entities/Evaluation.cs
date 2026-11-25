namespace EduNation.Domain.Entities
{
    public class Evaluation
    {
        public Guid Id { get; private set; }
        public Guid CourseSectionId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public decimal MaxScore { get; private set; }
        public decimal Weight { get; private set; }
        public DateOnly EvaluationDate { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public bool IsActive { get; private set; }
        public Guid? InstitutionId { get; private set; }

        public Evaluation(Guid courseSectionId, string name, string? description, decimal maxScore, decimal weight, DateOnly evaluationDate)
        {
            if (courseSectionId == Guid.Empty)
                throw new ArgumentException("Course section id cannot be empty.", nameof(courseSectionId));

            ValidateName(name);
            ValidateMaxScore(maxScore);
            ValidateWeight(weight);
            if (evaluationDate == DateOnly.MinValue)
                throw new ArgumentException("Evaluation date cannot be empty.", nameof(evaluationDate));

            Id = Guid.NewGuid();
            CourseSectionId = courseSectionId;
            Name = name.Trim();
            Description = NormalizeDescription(description);
            MaxScore = maxScore;
            Weight = weight;
            EvaluationDate = evaluationDate;
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;
        }

        public void Update(string name, string? description, decimal maxScore, decimal weight, DateOnly evaluationDate)
        {
            ValidateName(name);
            ValidateMaxScore(maxScore);
            ValidateWeight(weight);
            if (evaluationDate == DateOnly.MinValue)
                throw new ArgumentException("Evaluation date cannot be empty.", nameof(evaluationDate));

            Name = name.Trim();
            Description = NormalizeDescription(description);
            MaxScore = maxScore;
            Weight = weight;
            EvaluationDate = evaluationDate;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Evaluation name cannot be empty.", nameof(name));

            if (name.Trim().Length > 150)
                throw new ArgumentException("Evaluation name cannot exceed 150 characters.", nameof(name));
        }

        private static void ValidateMaxScore(decimal maxScore)
        {
            if (maxScore <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxScore), "Max score must be greater than zero.");
        }

        private static void ValidateWeight(decimal weight)
        {
            if (weight <= 0 || weight > 100)
                throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be greater than zero and not exceed 100.");
        }

        private static string? NormalizeDescription(string? description)
        {
            var normalized = description?.Trim();
            return string.IsNullOrEmpty(normalized) ? null : normalized;
        }

        private Evaluation()
        {
        }
    }
}
