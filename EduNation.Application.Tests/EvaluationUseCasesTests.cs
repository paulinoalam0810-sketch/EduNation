using Moq;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Grades;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class EvaluationUseCasesTests
    {
        private readonly Mock<IEvaluationRepository> _evaluationRepository = new();
        private readonly Mock<ICourseSectionRepository> _courseSectionRepository = new();

        [Fact]
        public async Task Create_ShouldPersistEvaluation_WhenSectionExists()
        {
            var courseSection = new CourseSection(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "SEC-1", "Morning", 30);
            _courseSectionRepository.Setup(r => r.GetByIdAsync(courseSection.Id, It.IsAny<CancellationToken>())).ReturnsAsync(courseSection);
            _evaluationRepository.Setup(r => r.AddAsync(It.IsAny<Evaluation>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Evaluation evaluation, CancellationToken _) => evaluation);

            var result = await new CreateEvaluationUseCase(_evaluationRepository.Object, _courseSectionRepository.Object).ExecuteAsync(
                new EvaluationRequest
                {
                    CourseSectionId = courseSection.Id,
                    Name = "Midterm",
                    MaxScore = 100,
                    Weight = 30,
                    EvaluationDate = new DateOnly(2025, 10, 1)
                }, CancellationToken.None);

            Assert.Equal("Midterm", result.Name);
            _evaluationRepository.Verify(r => r.AddAsync(It.IsAny<Evaluation>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldThrowNotFound_WhenSectionIsMissing()
        {
            _courseSectionRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseSection?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => new CreateEvaluationUseCase(_evaluationRepository.Object, _courseSectionRepository.Object).ExecuteAsync(
                new EvaluationRequest
                {
                    CourseSectionId = Guid.NewGuid(),
                    Name = "Midterm",
                    MaxScore = 100,
                    Weight = 30,
                    EvaluationDate = new DateOnly(2025, 10, 1)
                }, CancellationToken.None));
        }

        [Fact]
        public async Task GetById_ShouldThrowNotFound_WhenMissing()
        {
            _evaluationRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Evaluation?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => new GetEvaluationByIdUseCase(_evaluationRepository.Object)
                .ExecuteAsync(Guid.NewGuid(), CancellationToken.None));
        }

        [Fact]
        public async Task Deactivate_ShouldSetInactiveAndSave()
        {
            var evaluation = new Evaluation(Guid.NewGuid(), "Midterm", null, 100, 30, new DateOnly(2025, 10, 1));
            _evaluationRepository.Setup(r => r.GetByIdAsync(evaluation.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evaluation);

            await new DeactivateEvaluationUseCase(_evaluationRepository.Object).ExecuteAsync(evaluation.Id, CancellationToken.None);

            Assert.False(evaluation.IsActive);
            _evaluationRepository.Verify(r => r.UpdateAsync(evaluation, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
