using Moq;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.AcademicPeriods;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class AcademicPeriodUseCasesTests
    {
        private readonly Mock<IAcademicPeriodRepository> _repository = new();

        [Fact]
        public async Task Create_ShouldPersistNormalizedPeriod_WhenCodeIsAvailable()
        {
            _repository.Setup(repository => repository.ExistsByCodeAsync("FALL-2025", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _repository.Setup(repository => repository.AddAsync(It.IsAny<AcademicPeriod>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AcademicPeriod academicPeriod, CancellationToken _) => academicPeriod);

            var result = await new CreateAcademicPeriodUseCase(_repository.Object).ExecuteAsync(
                new AcademicPeriodRequest { Code = "fall-2025", Name = "Fall 2025", StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2025, 12, 15) }, CancellationToken.None);

            Assert.Equal("FALL-2025", result.Code);
            Assert.True(result.IsActive);
            _repository.Verify(repository => repository.AddAsync(It.IsAny<AcademicPeriod>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldThrowConflict_WhenCodeAlreadyExists()
        {
            _repository.Setup(repository => repository.ExistsByCodeAsync("FALL-2025", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            await Assert.ThrowsAsync<ConflictException>(() => new CreateAcademicPeriodUseCase(_repository.Object).ExecuteAsync(
                new AcademicPeriodRequest { Code = "fall-2025", Name = "Fall 2025", StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2025, 12, 15) }, CancellationToken.None));

            _repository.Verify(repository => repository.AddAsync(It.IsAny<AcademicPeriod>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetById_ShouldThrowNotFound_WhenPeriodIsMissing()
        {
            _repository.Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AcademicPeriod?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => new GetAcademicPeriodByIdUseCase(_repository.Object)
                .ExecuteAsync(Guid.NewGuid(), CancellationToken.None));
        }

        [Fact]
        public async Task Update_ShouldExcludeCurrentPeriodFromDuplicateCheck()
        {
            var academicPeriod = new AcademicPeriod("Fall 2025", "FALL-2025", new DateOnly(2025, 9, 1), new DateOnly(2025, 12, 15));
            _repository.Setup(repository => repository.GetByIdAsync(academicPeriod.Id, It.IsAny<CancellationToken>())).ReturnsAsync(academicPeriod);
            _repository.Setup(repository => repository.ExistsByCodeAsync("SPRING-2026", academicPeriod.Id, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var result = await new UpdateAcademicPeriodUseCase(_repository.Object).ExecuteAsync(academicPeriod.Id,
                new AcademicPeriodRequest { Code = "spring-2026", Name = "Spring 2026", StartDate = new DateOnly(2026, 1, 10), EndDate = new DateOnly(2026, 5, 20) }, CancellationToken.None);

            Assert.Equal("Spring 2026", result.Name);
            _repository.Verify(repository => repository.UpdateAsync(academicPeriod, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrowConflict_WhenAnotherPeriodUsesCode()
        {
            var academicPeriod = new AcademicPeriod("Fall 2025", "FALL-2025", new DateOnly(2025, 9, 1), new DateOnly(2025, 12, 15));
            _repository.Setup(repository => repository.GetByIdAsync(academicPeriod.Id, It.IsAny<CancellationToken>())).ReturnsAsync(academicPeriod);
            _repository.Setup(repository => repository.ExistsByCodeAsync("MATH-1", academicPeriod.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<ConflictException>(() => new UpdateAcademicPeriodUseCase(_repository.Object).ExecuteAsync(academicPeriod.Id,
                new AcademicPeriodRequest { Code = "MATH-1", Name = "Math", StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2025, 12, 15) }, CancellationToken.None));

            _repository.Verify(repository => repository.UpdateAsync(It.IsAny<AcademicPeriod>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Deactivate_ShouldSetInactiveAndSave()
        {
            var academicPeriod = new AcademicPeriod("Fall 2025", "FALL-2025", new DateOnly(2025, 9, 1), new DateOnly(2025, 12, 15));
            _repository.Setup(repository => repository.GetByIdAsync(academicPeriod.Id, It.IsAny<CancellationToken>())).ReturnsAsync(academicPeriod);

            await new DeactivateAcademicPeriodUseCase(_repository.Object).ExecuteAsync(academicPeriod.Id, CancellationToken.None);

            Assert.False(academicPeriod.IsActive);
            _repository.Verify(repository => repository.UpdateAsync(academicPeriod, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
