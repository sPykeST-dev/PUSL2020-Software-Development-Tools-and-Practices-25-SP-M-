using BlindMatch.Core.Entities;

namespace BlindMatch.Core.Interfaces.Repositories;

public interface ISupervisorRepository : IRepository<Supervisor>
{
    Task<Supervisor?> GetByIdWithResearchAreasAsync(string id);
    Task<ICollection<Supervisor>> GetSupervisorsByResearchAreaAsync(int researchAreaId);
    Task<bool> HasCapacityAsync(string supervisorId);
    Task IncrementProjectCountAsync(string supervisorId);
    Task DecrementProjectCountAsync(string supervisorId);
}
