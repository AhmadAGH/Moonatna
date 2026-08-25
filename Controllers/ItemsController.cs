using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moonatna.Models;
using Moonatna.Services.Families;
using Moonatna.Services.Items;
using Moonatna.ViewModels.Items;

namespace Moonatna.Controllers;

// AJAX endpoints for item mutations — every page (pantry, list, organize) calls these.
[Authorize]
public class ItemsController : BaseController
{
    private readonly IItemsService _items;
    private readonly IFamiliesService _families;

    public ItemsController(IItemsService items, IFamiliesService families)
        => (_items, _families) = (items, families);

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddItemViewModel vm)
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return BadRequest();
        if (string.IsNullOrWhiteSpace(vm.Name)) return BadRequest();

        // Pantry add = Available, list add (ad-hoc) = OutOfStock
        var initialState = vm.IsAdHoc ? ItemState.OutOfStock : ItemState.Available;
        var item = await _items.AddItemAsync(family.Id, vm.Name, vm.CategoryId, vm.IsAdHoc, initialState, UserId);

        return Ok(new { item.Id, item.Name, state = (int)item.State, item.IsAdHoc });
    }

    [HttpPost]
    public async Task<IActionResult> SetState([FromBody] SetItemStateViewModel vm)
    {
        var guard = await VerifyItemFamilyAsync(vm.ItemId);
        if (guard is not null) return guard;

        await _items.SetStateAsync(vm.ItemId, vm.State, UserId);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Purchase([FromBody] PurchaseItemViewModel vm)
    {
        var guard = await VerifyItemFamilyAsync(vm.ItemId);
        if (guard is not null) return guard;

        await _items.PurchaseAsync(vm.ItemId, UserId);
        // The item leaves the shopping list in all four outcomes — the UI just removes the row.
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> SetCategory([FromBody] SetItemCategoryViewModel vm)
    {
        var guard = await VerifyItemFamilyAsync(vm.ItemId);
        if (guard is not null) return guard;

        await _items.SetCategoryAsync(vm.ItemId, vm.CategoryId);
        return Ok();
    }

    // IDOR guard: never mutate an item that belongs to another family.
    private async Task<IActionResult?> VerifyItemFamilyAsync(int itemId)
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return BadRequest();

        var item = await _items.GetByIdAsync(itemId);
        if (item is null || item.FamilyId != family.Id) return NotFound();

        return null;
    }
}
