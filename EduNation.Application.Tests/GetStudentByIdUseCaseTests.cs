using Moq;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.Exceptions;
using EduNation.Application.GetStudentById;
using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class GetStudentByIdUseCaseTests
    {
        private readonly Mock<IStudentRepository> _studentRepository;
        private readonly GetStudentByIdUseCase _getStudentByIdUseCase;

        public GetStudentByIdUseCaseTests()
        {
            _studentRepository = new Mock<IStudentRepository>();
            _getStudentByIdUseCase = new GetStudentByIdUseCase(_studentRepository.Object);
        }

        [Fact]
        public async Task GetStudentByIdUseCase_ShouldReturnStudent_WhenStudentExists()
        {
            //Arrange
            var student = new Student(

                "Alam",
                "Paulino",
                "alamp@example.com",
                new DateOnly(2008, 10, 8)
            );

            _studentRepository
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(student);

            //Act
            var result = await _getStudentByIdUseCase.ExecuteAsync(student.Id, CancellationToken.None);

            //Assert
            Assert.NotNull(result);

            Assert.Equal(student.Id, result.Id);
            Assert.Equal("Alam", result.FirstName);
            Assert.Equal("Paulino", result.LastName);
            Assert.Equal("alamp@example.com", result.Email);
            Assert.Equal(new DateOnly(2008, 10, 8), result.DateOfBirth);

            _studentRepository.Verify(
                r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetStudentByIdUseCase_ShouldThrowNotFoundException_WhenStudentDoesNotExists()
        {
            //Arrange
            _studentRepository
                .Setup(r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Student?)null);

            //Act
            await Assert.ThrowsAsync<NotFoundException>(
                () => _getStudentByIdUseCase.ExecuteAsync(
                    Guid.NewGuid(),
                    CancellationToken.None));

            //Assert
            _studentRepository.Verify(
                r => r.GetByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
