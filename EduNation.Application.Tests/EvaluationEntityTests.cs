using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class EvaluationEntityTests
    {
        [Fact]
        public void Constructor_ShouldSetDefaults()
        {
            var courseSectionId = Guid.NewGuid();

            var evaluation = new Evaluation(courseSectionId, "Midterm", "Desc", 100, 30, new DateOnly(2025, 10, 1));

            Assert.Equal(courseSectionId, evaluation.CourseSectionId);
            Assert.Equal("Midterm", evaluation.Name);
            Assert.Equal("Desc", evaluation.Description);
            Assert.Equal(100, evaluation.MaxScore);
            Assert.Equal(30, evaluation.Weight);
            Assert.True(evaluation.IsActive);
            Assert.NotEqual(Guid.Empty, evaluation.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Constructor_ShouldRejectInvalidMaxScore(decimal maxScore)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Evaluation(Guid.NewGuid(), "Midterm", null, maxScore, 30, new DateOnly(2025, 10, 1)));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public void Constructor_ShouldRejectInvalidWeight(decimal weight)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Evaluation(Guid.NewGuid(), "Midterm", null, 100, weight, new DateOnly(2025, 10, 1)));
        }

        [Fact]
        public void Constructor_ShouldRejectEmptyName()
        {
            Assert.Throws<ArgumentException>(() =>
                new Evaluation(Guid.NewGuid(), "", null, 100, 30, new DateOnly(2025, 10, 1)));
        }

        [Fact]
        public void Deactivate_ShouldSetInactive()
        {
            var evaluation = new Evaluation(Guid.NewGuid(), "Midterm", null, 100, 30, new DateOnly(2025, 10, 1));

            evaluation.Deactivate();

            Assert.False(evaluation.IsActive);
        }
    }
}
