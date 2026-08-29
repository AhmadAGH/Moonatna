using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moonatna.Repositories.Lookups;   // adjust to TalabLink's ILookupsRepository namespace
using Moonatna.Services.Families;
using Moonatna.Services.Items;
using Moonatna.ViewModels.Shopping;
using System.Globalization;

namespace Moonatna.Controllers;

[Authorize]
public class ShoppingController : BaseController
{
    private readonly IItemsService _items;
    private readonly IFamiliesService _families;
    private readonly ILookupsRepository _lookups;

    public ShoppingController(IItemsService items, IFamiliesService families, ILookupsRepository lookups)
        => (_items, _families, _lookups) = (items, families, lookups);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return RedirectToAction("Onboarding", "Family");

        var categories = await _lookups.GetActiveCategoriesAsync();
        var isArabic = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar";
        var categoryNames = categories.ToDictionary(c => c.Id, c => isArabic ? c.NameAr : c.NameEn);
        var categoryIcons = categories.ToDictionary(c => c.Id, c => c.IconClass);

        var items = await _items.GetShoppingListAsync(family.Id);

        var vm = new ShoppingListViewModel
        {
            FamilyId = family.Id,
            FamilyName = family.Name,
            Items = items.Select(i => new ShoppingItemViewModel
            {
                Id = i.Id,
                Name = i.Name,
                State = i.State,
                IsAdHoc = i.IsAdHoc,
                ImagePath = i.ImagePath,
                Quantity = i.Quantity,
                CategoryName = i.CategoryId.HasValue && categoryNames.TryGetValue(i.CategoryId.Value, out var cn) ? cn : null,
                CategoryIcon = i.CategoryId.HasValue && categoryIcons.TryGetValue(i.CategoryId.Value, out var ci) ? ci : null
            }).ToList()
        };

        return View(vm);
    }
}
