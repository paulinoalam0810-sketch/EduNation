using System.ComponentModel.DataAnnotations;

namespace Students.API.Models
{
    public sealed class StudentSearchRequest
    {
        public string? Search { get; init; }

        [Range(1, int.MaxValue)]
        public int PageNumber { get; init; } = 1;

        [Range(1, 100)]
        public int PageSize { get; init; } = 10;
    }
}
