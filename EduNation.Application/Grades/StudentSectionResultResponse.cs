namespace EduNation.Application.Grades
{
    public class StudentSectionResultResponse
    {
        public Guid StudentId { get; set; }
        public Guid CourseSectionId { get; set; }
        public decimal? WeightedScore { get; set; }
        public decimal TotalWeight { get; set; }
        public int GradesCount { get; set; }
    }
}
