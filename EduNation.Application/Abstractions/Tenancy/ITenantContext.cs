namespace EduNation.Application.Abstractions.Tenancy
{
    public interface ITenantContext
    {
        Guid? InstitutionId { get; }
        bool IsSuperAdmin { get; }
        IReadOnlyCollection<Guid> AllowedInstitutionIds { get; }
    }
}
