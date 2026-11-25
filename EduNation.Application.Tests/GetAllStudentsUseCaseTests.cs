using Moq;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Application.GetAllStudents;
using EduNation.Domain.Entities;

namespace EduNation.Application.Tests
{
    public class GetAllStudentsUseCaseTests
    {
        private readonly Mock<IStudentRepository> _studentRepository;
        private readonly GetAllStudentsUseCase _getAllStudentsUseCase;

        public GetAllStudentsUseCaseTests()
        {
            _studentRepository = new Mock<IStudentRepository>();
            _getAllStudentsUseCase = new GetAllStudentsUseCase(_studentRepository.Object);
        }

        [Fact]
        public async Task GetAllStudentsUseCase_ShouldReturnAllStudents_WhenStudentsExists()
        {
            //Arrange
            var student = new Student(
                "Alam",
                "Paulino",
                "alamp@example.com",
                new DateOnly(2008, 10, 8)
            );

            var student1 = new Student(
                "Karen",
                "Romero",
                "karenr@example.com",
                new DateOnly(2008, 12, 8)
            );

            var students = new List<Student> { student, student1 };

            _studentRepository
                .Setup(r => r.GetAllAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(students);

            //Act
            var result = await _getAllStudentsUseCase.ExecuteAsync(CancellationToken.None);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Equal(student.Id, result[0].Id);
            Assert.Equal("Alam", result[0].FirstName);
            Assert.Equal("Paulino", result[0].LastName);
            Assert.Equal("alamp@example.com", result[0].Email);
            Assert.Equal(new DateOnly(2008, 10, 8), result[0].DateOfBirth);

            Assert.Equal(student1.Id, result[1].Id);
            Assert.Equal("Karen", result[1].FirstName);
            Assert.Equal("Romero", result[1].LastName);
            Assert.Equal("karenr@example.com", result[1].Email);
            Assert.Equal(new DateOnly(2008, 12, 8), result[1].DateOfBirth);

            _studentRepository.Verify(
                r => r.GetAllAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetAllStudentsUseCase_ShouldReturnEmptyList_WhenNoStudentsExists()
        {
            //Arrange
            _studentRepository
                .Setup(r => r.GetAllAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Student>());

            //Act
            var result = await _getAllStudentsUseCase.ExecuteAsync(CancellationToken.None);

            //Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _studentRepository.Verify(
                r => r.GetAllAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
