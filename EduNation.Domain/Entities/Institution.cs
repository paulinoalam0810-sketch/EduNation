namespace EduNation.Domain.Entities
{
    public class Institution
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Code { get; private set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; private set; }
        public bool IsActive { get; private set; }

        public Institution(string name, string code)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Institution name cannot be empty.", nameof(name));

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Institution code cannot be empty.", nameof(code));

            Id = Guid.NewGuid();
            Name = name.Trim();
            Code = code.Trim().ToUpperInvariant();
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        private Institution()
        {
        }
    }
}
