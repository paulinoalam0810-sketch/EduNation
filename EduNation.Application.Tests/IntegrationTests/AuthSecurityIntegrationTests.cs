using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EduNation.Application.Tests.Infrastructure;

namespace EduNation.Application.Tests.Integration
{
    public class AuthSecurityIntegrationTests : IDisposable
    {
        private readonly CustomWebApplicationFactory _factory = new();
        private readonly HttpClient _client;

        public AuthSecurityIntegrationTests()
        {
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Fact]
        public async Task Register_ShouldIgnorePrivilegedRole()
        {
            var email = $"user{Guid.NewGuid():N}@example.com";
            var response = await _client.PostAsJsonAsync("/api/auth/register", new
            {
                Email = email,
                Password = "Password123!",
                Role = "SuperAdmin"
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = "Password123!" });
            var json = await loginResponse.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(json);
            var roles = document.RootElement.GetProperty("roles").EnumerateArray().Select(x => x.GetString()).ToList();

            Assert.Contains("Student", roles);
            Assert.DoesNotContain("SuperAdmin", roles);
        }

        [Fact]
        public async Task Register_ShouldRejectDuplicateEmail()
        {
            var email = $"user{Guid.NewGuid():N}@example.com";
            await _client.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = "Password123!" });

            var response = await _client.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = "Password123!" });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            var email = $"user{Guid.NewGuid():N}@example.com";
            await _client.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = "Password123!" });

            var response = await _client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = "WrongPassword123!" });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ProtectedEndpoint_ShouldReturnUnauthorized_WhenNoToken()
        {
            var response = await _client.GetAsync("/api/institutions");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateInstitution_ShouldReturnForbidden_WhenUserIsNotSuperAdmin()
        {
            var email = $"user{Guid.NewGuid():N}@example.com";
            await _client.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = "Password123!" });

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = "Password123!" });
            var json = await loginResponse.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(json);
            var token = document.RootElement.GetProperty("token").GetString();

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsJsonAsync("/api/institutions", new { Name = "Test", Code = "TEST" });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task ProtectedEndpoint_ShouldReturnUnauthorized_WhenTokenIsInvalid()
        {
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalid.token.value");

            var response = await _client.GetAsync("/api/auth/me");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
