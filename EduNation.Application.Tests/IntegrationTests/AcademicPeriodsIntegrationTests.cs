using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using EduNation.Application.AcademicPeriods;
using EduNation.Application.Tests.Infrastructure;

namespace EduNation.Application.Tests.Integration
{
    public class AcademicPeriodsIntegrationTests : IDisposable
    {
        private readonly CustomWebApplicationFactory _factory = new();
        private readonly HttpClient _client;

        public AcademicPeriodsIntegrationTests()
        {
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Fact]
        public async Task AcademicPeriodEndpoints_ShouldCreateReadUpdateAndDeactivatePeriod()
        {
            var createResponse = await _client.PostAsJsonAsync("/api/academicperiods", new AcademicPeriodRequest
            {
                Code = "fall-2025", Name = "Fall 2025", StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2025, 12, 15)
            });

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var created = await createResponse.Content.ReadFromJsonAsync<AcademicPeriodResponse>();
            Assert.NotNull(created);
            Assert.Equal("FALL-2025", created.Code);

            var getResponse = await _client.GetAsync($"/api/academicperiods/{created.Id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var retrieved = await getResponse.Content.ReadFromJsonAsync<AcademicPeriodResponse>();
            Assert.Equal(created.Id, retrieved?.Id);

            var updateResponse = await _client.PutAsJsonAsync($"/api/academicperiods/{created.Id}", new AcademicPeriodRequest
            {
                Code = "fall-2025-2", Name = "Fall 2025 II", StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2025, 12, 20)
            });
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            var updated = await updateResponse.Content.ReadFromJsonAsync<AcademicPeriodResponse>();
            Assert.Equal("FALL-2025-2", updated?.Code);
            Assert.Equal(new DateOnly(2025, 12, 20), updated?.EndDate);

            var deactivateResponse = await _client.DeleteAsync($"/api/academicperiods/{created.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"/api/academicperiods/{created.Id}")).StatusCode);
        }

        [Fact]
        public async Task GetAllAcademicPeriods_ShouldReturnOnlyActivePeriods()
        {
            var first = await CreatePeriodAsync("FALL-2025");
            var second = await CreatePeriodAsync("SPRING-2026");
            Assert.Equal(HttpStatusCode.NoContent, (await _client.DeleteAsync($"/api/academicperiods/{first.Id}")).StatusCode);

            var response = await _client.GetAsync("/api/academicperiods");
            var periods = await response.Content.ReadFromJsonAsync<List<AcademicPeriodResponse>>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(periods);
            Assert.Equal(second.Id, Assert.Single(periods).Id);
        }

        [Fact]
        public async Task CreateAcademicPeriod_ShouldReturnConflict_WhenCodeExistsIgnoringCase()
        {
            await CreatePeriodAsync("FALL-2025");
            var response = await _client.PostAsJsonAsync("/api/academicperiods", new AcademicPeriodRequest
            {
                Code = " fall-2025 ", Name = "Duplicate", StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2025, 12, 15)
            });
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.Equal("An academic period with this code already exists.", problem?.Detail);
        }

        [Theory]
        [InlineData("", "Valid name", 2025, 9, 1, 2025, 12, 15)]
        [InlineData("PERIOD-1", "", 2025, 9, 1, 2025, 12, 15)]
        public async Task CreateAcademicPeriod_ShouldReturnBadRequest_WhenInputIsInvalid(string code, string name, int sy, int sm, int sd, int ey, int em, int ed)
        {
            var response = await _client.PostAsJsonAsync("/api/academicperiods", new AcademicPeriodRequest
            {
                Code = code, Name = name, StartDate = new DateOnly(sy, sm, sd), EndDate = new DateOnly(ey, em, ed)
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateAcademicPeriod_ShouldReturnBadRequest_WhenDateRangeIsInvalid()
        {
            var response = await _client.PostAsJsonAsync("/api/academicperiods", new AcademicPeriodRequest
            {
                Code = "PERIOD-1", Name = "Valid name", StartDate = new DateOnly(2025, 12, 15), EndDate = new DateOnly(2025, 9, 1)
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAcademicPeriod_ShouldReturnConflict_WhenCodeBelongsToAnotherPeriod()
        {
            var existing = await CreatePeriodAsync("FALL-2025");
            var target = await CreatePeriodAsync("SPRING-2026");

            var response = await _client.PutAsJsonAsync($"/api/academicperiods/{target.Id}", new AcademicPeriodRequest
            {
                Code = existing.Code, Name = "Spring", StartDate = new DateOnly(2026, 1, 10), EndDate = new DateOnly(2026, 5, 20)
            });

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        private async Task<AcademicPeriodResponse> CreatePeriodAsync(string code)
        {
            var response = await _client.PostAsJsonAsync("/api/academicperiods", new AcademicPeriodRequest
            {
                Code = code, Name = $"Period {code}", StartDate = new DateOnly(2025, 9, 1), EndDate = new DateOnly(2025, 12, 15)
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return Assert.IsType<AcademicPeriodResponse>(await response.Content.ReadFromJsonAsync<AcademicPeriodResponse>());
        }
    }
}
