using EduNation.Domain.Entities;

namespace EduNation.Application.SearchAttendances
{
    public sealed class AttendanceSearchCriteria
    {
        public Guid? EnrollmentId { get; init; }
        public Guid? StudentId { get; init; }
        public Guid? CourseSectionId { get; init; }
        public DateOnly? Date { get; init; }
        public AttendanceStatus? Status { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
