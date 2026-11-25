using EduNation.Domain.Entities;

namespace EduNation.Application.AcademicPeriods
{
    internal static class AcademicPeriodMapper
    {
        public static AcademicPeriodResponse ToResponse(AcademicPeriod academicPeriod)
        {
            return new AcademicPeriodResponse
            {
                Id = academicPeriod.Id,
                Name = academicPeriod.Name,
                Code = academicPeriod.Code,
                StartDate = academicPeriod.StartDate,
                EndDate = academicPeriod.EndDate,
                CreatedAt = academicPeriod.CreatedAt,
                IsActive = academicPeriod.IsActive
            };
        }
    }
}
