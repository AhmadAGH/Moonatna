using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moonatna.Models;
using Moonatna.Services.Families;
using Moonatna.Services.Items;
using Moonatna.ViewModels.Items;

namespace Moonatna.Controllers;

[Authorize]
public class ItemsController : BaseController
{
    private readonly IItemsService _items;
    private readonly IFamiliesService _families;
    private readonly IWebHostEnvironment _env;

    public ItemsController(IItemsService items, IFamiliesService families, IWebHostEnvironment env)
    {
        _items = items;
        _families = families;
        _env = env;
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddItemViewModel vm)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return BadRequest();

        var initialState = vm.IsAdHoc ? ItemState.OutOfStock : ItemState.Available;
        var item = await _items.AddItemAsync(family.Id, vm.Name, vm.CategoryId, vm.IsAdHoc, initialState, UserId, vm.Quantity);

        return Ok(new { id = item.Id, name = item.Name, state = (byte)item.State, isAdHoc = item.IsAdHoc, quantity = item.Quantity });
    }

    [HttpPost]
    public async Task<IActionResult> SetState([FromBody] SetItemStateViewModel vm)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var guard = await VerifyItemFamilyAsync(vm.ItemId);
        if (guard is not null) return guard;

        await _items.SetStateAsync(vm.ItemId, vm.State, UserId);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> SetQuantity([FromBody] SetItemQuantityViewModel vm)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var guard = await VerifyItemFamilyAsync(vm.ItemId);
        if (guard is not null) return guard;

        await _items.SetQuantityAsync(vm.ItemId, vm.Quantity, UserId);
        return Ok(new { itemId = vm.ItemId, quantity = vm.Quantity });
    }

    [HttpPost]
    public async Task<IActionResult> SetCategory([FromBody] SetItemCategoryViewModel vm)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var guard = await VerifyItemFamilyAsync(vm.ItemId);
        if (guard is not null) return guard;

        await _items.SetCategoryAsync(vm.ItemId, vm.CategoryId);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Purchase([FromBody] int itemId)
    {
        var guard = await VerifyItemFamilyAsync(itemId);
        if (guard is not null) return guard;

        await _items.PurchaseAsync(itemId, UserId);
        return Ok();
    }

    private async Task<IActionResult?> VerifyItemFamilyAsync(int itemId)
    {
        var item = await _items.GetByIdAsync(itemId);
        if (item is null) return NotFound();

        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null || item.FamilyId != family.Id) return Forbid();

        return null;
    }
}
