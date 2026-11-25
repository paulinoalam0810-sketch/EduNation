namespace EduNation.Application.Grades
{
    public class GradeRequest
    {
        public Guid EvaluationId { get; set; }
        public Guid EnrollmentId { get; set; }
        public decimal Score { get; set; }
        public string? Comments { get; set; }
    }
}
