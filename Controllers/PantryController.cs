using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moonatna.Repositories.Lookups;   // adjust to TalabLink's ILookupsRepository namespace
using Moonatna.Services.Families;
using Moonatna.Services.Items;
using Moonatna.ViewModels.Pantries;
using Moonatna.ViewModels.Shared;
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
        // one lookup: the category carries both its name and whichever icon
        // kind it uses (Font Awesome class or a designed local file)
        var categoryById = categories.ToDictionary(c => c.Id);

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
                ImagePath = i.ImagePath,
                Quantity = i.Quantity,
                CategoryId = i.CategoryId,
                CategoryName = i.CategoryId.HasValue && categoryById.TryGetValue(i.CategoryId.Value, out var cn) ? (isArabic ? cn.NameAr : cn.NameEn) : null,
                CategoryIcon = i.CategoryId.HasValue && categoryById.TryGetValue(i.CategoryId.Value, out var ci) ? CategoryIconViewModel.From(ci) : null
            }).ToList()
        };

        return View(vm);
    }
}
