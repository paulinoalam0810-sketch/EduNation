using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class CourseTests
    {
        [Fact]
        public void Constructor_ShouldNormalizeValuesAndSetDefaults()
        {
            var course = new Course("  cs-101 ", "  Intro to CS  ", "  basics  ", 3);

            Assert.Equal("CS-101", course.Code);
            Assert.Equal("Intro to CS", course.Name);
            Assert.Equal("basics", course.Description);
            Assert.Equal(3, course.Credits);
            Assert.True(course.IsActive);
            Assert.NotEqual(Guid.Empty, course.Id);
        }

        [Theory]
        [InlineData("", "Course code cannot be empty.")]
        [InlineData("   ", "Course code cannot be empty.")]
        public void Constructor_ShouldRejectEmptyCode(string code, string expectedMessage)
        {
            var exception = Assert.Throws<ArgumentException>(() => new Course(code, "Math", null, 3));
            Assert.Equal(expectedMessage, exception.Message.Split(" (Parameter")[0]);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(31)]
        public void Constructor_ShouldRejectCreditsOutsideSupportedRange(int credits)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Course("MATH-1", "Math", null, credits));
        }

        [Fact]
        public void Update_ShouldReplaceFieldsAndDeactivateShouldPreserveEntity()
        {
            var course = new Course("MATH-1", "Math", null, 3);
            var createdAt = course.CreatedAt;

            course.Update("bio-2", "Biology", "Life sciences", 4);
            course.Deactivate();

            Assert.Equal("BIO-2", course.Code);
            Assert.Equal("Biology", course.Name);
            Assert.Equal("Life sciences", course.Description);
            Assert.Equal(4, course.Credits);
            Assert.Equal(createdAt, course.CreatedAt);
            Assert.False(course.IsActive);
        }
    }
}
