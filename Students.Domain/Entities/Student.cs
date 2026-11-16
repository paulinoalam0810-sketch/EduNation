using System.Text.RegularExpressions;

namespace Students.Domain.Entities
{
    public class Student
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public DateOnly DateOfBirth { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public bool IsActive { get; private set; }

        public Student(string firstName, string lastName, string email, DateOnly dateOfBirth)
        {
            ValidateFirstName(firstName);
            ValidateLastName(lastName);
            ValidateEmail(email);
            ValidateDateOfBirth(dateOfBirth);

            Id = Guid.NewGuid();
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email.Trim().ToLowerInvariant();
            DateOfBirth = dateOfBirth;
            CreatedAt = DateTimeOffset.UtcNow;
            IsActive = true;
        }

        public void UpdatePersonalInfo(string firstName, string lastName, string email, DateOnly dateOfBirth)
        {
            ValidateFirstName(firstName);
            ValidateLastName(lastName);
            ValidateEmail(email);
            ValidateDateOfBirth(dateOfBirth);

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Email = email.Trim().ToLowerInvariant();
            DateOfBirth = dateOfBirth;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        private void ValidateFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        }

        private void ValidateLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty.", nameof(lastName));
        }

        private void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(email, emailPattern))
                throw new ArgumentException("Invalid email format.", nameof(email));
        }

        private void ValidateDateOfBirth(DateOnly dateOfBirth)
        {
            if (dateOfBirth == DateOnly.MinValue)
                throw new ArgumentException("Date of birth cannot be empty.", nameof(dateOfBirth));

            if (dateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ArgumentException("Date of birth cannot be in the future.", nameof(dateOfBirth));
        }

    }
}
