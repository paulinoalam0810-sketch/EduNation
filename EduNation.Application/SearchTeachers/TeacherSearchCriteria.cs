namespace EduNation.Application.SearchTeachers
{
    public sealed class TeacherSearchCriteria
    {
        public string? Search { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
