using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EduNation.Infrastructure.Persistence;

namespace EduNation.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                return BadRequest();

            var user = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            const string role = "Student";
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            return CreatedAtAction(nameof(Me), new { }, new { user.Id, user.Email, role });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
                return Unauthorized();

            var memberships = await _dbContext.UserInstitutions
                .Where(ui => ui.UserId == user.Id)
                .ToListAsync(cancellationToken);

            var globalRoles = await _userManager.GetRolesAsync(user);
            Guid? activeInstitutionId = memberships.FirstOrDefault()?.InstitutionId;
            var activeRole = memberships.FirstOrDefault()?.Role ?? globalRoles.FirstOrDefault() ?? "Student";
            var allowedInstitutions = memberships.Select(m => m.InstitutionId).ToList();

            var token = GenerateJwtToken(user, activeInstitutionId, activeRole, allowedInstitutions);

            return Ok(new { token, user.Id, user.Email, roles = globalRoles, activeInstitutionId, activeRole, allowedInstitutions });
        }

        [HttpGet("me")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            var institutionId = User.FindFirstValue("institution_id");
            var institutionRole = User.FindFirstValue("institution_role");

            return Ok(new { user.Id, user.Email, roles, institutionId, institutionRole });
        }

        [HttpGet("institutions")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> GetUserInstitutions(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var memberships = await (
                from ui in _dbContext.UserInstitutions
                where ui.UserId == userId
                join i in _dbContext.Institutions on ui.InstitutionId equals i.Id
                select new
                {
                    ui.InstitutionId,
                    ui.Role,
                    i.Name,
                    i.Code,
                    i.IsActive
                }).ToListAsync(cancellationToken);

            return Ok(memberships);
        }

        [HttpPost("switch-institution")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> SwitchInstitution([FromBody] SwitchInstitutionRequest request, CancellationToken cancellationToken)
        {
            if (request is null)
                return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Unauthorized();

            var institution = await _dbContext.Institutions
                .FirstOrDefaultAsync(i => i.Id == request.InstitutionId && i.IsActive, cancellationToken);

            if (institution is null)
                return NotFound();

            var membership = await _dbContext.UserInstitutions
                .FirstOrDefaultAsync(ui => ui.UserId == userId && ui.InstitutionId == request.InstitutionId, cancellationToken);

            var isSuperAdmin = await _userManager.IsInRoleAsync(user, "SuperAdmin");
            if (membership is null && !isSuperAdmin)
                return Forbid();

            var role = membership?.Role ?? "SuperAdmin";

            var allowedInstitutions = await _dbContext.UserInstitutions
                .Where(ui => ui.UserId == userId)
                .Select(ui => ui.InstitutionId)
                .ToListAsync(cancellationToken);

            var token = GenerateJwtToken(user, request.InstitutionId, role, allowedInstitutions);

            return Ok(new { token, institutionId = request.InstitutionId, role, allowedInstitutions });
        }

        [HttpPost("logout")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Logout()
        {
            return NoContent();
        }

        private string GenerateJwtToken(IdentityUser user, Guid? activeInstitutionId, string role, IEnumerable<Guid> allowedInstitutions)
        {
            var key = _configuration["Jwt:Key"] ?? "EduNationDevelopmentKey_ChangeMe_1234567890!";
            var issuer = _configuration["Jwt:Issuer"] ?? "EduNation";
            var audience = _configuration["Jwt:Audience"] ?? "EduNation";
            var expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var m) ? m : 60;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.Add(new Claim(ClaimTypes.Role, role));

            if (activeInstitutionId.HasValue)
            {
                claims.Add(new Claim("institution_id", activeInstitutionId.Value.ToString()));
            }

            foreach (var institutionId in allowedInstitutions)
            {
                claims.Add(new Claim("institution_ids", institutionId.ToString()));
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Role { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class SwitchInstitutionRequest
    {
        public Guid InstitutionId { get; set; }
    }
}
