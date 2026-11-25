namespace EduNation.Application.Responses
{
    public class StudentResponse
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public DateOnly DateOfBirth { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public bool IsActive { get; init; }
    }
}
