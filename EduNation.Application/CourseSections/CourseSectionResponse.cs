namespace EduNation.Application.CourseSections
{
    public class CourseSectionResponse
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public Guid AcademicPeriodId { get; set; }
        public Guid TeacherId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
