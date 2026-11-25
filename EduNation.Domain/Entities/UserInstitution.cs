namespace EduNation.Domain.Entities
{
    public class UserInstitution
    {
        public string UserId { get; private set; } = string.Empty;
        public Guid InstitutionId { get; private set; }
        public string Role { get; private set; } = "Student";
        public DateTimeOffset CreatedAt { get; private set; }

        public UserInstitution(string userId, Guid institutionId, string role = "Student")
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User id cannot be empty.", nameof(userId));

            if (institutionId == Guid.Empty)
                throw new ArgumentException("Institution id cannot be empty.", nameof(institutionId));

            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role cannot be empty.", nameof(role));

            UserId = userId;
            InstitutionId = institutionId;
            Role = role.Trim();
            CreatedAt = DateTimeOffset.UtcNow;
        }

        private UserInstitution()
        {
        }
    }
}
