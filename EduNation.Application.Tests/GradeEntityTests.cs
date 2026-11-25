using System.Threading;
using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class GradeEntityTests
    {
        [Fact]
        public void Constructor_ShouldSetDefaults()
        {
            var evaluationId = Guid.NewGuid();
            var enrollmentId = Guid.NewGuid();

            var grade = new Grade(evaluationId, enrollmentId, 85, "Good");

            Assert.Equal(evaluationId, grade.EvaluationId);
            Assert.Equal(enrollmentId, grade.EnrollmentId);
            Assert.Equal(85, grade.Score);
            Assert.Equal("Good", grade.Comments);
            Assert.NotEqual(Guid.Empty, grade.Id);
            Assert.True(grade.CreatedAt <= DateTimeOffset.UtcNow);
            Assert.True(grade.UpdatedAt <= DateTimeOffset.UtcNow);
        }

        [Fact]
        public void Constructor_ShouldRejectNegativeScore()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Grade(Guid.NewGuid(), Guid.NewGuid(), -1, null));
        }

        [Fact]
        public void Update_ShouldChangeScoreAndUpdatedAt()
        {
            var grade = new Grade(Guid.NewGuid(), Guid.NewGuid(), 80, "Initial");
            var createdAt = grade.CreatedAt;
            var updatedAt = grade.UpdatedAt;

            Thread.Sleep(5);
            grade.Update(90, "Improved");

            Assert.Equal(90, grade.Score);
            Assert.Equal("Improved", grade.Comments);
            Assert.Equal(createdAt, grade.CreatedAt);
            Assert.True(grade.UpdatedAt >= updatedAt);
        }
    }
}
