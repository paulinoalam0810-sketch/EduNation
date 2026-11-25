using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class AcademicPeriodEntityTests
    {
        [Fact]
        public void Constructor_ShouldNormalizeValuesAndSetDefaults()
        {
            var academicPeriod = new AcademicPeriod("  Fall 2025  ", "  fall-2025 ", new DateOnly(2025, 9, 1), new DateOnly(2025, 12, 15));

            Assert.Equal("Fall 2025", academicPeriod.Name);
            Assert.Equal("FALL-2025", academicPeriod.Code);
            Assert.Equal(new DateOnly(2025, 9, 1), academicPeriod.StartDate);
            Assert.Equal(new DateOnly(2025, 12, 15), academicPeriod.EndDate);
            Assert.True(academicPeriod.IsActive);
            Assert.NotEqual(Guid.Empty, academicPeriod.Id);
        }

        [Theory]
        [InlineData("", "Academic period name cannot be empty.")]
        [InlineData("   ", "Academic period name cannot be empty.")]
        public void Constructor_ShouldRejectEmptyName(string name, string expectedMessage)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new AcademicPeriod(name, "FALL-2025", new DateOnly(2025, 9, 1), new DateOnly(2025, 12, 15)));
            Assert.Equal(expectedMessage, exception.Message.Split(" (Parameter")[0]);
        }

        [Theory]
        [InlineData("", "Academic period code cannot be empty.")]
        [InlineData("   ", "Academic period code cannot be empty.")]
        public void Constructor_ShouldRejectEmptyCode(string code, string expectedMessage)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new AcademicPeriod("Fall 2025", code, new DateOnly(2025, 9, 1), new DateOnly(2025, 12, 15)));
            Assert.Equal(expectedMessage, exception.Message.Split(" (Parameter")[0]);
        }

        [Theory]
        [InlineData(2025, 9, 1, 2025, 9, 1)]
        [InlineData(2025, 10, 1, 2025, 9, 1)]
        public void Constructor_ShouldRejectInvalidDateRange(int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay)
        {
            Assert.Throws<ArgumentException>(() =>
                new AcademicPeriod("Fall 2025", "FALL-2025", new DateOnly(startYear, startMonth, startDay), new DateOnly(endYear, endMonth, endDay)));
        }

        [Fact]
        public void Update_ShouldReplaceFieldsAndDeactivateShouldPreserveEntity()
        {
            var academicPeriod = new AcademicPeriod("Fall 2025", "FALL-2025", new DateOnly(2025, 9, 1), new DateOnly(2025, 12, 15));
            var createdAt = academicPeriod.CreatedAt;

            academicPeriod.Update("Spring 2026", "spring-2026", new DateOnly(2026, 1, 10), new DateOnly(2026, 5, 20));
            academicPeriod.Deactivate();

            Assert.Equal("Spring 2026", academicPeriod.Name);
            Assert.Equal("SPRING-2026", academicPeriod.Code);
            Assert.Equal(new DateOnly(2026, 1, 10), academicPeriod.StartDate);
            Assert.Equal(new DateOnly(2026, 5, 20), academicPeriod.EndDate);
            Assert.Equal(createdAt, academicPeriod.CreatedAt);
            Assert.False(academicPeriod.IsActive);
        }
    }
}
