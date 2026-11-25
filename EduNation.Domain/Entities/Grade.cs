namespace EduNation.Domain.Entities
{
    public class Grade
    {
        public Guid Id { get; private set; }
        public Guid EvaluationId { get; private set; }
        public Guid EnrollmentId { get; private set; }
        public decimal Score { get; private set; }
        public string? Comments { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset UpdatedAt { get; private set; }
        public Guid? InstitutionId { get; private set; }

        public Grade(Guid evaluationId, Guid enrollmentId, decimal score, string? comments)
        {
            if (evaluationId == Guid.Empty)
                throw new ArgumentException("Evaluation id cannot be empty.", nameof(evaluationId));

            if (enrollmentId == Guid.Empty)
                throw new ArgumentException("Enrollment id cannot be empty.", nameof(enrollmentId));

            if (score < 0)
                throw new ArgumentOutOfRangeException(nameof(score), "Score cannot be negative.");

            Id = Guid.NewGuid();
            EvaluationId = evaluationId;
            EnrollmentId = enrollmentId;
            Score = score;
            Comments = NormalizeComments(comments);
            CreatedAt = DateTimeOffset.UtcNow;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void Update(decimal score, string? comments)
        {
            if (score < 0)
                throw new ArgumentOutOfRangeException(nameof(score), "Score cannot be negative.");

            Score = score;
            Comments = NormalizeComments(comments);
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        private static string? NormalizeComments(string? comments)
        {
            var normalized = comments?.Trim();
            return string.IsNullOrEmpty(normalized) ? null : normalized;
        }

        private Grade()
        {
        }
    }
}
