using Moq;
using Students.Application.Abstractions.Persistence;
using Students.Application.Exceptions;
using Students.Application.UpdateStudent;
using Students.Domain.Entities;

namespace Students.Application.Tests
{
    public class UpdateStudentUseCaseTests
    {
        private readonly Mock<IStudentRepository> _studentRepository;
        private readonly UpdateStudentUseCase _updateStudentUseCase;

        public UpdateStudentUseCaseTests()
        {
            _studentRepository = new Mock<IStudentRepository>();
            _updateStudentUseCase = new UpdateStudentUseCase(_studentRepository.Object);
        }

        [Fact]
        public async Task UpdateStudentUseCase_ShouldUpdateStudent_WhenStudentExists()
        {
            //Arrange
            var student = new Student
            (
                "Alam",
                "Paulino",
                "alamp@example.com",
                new DateOnly(2008, 10, 8)
            );

            var request = new UpdateStudentRequest
            {
                FirstName = "Samil",
                LastName = "Paulino",
                Email = "samilp@example.com",
                DateOfBirth = new DateOnly(2013, 3, 13)
            };

            _studentRepository
                .Setup(r => r.GetByIdAsync(
                    student.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(student);

            //Act
            var result = await _updateStudentUseCase.ExecuteAsync(student.Id, request, CancellationToken.None);

            //Assert
            Assert.Equal(student.Id, result.Id);

            Assert.Equal("Samil", result.FirstName);
            Assert.Equal("Paulino", result.LastName);
            Assert.Equal("samilp@example.com", result.Email);
            Assert.Equal(new DateOnly(2013, 3, 13), result.DateOfBirth);

            _studentRepository.Verify(
                r => r.UpdateAsync(
                    student,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateStudentUseCase_ShouldThrowNotFoundException_WhenStudentDoesNotExist()
        {
            //Arrange
            var studentId = Guid.NewGuid();

            var request = new UpdateStudentRequest
            {
                FirstName = "Samil",
                LastName = "Paulino",
                Email = "samilp@example.com",
                DateOfBirth = new DateOnly(2013, 3, 13)
            };

            _studentRepository
                .Setup(r => r.GetByIdAsync(
                    studentId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Student?)null);

            //Act
            await Assert.ThrowsAsync<NotFoundException>(
                async () => await _updateStudentUseCase.ExecuteAsync(
                    studentId,
                    request,
                    CancellationToken.None
                )
            );

            //Assert
            _studentRepository.Verify(
                r => r.UpdateAsync(
                    It.IsAny<Student>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
