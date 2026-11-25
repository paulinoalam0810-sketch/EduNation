namespace EduNation.Application.CourseSections
{
    public class CourseSectionRequest
    {
        public Guid CourseId { get; set; }
        public Guid AcademicPeriodId { get; set; }
        public Guid TeacherId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Name { get; set; }
        public int Capacity { get; set; }
    }
}
