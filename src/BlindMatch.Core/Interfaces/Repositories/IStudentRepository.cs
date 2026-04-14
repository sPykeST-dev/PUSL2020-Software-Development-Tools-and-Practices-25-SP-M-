using BlindMatch.Core.Entities;

namespace BlindMatch.Core.Interfaces.Repositories;

public interface IStudentRepository : IRepository<Student>
{
	Task<Student?> GetByUserIdAsync(string userId);
}
