using Moq;
using Students.Application.Abstractions.Persistence;
using Students.Application.DeactivateStudent;
using Students.Domain.Entities;

namespace Students.Application.Tests
{
    public class DeactivateStudentUseCaseTests
    {
        private readonly Mock<IStudentRepository> _studentRepository;
        private readonly DeactivateStudentUseCase  _deactivateStudentUseCase;

        public DeactivateStudentUseCaseTests()
        {
            _studentRepository = new Mock<IStudentRepository>();
            _deactivateStudentUseCase = new DeactivateStudentUseCase(_studentRepository.Object);
        }

        [Fact]
        public async Task DeactivateStudentUseCase_ShouldDeactivateStudent_WhenStudentExists()
        {
            //Arrange
            var student = new Student
            ( 
                "Alam",
                "Paulino",
                "alamp@example.com",
                new DateOnly(2008, 10, 8)
            );

            _studentRepository
                .Setup(r => r.GetByIdAsync(
                    student.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(student);

            //Act
            await _deactivateStudentUseCase.ExecuteAsync(student.Id, CancellationToken.None);

            //Assert
            Assert.False(student.IsActive);

            _studentRepository.Verify(
                r => r.UpdateAsync(
                    student,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
