using System.Security.Claims;
using BlindMatch.Core.Common;
using BlindMatch.Core.Interfaces.Repositories;
using BlindMatch.Core.Interfaces.Services;
using BlindMatch.Web.ViewModels.Match;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlindMatch.Web.Controllers;

[Authorize]
public class MatchController : Controller
{
    private readonly IMatchRepository _matchRepository;
    private readonly IIdentityRevealService _identityRevealService;

    public MatchController(
        IMatchRepository matchRepository,
        IIdentityRevealService identityRevealService)
    {
        _matchRepository = matchRepository;
        _identityRevealService = identityRevealService;
    }

    //Students see their proposal's match status and the revealed supervisor details.
    [HttpGet]
    [Authorize(Policy = Policies.StudentOnly)]
    public async Task<IActionResult> MyMatch()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var allMatches = await _matchRepository.FindAsync(m => m.StudentId == studentId);
        var match = allMatches.FirstOrDefault();

        if (match == null)
            return View("NoMatch");

        var supervisorDetails = await _identityRevealService
            .GetSupervisorDetailsForStudentAsync(match.ProposalId, studentId);

        if (supervisorDetails == null)
            return View("NoMatch");

        var vm = new MyMatchViewModel
        {
            ProposalId = match.ProposalId,
            MatchStatus = match.Status,
            SupervisorFullName = supervisorDetails.FullName,
            SupervisorEmail = supervisorDetails.Email,
            SupervisorDepartment = supervisorDetails.Department
        };

        return View(vm);
    }

    // Supervisors see all their confirmed matches with revealed student details.
    //This is where they land after confirming interest.

    [HttpGet]
    [Authorize(Policy = Policies.SupervisorOnly)]
    public async Task<IActionResult> MySupervisedMatches()
    {
        var supervisorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var matches = await _matchRepository.GetBySupervisorIdWithDetailsAsync(supervisorId);

        var vm = matches.Select(m => new SupervisedMatchItemViewModel
        {
            MatchId = m.Id,
            ProposalTitle = m.Proposal.Title,
            MatchStatus = m.Status,
            StudentFullName = m.Student.FullName,
            StudentEmail = m.Student.Email!,
            Programme = m.Student.Programme,
            YearOfStudy = m.Student.YearOfStudy
        }).ToList();

        return View(vm);
    }
}