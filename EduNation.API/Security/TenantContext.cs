using System.Security.Claims;
using EduNation.Application.Abstractions.Tenancy;

namespace EduNation.API.Security
{
    public class TenantContext : ITenantContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? InstitutionId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue("institution_id");
                return Guid.TryParse(value, out var id) ? id : null;
            }
        }

        public bool IsSuperAdmin
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.IsInRole("SuperAdmin") == true;
            }
        }

        public IReadOnlyCollection<Guid> AllowedInstitutionIds
        {
            get
            {
                var values = _httpContextAccessor.HttpContext?.User?.FindAll("institution_ids")
                    .Select(c => Guid.TryParse(c.Value, out var id) ? id : Guid.Empty)
                    .Where(id => id != Guid.Empty)
                    .ToList();

                return values ?? new List<Guid>();
            }
        }
    }
}
