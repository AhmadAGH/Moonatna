using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moonatna.Repositories.Lookups;   // adjust to TalabLink's ILookupsRepository namespace
using Moonatna.Services.Families;
using Moonatna.Services.Items;
using Moonatna.ViewModels.Pantries;
using System.Globalization;

namespace Moonatna.Controllers;

[Authorize]
public class PantryController : BaseController
{
    private readonly IItemsService _items;
    private readonly IFamiliesService _families;
    private readonly ILookupsRepository _lookups;

    public PantryController(IItemsService items, IFamiliesService families, ILookupsRepository lookups)
        => (_items, _families, _lookups) = (items, families, lookups);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return RedirectToAction("Onboarding", "Family");

        // Categories come ONLY through ILookupsRepository — the golden rule.
        // Adjust GetByTableAsync / NameAr / NameEn to your actual Lookup shape.
        var categories = await _lookups.GetActiveCategoriesAsync();
        var isArabic = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar";
        var categoryNames = categories.ToDictionary(c => c.Id, c => isArabic ? c.NameAr : c.NameEn);

        var items = await _items.GetPantryAsync(family.Id);

        var vm = new PantryIndexViewModel
        {
            FamilyId = family.Id,
            FamilyName = family.Name,
            Items = items.Select(i => new PantryItemViewModel
            {
                Id = i.Id,
                Name = i.Name,
                State = i.State,
                CategoryName = i.CategoryId.HasValue && categoryNames.TryGetValue(i.CategoryId.Value, out var cn) ? cn : null
            }).ToList()
        };

        return View(vm);
    }
}
