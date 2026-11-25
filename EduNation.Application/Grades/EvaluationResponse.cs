using EduNation.Domain.Entities;

namespace EduNation.Application.Grades
{
    public class EvaluationResponse
    {
        public Guid Id { get; set; }
        public Guid CourseSectionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal MaxScore { get; set; }
        public decimal Weight { get; set; }
        public DateOnly EvaluationDate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
