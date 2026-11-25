using System.ComponentModel.DataAnnotations;

namespace EduNation.API.Models
{
    public sealed class GradeSearchRequest
    {
        public Guid? EvaluationId { get; init; }
        public Guid? EnrollmentId { get; init; }
        public Guid? StudentId { get; init; }
        public Guid? CourseSectionId { get; init; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; init; } = 1;

        [Range(1, 100)]
        public int PageSize { get; init; } = 10;
    }
}
