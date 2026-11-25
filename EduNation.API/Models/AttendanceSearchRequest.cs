using System.ComponentModel.DataAnnotations;
using EduNation.Domain.Entities;

namespace EduNation.API.Models
{
    public sealed class AttendanceSearchRequest
    {
        public Guid? EnrollmentId { get; init; }
        public Guid? StudentId { get; init; }
        public Guid? CourseSectionId { get; init; }
        public DateOnly? Date { get; init; }
        public AttendanceStatus? Status { get; init; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; init; } = 1;

        [Range(1, 100)]
        public int PageSize { get; init; } = 10;
    }
}
