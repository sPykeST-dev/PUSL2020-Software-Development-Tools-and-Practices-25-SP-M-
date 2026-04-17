using BlindMatch.Core.Entities;
using BlindMatch.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlindMatch.Web.Pages.ModuleLeader;

[Authorize(Roles = "ModuleLeader")]
public class ResearchAreasModel : PageModel
{
    private readonly IRepository<ResearchArea> _researchAreaRepository;

    public ResearchAreasModel(IRepository<ResearchArea> researchAreaRepository)
    {
        _researchAreaRepository = researchAreaRepository;
    }

    public List<ResearchArea> ResearchAreas { get; set; } = new();

    [BindProperty]
    public ResearchArea NewArea { get; set; } = new();

    public async Task OnGetAsync()
    {
        var all = await _researchAreaRepository.GetAllAsync();
        ResearchAreas = all.OrderByDescending(r => r.IsActive).ThenBy(r => r.Name).ToList();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        NewArea.Name = NewArea.Name?.Trim() ?? string.Empty;
        NewArea.IsActive = true;

        await _researchAreaRepository.AddAsync(NewArea);
        TempData["Success"] = "Research area created.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRenameAsync(int id, string newName)
    {
        var item = await _researchAreaRepository.GetByIdAsync(id);
        if (item == null)
            return NotFound();

        item.Name = newName?.Trim() ?? string.Empty;
        await _researchAreaRepository.UpdateAsync(item);
        TempData["Success"] = "Research area renamed.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeactivateAsync(int id)
    {
        var item = await _researchAreaRepository.GetByIdAsync(id);
        if (item == null)
            return NotFound();

        item.IsActive = false;
        await _researchAreaRepository.UpdateAsync(item);
        TempData["Success"] = "Research area deactivated.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostReactivateAsync(int id)
    {
        var item = await _researchAreaRepository.GetByIdAsync(id);
        if (item == null)
            return NotFound();

        item.IsActive = true;
        await _researchAreaRepository.UpdateAsync(item);
        TempData["Success"] = "Research area reactivated.";
        return RedirectToPage();
    }
}