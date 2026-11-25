namespace Students.Application.SearchStudents
{
    public sealed class StudentSearchCriteria
    {
        public string? Search { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
