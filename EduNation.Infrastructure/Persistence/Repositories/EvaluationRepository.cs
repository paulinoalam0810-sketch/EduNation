using Microsoft.EntityFrameworkCore;
using EduNation.Application.Abstractions.Persistence;
using EduNation.Domain.Entities;
using EduNation.Infrastructure.Persistence;

namespace EduNation.Infrastructure.Persistence.Repositories
{
    public class EvaluationRepository : IEvaluationRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public EvaluationRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Evaluation> AddAsync(Evaluation evaluation, CancellationToken cancellationToken)
        {
            await _applicationDbContext.Evaluations.AddAsync(evaluation, cancellationToken);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return evaluation;
        }

        public Task<Evaluation?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _applicationDbContext.Evaluations
                .FirstOrDefaultAsync(evaluation => evaluation.Id == id && evaluation.IsActive, cancellationToken);
        }

        public async Task<IReadOnlyList<Evaluation>> GetByCourseSectionIdAsync(Guid courseSectionId, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.Evaluations
                .AsNoTracking()
                .Where(evaluation => evaluation.CourseSectionId == courseSectionId && evaluation.IsActive)
                .OrderBy(evaluation => evaluation.EvaluationDate)
                .ThenBy(evaluation => evaluation.Name)
                .ThenBy(evaluation => evaluation.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(Evaluation evaluation, CancellationToken cancellationToken)
        {
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
