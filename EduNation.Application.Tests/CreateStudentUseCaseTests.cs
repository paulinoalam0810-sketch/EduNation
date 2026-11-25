using Moq;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.CreateStudent;
using EduNation.Application.GetStudentById;
using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class CreateStudentUseCaseTests
    {
        private readonly Mock<IStudentRepository> _studentRepository;
        private readonly CreateStudentUseCase _createStudentUseCase;

        public CreateStudentUseCaseTests()
        {
            _studentRepository = new Mock<IStudentRepository>();
            _createStudentUseCase = new CreateStudentUseCase(_studentRepository.Object);
        }

        [Fact]
        public async Task CreateStudentUseCase_ShouldCreateStudent()
        {
            // Arrange: preparar los datos
            var request = new CreateStudentRequest
            {
                FirstName = "Alam",
                LastName = "Paulino",
                Email = "alam@example.com",
                DateOfBirth = new DateOnly(2008, 10, 8)
            };

            _studentRepository
                .Setup(r => r.AddAsync(
                    It.IsAny<Student>(),
                    It.IsAny<CancellationToken>()))
                .Returns((Student student, CancellationToken _) =>
                    Task.FromResult(student));

            //Act: ejecutar el caso de uso
            var result = await _createStudentUseCase.ExecuteAsync(
                request, CancellationToken.None);

            //Asert: comprobar los resultados
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("Alam", result.FirstName);
            Assert.Equal("Paulino", result.LastName);
            Assert.Equal("alam@example.com", result.Email);

            _studentRepository.Verify(
                r => r.AddAsync(
                    It.IsAny<Student>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateStudentUseCase_ShouldThrowArgumentException_WhenEmailIsInvalid()
        {
            //Arrange
            var request = new CreateStudentRequest
            {
                FirstName = "Alam",
                LastName = "Paulino",
                Email = "correo-invalido",
                DateOfBirth = new DateOnly(2008, 10, 8)
            };

            //Act
            await Assert.ThrowsAsync<ArgumentException>(
                () => _createStudentUseCase.ExecuteAsync(
                    request, CancellationToken.None));

            //Assert
            _studentRepository.Verify(
                r => r.AddAsync(
                    It.IsAny<Student>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
