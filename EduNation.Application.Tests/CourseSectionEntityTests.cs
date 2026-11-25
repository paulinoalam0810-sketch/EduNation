using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class CourseSectionEntityTests
    {
        [Fact]
        public void Constructor_ShouldNormalizeAndSetDefaults()
        {
            var courseSection = new CourseSection(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "  sec-1  ", "  Morning  ", 30);

            Assert.Equal("SEC-1", courseSection.Code);
            Assert.Equal("Morning", courseSection.Name);
            Assert.Equal(30, courseSection.Capacity);
            Assert.True(courseSection.IsActive);
            Assert.NotEqual(Guid.Empty, courseSection.Id);
        }

        [Fact]
        public void Constructor_ShouldAllowOptionalName()
        {
            var courseSection = new CourseSection(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "SEC-1", "   ", 30);

            Assert.Null(courseSection.Name);
        }

        [Theory]
        [InlineData("", "Course section code cannot be empty.")]
        [InlineData("   ", "Course section code cannot be empty.")]
        public void Constructor_ShouldRejectEmptyCode(string code, string expectedMessage)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new CourseSection(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), code, null, 10));
            Assert.Equal(expectedMessage, exception.Message.Split(" (Parameter")[0]);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_ShouldRejectInvalidCapacity(int capacity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new CourseSection(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "SEC-1", null, capacity));
        }

        [Fact]
        public void Update_ShouldReplaceFields()
        {
            var courseSection = new CourseSection(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "SEC-1", "Morning", 30);
            var createdAt = courseSection.CreatedAt;

            var newCourseId = Guid.NewGuid();
            var newPeriodId = Guid.NewGuid();
            var newTeacherId = Guid.NewGuid();
            courseSection.Update(newCourseId, newPeriodId, newTeacherId, "sec-2", "Evening", 40);
            courseSection.Deactivate();

            Assert.Equal(newCourseId, courseSection.CourseId);
            Assert.Equal(newPeriodId, courseSection.AcademicPeriodId);
            Assert.Equal(newTeacherId, courseSection.TeacherId);
            Assert.Equal("SEC-2", courseSection.Code);
            Assert.Equal("Evening", courseSection.Name);
            Assert.Equal(40, courseSection.Capacity);
            Assert.Equal(createdAt, courseSection.CreatedAt);
            Assert.False(courseSection.IsActive);
        }
    }
}
