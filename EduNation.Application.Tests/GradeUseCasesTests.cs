using Moq;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Grades;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class GradeUseCasesTests
    {
        private readonly Mock<IGradeRepository> _gradeRepository = new();
        private readonly Mock<IEvaluationRepository> _evaluationRepository = new();
        private readonly Mock<IEnrollmentRepository> _enrollmentRepository = new();

        private CreateGradeUseCase CreateUseCase() =>
            new(_gradeRepository.Object, _evaluationRepository.Object, _enrollmentRepository.Object);

        [Fact]
        public async Task Create_ShouldPersistGrade_WhenValid()
        {
            var courseSectionId = Guid.NewGuid();
            var evaluation = new Evaluation(courseSectionId, "Midterm", null, 100, 30, new DateOnly(2025, 10, 1));
            var realEnrollment = new Enrollment(Guid.NewGuid(), courseSectionId);

            _evaluationRepository.Setup(r => r.GetByIdAsync(evaluation.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evaluation);
            _enrollmentRepository.Setup(r => r.GetByIdAsync(realEnrollment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(realEnrollment);
            _gradeRepository.Setup(r => r.AddAsync(It.IsAny<Grade>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Grade grade, CancellationToken _) => grade);

            var result = await CreateUseCase().ExecuteAsync(new GradeRequest
            {
                EvaluationId = evaluation.Id,
                EnrollmentId = realEnrollment.Id,
                Score = 85,
                Comments = "Good"
            }, CancellationToken.None);

            Assert.Equal(85, result.Score);
            _gradeRepository.Verify(r => r.AddAsync(It.IsAny<Grade>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldThrowConflict_WhenEnrollmentInDifferentSection()
        {
            var evaluation = new Evaluation(Guid.NewGuid(), "Midterm", null, 100, 30, new DateOnly(2025, 10, 1));
            var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());

            _evaluationRepository.Setup(r => r.GetByIdAsync(evaluation.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evaluation);
            _enrollmentRepository.Setup(r => r.GetByIdAsync(enrollment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);

            await Assert.ThrowsAsync<ConflictException>(() => CreateUseCase().ExecuteAsync(new GradeRequest
            {
                EvaluationId = evaluation.Id,
                EnrollmentId = enrollment.Id,
                Score = 85
            }, CancellationToken.None));
        }

        [Fact]
        public async Task Create_ShouldThrowNotFound_WhenEvaluationIsInactive()
        {
            var evaluation = new Evaluation(Guid.NewGuid(), "Midterm", null, 100, 30, new DateOnly(2025, 10, 1));
            evaluation.Deactivate();
            var courseSectionId = evaluation.CourseSectionId;
            var enrollment = new Enrollment(Guid.NewGuid(), courseSectionId);

            _evaluationRepository.Setup(r => r.GetByIdAsync(evaluation.Id, It.IsAny<CancellationToken>())).ReturnsAsync((Evaluation?)null);
            _enrollmentRepository.Setup(r => r.GetByIdAsync(enrollment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);

            await Assert.ThrowsAsync<NotFoundException>(() => CreateUseCase().ExecuteAsync(new GradeRequest
            {
                EvaluationId = evaluation.Id,
                EnrollmentId = enrollment.Id,
                Score = 85
            }, CancellationToken.None));
        }

        [Fact]
        public async Task Create_ShouldThrowArgumentOutOfRange_WhenScoreExceedsMax()
        {
            var courseSectionId = Guid.NewGuid();
            var evaluation = new Evaluation(courseSectionId, "Midterm", null, 100, 30, new DateOnly(2025, 10, 1));
            var enrollment = new Enrollment(Guid.NewGuid(), courseSectionId);

            _evaluationRepository.Setup(r => r.GetByIdAsync(evaluation.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evaluation);
            _enrollmentRepository.Setup(r => r.GetByIdAsync(enrollment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => CreateUseCase().ExecuteAsync(new GradeRequest
            {
                EvaluationId = evaluation.Id,
                EnrollmentId = enrollment.Id,
                Score = 101
            }, CancellationToken.None));
        }

        [Fact]
        public async Task Create_ShouldPropagateConflict_WhenDuplicateExists()
        {
            var courseSectionId = Guid.NewGuid();
            var evaluation = new Evaluation(courseSectionId, "Midterm", null, 100, 30, new DateOnly(2025, 10, 1));
            var enrollment = new Enrollment(Guid.NewGuid(), courseSectionId);

            _evaluationRepository.Setup(r => r.GetByIdAsync(evaluation.Id, It.IsAny<CancellationToken>())).ReturnsAsync(evaluation);
            _enrollmentRepository.Setup(r => r.GetByIdAsync(enrollment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);
            _gradeRepository.Setup(r => r.AddAsync(It.IsAny<Grade>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConflictException("A grade for this evaluation and enrollment already exists."));

            await Assert.ThrowsAsync<ConflictException>(() => CreateUseCase().ExecuteAsync(new GradeRequest
            {
                EvaluationId = evaluation.Id,
                EnrollmentId = enrollment.Id,
                Score = 85
            }, CancellationToken.None));
        }

        [Fact]
        public async Task Update_ShouldChangeScoreAndSave()
        {
            var grade = new Grade(Guid.NewGuid(), Guid.NewGuid(), 80, "Initial");
            var evaluation = new Evaluation(Guid.NewGuid(), "Midterm", null, 100, 30, new DateOnly(2025, 10, 1));

            _gradeRepository.Setup(r => r.GetByIdAsync(grade.Id, It.IsAny<CancellationToken>())).ReturnsAsync(grade);
            _evaluationRepository.Setup(r => r.GetByIdAsync(grade.EvaluationId, It.IsAny<CancellationToken>())).ReturnsAsync(evaluation);

            var result = await new UpdateGradeUseCase(_gradeRepository.Object, _evaluationRepository.Object).ExecuteAsync(grade.Id, 90, "Improved", CancellationToken.None);

            Assert.Equal(90, result.Score);
            _gradeRepository.Verify(r => r.UpdateAsync(grade, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
