using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduNation.Domain.Entities;
using EduNation.Infrastructure.Persistence;

namespace EduNation.API.Controllers
{
    [Route("api/institutions")]
    [ApiController]
    [Authorize]
    public class InstitutionsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public InstitutionsController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetInstitutions(CancellationToken cancellationToken)
        {
            var institutions = await _dbContext.Institutions
                .AsNoTracking()
                .Where(i => i.IsActive)
                .OrderBy(i => i.Name)
                .ToListAsync(cancellationToken);

            return Ok(institutions);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateInstitution([FromBody] CreateInstitutionRequest request, CancellationToken cancellationToken)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Code))
                return BadRequest();

            var institution = new Institution(request.Name, request.Code);
            await _dbContext.Institutions.AddAsync(institution, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(GetInstitutionById), new { id = institution.Id }, institution);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetInstitutionById(Guid id, CancellationToken cancellationToken)
        {
            var institution = await _dbContext.Institutions
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

            if (institution is null)
                return NotFound();

            return Ok(institution);
        }

        [HttpPost("{id:guid}/users")]
        [Authorize(Roles = "SuperAdmin,InstitutionAdmin")]
        public async Task<IActionResult> AssociateUser(Guid id, [FromBody] AssociateUserRequest request, CancellationToken cancellationToken)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.UserId))
                return BadRequest();

            var institution = await _dbContext.Institutions.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
            if (institution is null)
                return NotFound();

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null)
                return NotFound();

            var exists = await _dbContext.UserInstitutions
                .AnyAsync(ui => ui.UserId == request.UserId && ui.InstitutionId == id, cancellationToken);

            if (exists)
                return Conflict();

            var association = new UserInstitution(request.UserId, id, request.Role ?? "Student");
            await _dbContext.UserInstitutions.AddAsync(association, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        [HttpGet("{id:guid}/users")]
        public async Task<IActionResult> GetInstitutionUsers(Guid id, CancellationToken cancellationToken)
        {
            var users = await _dbContext.UserInstitutions
                .AsNoTracking()
                .Where(ui => ui.InstitutionId == id)
                .ToListAsync(cancellationToken);

            return Ok(users);
        }
    }

    public class CreateInstitutionRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class AssociateUserRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string? Role { get; set; }
    }
}
