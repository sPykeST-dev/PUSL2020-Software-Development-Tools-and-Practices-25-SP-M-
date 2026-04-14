using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlindMatch.Infrastructure.Repositories;

public class StudentRepository : Repository<Student>, IStudentRepository
{
	public StudentRepository(ApplicationDbContext context) : base(context)
	{
	}

	public async Task<Student?> GetByUserIdAsync(string userId)
	{
		return await _context.Students
			.Include(s => s.Proposal)
			.ThenInclude(p => p!.ResearchArea)
			.FirstOrDefaultAsync(s => s.Id == userId);
	}
}
