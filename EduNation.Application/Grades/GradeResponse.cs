namespace EduNation.Application.Grades
{
    public class GradeResponse
    {
        public Guid Id { get; set; }
        public Guid EvaluationId { get; set; }
        public Guid EnrollmentId { get; set; }
        public decimal Score { get; set; }
        public string? Comments { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
