using EduNation.Domain.Entities;

namespace EduNation.Application.Grades
{
    public sealed class GradeSearchCriteria
    {
        public Guid? EvaluationId { get; init; }
        public Guid? EnrollmentId { get; init; }
        public Guid? StudentId { get; init; }
        public Guid? CourseSectionId { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
