using Moq;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Teachers;
using EduNation.Application.Exceptions;
using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class TeacherUseCasesTests
    {
        private readonly Mock<ITeacherRepository> _repository = new();

        [Fact]
        public async Task Create_ShouldPersistTeacher_WhenEmailIsAvailable()
        {
            _repository.Setup(r => r.AddAsync(It.IsAny<Teacher>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Teacher teacher, CancellationToken _) => teacher);

            var result = await new CreateTeacherUseCase(_repository.Object).ExecuteAsync(
                new TeacherRequest { FirstName = "Alam", LastName = "Paulino", Email = "Alam@Example.com", PhoneNumber = "123" },
                CancellationToken.None);

            Assert.Equal("alam@example.com", result.Email);
            Assert.True(result.IsActive);
            _repository.Verify(r => r.AddAsync(It.IsAny<Teacher>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldThrowNotFound_WhenTeacherIsMissing()
        {
            _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Teacher?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => new GetTeacherByIdUseCase(_repository.Object)
                .ExecuteAsync(Guid.NewGuid(), CancellationToken.None));
        }

        [Fact]
        public async Task Update_ShouldPersistChanges()
        {
            var teacher = new Teacher("Alam", "Paulino", "alam@example.com");
            _repository.Setup(r => r.GetByIdAsync(teacher.Id, It.IsAny<CancellationToken>())).ReturnsAsync(teacher);

            var result = await new UpdateTeacherUseCase(_repository.Object).ExecuteAsync(teacher.Id,
                new TeacherRequest { FirstName = "Carlos", LastName = "Martinez", Email = "carlos@example.com", PhoneNumber = "999" },
                CancellationToken.None);

            Assert.Equal("Carlos", result.FirstName);
            _repository.Verify(r => r.UpdateAsync(teacher, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deactivate_ShouldSetInactiveAndSave()
        {
            var teacher = new Teacher("Alam", "Paulino", "alam@example.com");
            _repository.Setup(r => r.GetByIdAsync(teacher.Id, It.IsAny<CancellationToken>())).ReturnsAsync(teacher);

            await new DeactivateTeacherUseCase(_repository.Object).ExecuteAsync(teacher.Id, CancellationToken.None);

            Assert.False(teacher.IsActive);
            _repository.Verify(r => r.UpdateAsync(teacher, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
