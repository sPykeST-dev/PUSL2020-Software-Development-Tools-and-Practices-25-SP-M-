using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces.Repositories;

namespace BlindMatch.Infrastructure.Repositories;

public class SupervisorRepository : Repository<Supervisor>, ISupervisorRepository
{
    public SupervisorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Supervisor?> GetByIdWithResearchAreasAsync(string id)
    {
        return await _context.Supervisors
            .Include(s => s.PreferredResearchAreas)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<ICollection<Supervisor>> GetSupervisorsByResearchAreaAsync(int researchAreaId)
    {
        return await _context.Supervisors
            .Include(s => s.PreferredResearchAreas)
            .Where(s => s.PreferredResearchAreas.Any(ra => ra.Id == researchAreaId))
            .ToListAsync();
    }

    public async Task<bool> HasCapacityAsync(string supervisorId)
    {
        var supervisor = await _context.Supervisors.FindAsync(supervisorId);
        return supervisor != null && supervisor.CurrentProjects < supervisor.MaxProjects;
    }

    public async Task IncrementProjectCountAsync(string supervisorId)
    {
        var supervisor = await _context.Supervisors.FindAsync(supervisorId);
        if (supervisor != null)
        {
            supervisor.CurrentProjects++;
            _context.Supervisors.Update(supervisor);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DecrementProjectCountAsync(string supervisorId)
    {
        var supervisor = await _context.Supervisors.FindAsync(supervisorId);
        if (supervisor != null && supervisor.CurrentProjects > 0)
        {
            supervisor.CurrentProjects--;
            _context.Supervisors.Update(supervisor);
            await _context.SaveChangesAsync();
        }
    }
}
