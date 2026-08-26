using System.Globalization;
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
    {
        _recipes = recipes;
        _items = items;
        _families = families;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return RedirectToAction("Onboarding", "Family");

        var recipes = await _recipes.GetRecipesWithBadgesAsync(family.Id);
        var counts = await _recipes.GetIngredientCountsAsync(family.Id);

        var vm = new RecipesIndexViewModel
        {
            FamilyId = family.Id,
            Recipes = recipes.Select(r =>
            {
                counts.TryGetValue(r.Id, out var bc);
                return new RecipeCardViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    PhotoPath = r.PhotoPath,
                    MissingCount = bc?.MissingCount ?? 0,
                    RequiredCount = bc?.RequiredCount ?? 0
                };
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
                Id = ing.Id,
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

    // The builder posts the whole recipe as JSON — including brand-new ingredients by name.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RecipeCreateViewModel vm)
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return RedirectToAction("Onboarding", "Family");

        var recipe = new Recipe
        {
            FamilyId = family.Id,
            Name = vm.Name,
            PhotoPath = vm.PhotoPath,
            CreatedByUserId = UserId
        };

        var id = await _recipes.CreateRecipeAsync(recipe, vm.Ingredients, UserId);
        return Ok(new { redirect = Url.Action(nameof(Details), new { id }) });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return RedirectToAction("Onboarding", "Family");

        var recipe = await _recipes.GetByIdAsync(id);
        if (recipe is null || recipe.FamilyId != family.Id) return NotFound();

        var ingredients = await _recipes.GetIngredientsAsync(id);

        var rows = new List<RecipeEditIngredientViewModel>();
        foreach (var ing in ingredients)
        {
            var item = await _items.GetByIdAsync(ing.ItemId);
            rows.Add(new RecipeEditIngredientViewModel
            {
                Id = ing.Id,
                ItemId = ing.ItemId,
                Name = item?.Name ?? "?",
                QuantityText = ing.QuantityText,
                IsOptional = ing.IsOptional
            });
        }

        return View(new RecipeEditViewModel
        {
            Id = recipe.Id,
            Name = recipe.Name,
            PhotoPath = recipe.PhotoPath,
            Ingredients = rows
        });
    }

    // Replace-style update: name + full ingredient set. Untouched rows keep their
    // Item link; renamed rows re-link by name (existing item) or create a new one.
    [HttpPost]
    public async Task<IActionResult> Edit([FromBody] RecipeEditViewModel vm)
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return RedirectToAction("Onboarding", "Family");

        var recipe = await _recipes.GetByIdAsync(vm.Id);
        if (recipe is null || recipe.FamilyId != family.Id) return NotFound();

        await _recipes.UpdateRecipeAsync(recipe, vm.Name, vm.PhotoPath, vm.Ingredients, UserId);
        return Ok(new { redirect = Url.Action(nameof(Details), new { id = recipe.Id }) });
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

    // Plain form post from the details overflow menu.
    [HttpPost]
    public async Task<IActionResult> Archive(int id)
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return RedirectToAction("Onboarding", "Family");

        var recipe = await _recipes.GetByIdAsync(id);
        if (recipe is null || recipe.FamilyId != family.Id) return NotFound();

        await _recipes.ArchiveAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // JSON call from the edit page — removes one ingredient row.
    [HttpPost]
    public async Task<IActionResult> RemoveIngredient([FromBody] RemoveIngredientViewModel vm)
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return BadRequest();

        var ok = await _recipes.RemoveIngredientAsync(vm.RecipeId, vm.IngredientId, family.Id);
        return ok ? Ok() : NotFound();
    }
}
