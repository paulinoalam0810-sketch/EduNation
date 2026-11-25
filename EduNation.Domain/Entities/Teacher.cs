using System.Text.RegularExpressions;

namespace EduNation.Domain.Entities
{
    public class Teacher
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string? PhoneNumber { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public bool IsActive { get; private set; }
        public Guid? InstitutionId { get; private set; }

        private Teacher()
        {
        }

        public Teacher(string firstName, string lastName, string email, string? phoneNumber = null)
        {
            ValidateFirstName(firstName);
            ValidateLastName(lastName);
            ValidateEmail(email);
            ValidatePhoneNumber(phoneNumber);

            Id = Guid.NewGuid();
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email.Trim().ToLowerInvariant();
            PhoneNumber = NormalizePhoneNumber(phoneNumber);
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;
        }

        public void UpdatePersonalInfo(string firstName, string lastName, string email, string? phoneNumber = null)
        {
            ValidateFirstName(firstName);
            ValidateLastName(lastName);
            ValidateEmail(email);
            ValidatePhoneNumber(phoneNumber);

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email.Trim().ToLowerInvariant();
            PhoneNumber = NormalizePhoneNumber(phoneNumber);
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        private static string? NormalizePhoneNumber(string? phoneNumber)
        {
            var normalized = phoneNumber?.Trim();
            return string.IsNullOrEmpty(normalized) ? null : normalized;
        }

        private static void ValidateFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        }

        private static void ValidateLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
        }

        private static void ValidateEmail(string email)
        {
            var trimmed = email?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(trimmed, emailPattern))
                throw new ArgumentException("Invalid email format.", nameof(email));
        }

        private static void ValidatePhoneNumber(string? phoneNumber)
        {
            if (phoneNumber is not null && phoneNumber.Trim().Length > 32)
                throw new ArgumentException("Phone number cannot exceed 32 characters.", nameof(phoneNumber));
        }
    }
}
