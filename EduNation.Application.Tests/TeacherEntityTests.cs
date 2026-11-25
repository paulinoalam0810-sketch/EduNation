using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class TeacherEntityTests
    {
        [Fact]
        public void Constructor_ShouldNormalizeValuesAndSetDefaults()
        {
            var teacher = new Teacher("  Alam  ", "  Paulino  ", "  Alam@Example.com  ", "  12345  ");

            Assert.Equal("Alam", teacher.FirstName);
            Assert.Equal("Paulino", teacher.LastName);
            Assert.Equal("alam@example.com", teacher.Email);
            Assert.Equal("12345", teacher.PhoneNumber);
            Assert.True(teacher.IsActive);
            Assert.NotEqual(Guid.Empty, teacher.Id);
        }

        [Fact]
        public void Constructor_ShouldAllowOptionalPhoneNumber()
        {
            var teacher = new Teacher("Alam", "Paulino", "alam@example.com");

            Assert.Null(teacher.PhoneNumber);
        }

        [Theory]
        [InlineData("", "First name cannot be empty.")]
        [InlineData("   ", "First name cannot be empty.")]
        public void Constructor_ShouldRejectEmptyFirstName(string firstName, string expectedMessage)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new Teacher(firstName, "Paulino", "alam@example.com"));
            Assert.Equal(expectedMessage, exception.Message.Split(" (Parameter")[0]);
        }

        [Theory]
        [InlineData("", "Last name cannot be empty.")]
        [InlineData("   ", "Last name cannot be empty.")]
        public void Constructor_ShouldRejectEmptyLastName(string lastName, string expectedMessage)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new Teacher("Alam", lastName, "alam@example.com"));
            Assert.Equal(expectedMessage, exception.Message.Split(" (Parameter")[0]);
        }

        [Theory]
        [InlineData("", "Email cannot be empty.")]
        [InlineData("   ", "Email cannot be empty.")]
        [InlineData("invalid", "Invalid email format.")]
        public void Constructor_ShouldRejectInvalidEmail(string email, string expectedMessage)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new Teacher("Alam", "Paulino", email));
            Assert.Equal(expectedMessage, exception.Message.Split(" (Parameter")[0]);
        }

        [Fact]
        public void Update_ShouldReplaceFields()
        {
            var teacher = new Teacher("Alam", "Paulino", "alam@example.com", "123");
            var createdAt = teacher.CreatedAt;

            teacher.UpdatePersonalInfo("Carlos", "Martinez", "carlos@example.com", "999");
            teacher.Deactivate();

            Assert.Equal("Carlos", teacher.FirstName);
            Assert.Equal("Martinez", teacher.LastName);
            Assert.Equal("carlos@example.com", teacher.Email);
            Assert.Equal("999", teacher.PhoneNumber);
            Assert.Equal(createdAt, teacher.CreatedAt);
            Assert.False(teacher.IsActive);
        }
    }
}
