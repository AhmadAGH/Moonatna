using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moonatna.Repositories.Lookups;   
using Moonatna.Services.Families;
using Moonatna.Services.Items;
using Moonatna.ViewModels.Orgnize;

namespace Moonatna.Controllers;

// The champion's page: assign categories to items. Passengers never see this.
[Authorize]
public class OrganizeController : BaseController
{
    private readonly IItemsService _items;
    private readonly IFamiliesService _families;
    private readonly ILookupsRepository _lookups;

    public OrganizeController(IItemsService items, IFamiliesService families, ILookupsRepository lookups)
        => (_items, _families, _lookups) = (items, families, lookups);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return RedirectToAction("Onboarding", "Family");

        // Categories via ILookupsRepository ONLY — the golden rule.
        var categories = await _lookups.GetActiveCategoriesAsync();
        var items = await _items.GetAllAsync(family.Id);   // all live items, not just pantry

        var vm = new OrganizeIndexViewModel
        {
            FamilyId = family.Id,
            Categories = categories.ToList(),
            Items = items.Select(i => new OrganizeItemViewModel
            {
                Id = i.Id,
                Name = i.Name,
                CategoryId = i.CategoryId
            }).ToList()
        };

        return View(vm);
    }
}
