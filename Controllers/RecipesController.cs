using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moonatna.Models;
using Moonatna.Services.Families;
using Moonatna.Services.Items;
using Moonatna.Services.Recipes;
using Moonatna.ViewModels.Recipes;

namespace Moonatna.Controllers;

[Authorize]
public class RecipesController : BaseController
{
    private readonly IRecipesService _recipes;
    private readonly IItemsService _items;
    private readonly IFamiliesService _families;

    public RecipesController(IRecipesService recipes, IItemsService items, IFamiliesService families)
        => (_recipes, _items, _families) = (recipes, items, families);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return RedirectToAction("Onboarding", "Family");

        var recipes = await _recipes.GetRecipesWithBadgesAsync(family.Id);
        var counts = await _recipes.GetMissingCountsAsync(family.Id);

        var vm = new RecipesIndexViewModel
        {
            FamilyId = family.Id,
            Recipes = recipes.Select(r => new RecipeCardViewModel
            {
                Id = r.Id,
                Name = r.Name,
                PhotoPath = r.PhotoPath,
                MissingCount = counts.GetValueOrDefault(r.Id)
            }).ToList()
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return RedirectToAction("Onboarding", "Family");

        var recipe = await _recipes.GetByIdAsync(id);
        if (recipe is null || recipe.FamilyId != family.Id) return NotFound();

        var ingredients = await _recipes.GetIngredientsAsync(id);

        var rows = new List<RecipeIngredientViewModel>();
        foreach (var ing in ingredients)
        {
            var item = await _items.GetByIdAsync(ing.ItemId);
            rows.Add(new RecipeIngredientViewModel
            {
                ItemId = ing.ItemId,
                Name = item?.Name ?? "?",
                QuantityText = ing.QuantityText,
                IsOptional = ing.IsOptional,
                IsAvailable = item is { IsArchived: false, State: ItemState.Available }
            });
        }

        var vm = new RecipeDetailsViewModel
        {
            Id = recipe.Id,
            FamilyId = recipe.FamilyId,
            Name = recipe.Name,
            PhotoPath = recipe.PhotoPath,
            Ingredients = rows,
            MissingCount = rows.Count(r => !r.IsOptional && !r.IsAvailable)
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult Create() => View(new RecipeCreateViewModel());

    // The Vue builder posts the whole recipe as JSON — including brand-new ingredients by name.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RecipeCreateViewModel vm)
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return BadRequest();
        if (string.IsNullOrWhiteSpace(vm.Name)) return BadRequest();

        var recipe = new Recipe
        {
            FamilyId = family.Id,
            Name = vm.Name.Trim(),
            PhotoPath = vm.PhotoPath,
            CreatedByUserId = UserId
        };

        var id = await _recipes.CreateRecipeAsync(recipe, vm.Ingredients, UserId);
        return Ok(new { redirect = Url.Action(nameof(Details), new { id }) });
    }

    // Plain form post from the details page — "add what's missing to the list".
    [HttpPost]
    public async Task<IActionResult> AddMissing(int recipeId)
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return RedirectToAction("Onboarding", "Family");

        var recipe = await _recipes.GetByIdAsync(recipeId);
        if (recipe is null || recipe.FamilyId != family.Id) return NotFound();

        await _recipes.AddMissingToListAsync(recipeId, UserId);
        return RedirectToAction(nameof(Details), new { id = recipeId });
    }
}
