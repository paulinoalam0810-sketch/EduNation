using Moq;
using Students.Application.Abstractions.Persistence;
using Students.Application.SearchStudents;
using Students.Domain.Entities;

namespace Students.Application.Tests
{
    public class SearchStudentsUseCaseTests
    {
        private readonly Mock<IStudentRepository> _studentRepository = new();
        private readonly SearchStudentsUseCase _useCase;

        public SearchStudentsUseCaseTests()
        {
            _useCase = new SearchStudentsUseCase(_studentRepository.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldMapStudentsAndPassTrimmedCriteria()
        {
            var student = new Student("Alex", "Johnson", "alex@example.com", new DateOnly(2000, 1, 1));
            var requestedCriteria = new StudentSearchCriteria
            {
                Search = "  Alex  ",
                PageNumber = 2,
                PageSize = 5
            };
            _studentRepository
                .Setup(repository => repository.SearchAsync(
                    It.Is<StudentSearchCriteria>(criteria =>
                        criteria.Search == "Alex" &&
                        criteria.PageNumber == 2 &&
                        criteria.PageSize == 5),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<Student>
                {
                    Items = [student],
                    PageNumber = 2,
                    PageSize = 5,
                    TotalCount = 7
                });

            var result = await _useCase.ExecuteAsync(requestedCriteria, CancellationToken.None);

            var mapped = Assert.Single(result.Items);
            Assert.Equal(student.Id, mapped.Id);
            Assert.Equal(student.Email, mapped.Email);
            Assert.Equal(2, result.PageNumber);
            Assert.Equal(5, result.PageSize);
            Assert.Equal(7, result.TotalCount);
            _studentRepository.VerifyAll();
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(1, 101)]
        public async Task ExecuteAsync_ShouldRejectInvalidPagination(int pageNumber, int pageSize)
        {
            var criteria = new StudentSearchCriteria { PageNumber = pageNumber, PageSize = pageSize };

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => _useCase.ExecuteAsync(criteria, CancellationToken.None));

            _studentRepository.Verify(
                repository => repository.SearchAsync(It.IsAny<StudentSearchCriteria>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
