using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moonatna.Models;
using Moonatna.Services.Families;
using Moonatna.ViewModels.Families;

namespace Moonatna.Controllers;

[Authorize]
public class FamilyController : BaseController
{
    private readonly IFamiliesService _families;

    public FamilyController(IFamiliesService families) => _families = families;

    [HttpGet]
    public IActionResult Onboarding() => View(new OnboardingViewModel());

    [HttpPost]
    public async Task<IActionResult> CreateFamily(OnboardingViewModel vm)
    {
        if (string.IsNullOrWhiteSpace(vm.CreateName))
        {
            vm.ErrorKey = "Family.Create.NameRequired";
            return View(nameof(Onboarding), vm);
        }

        var family = await _families.CreateFamilyAsync(vm.CreateName, UserId);
        ActiveFamilyId = family.Id;
        return RedirectToAction("Index", "Pantry");
    }

    [HttpPost]
    public async Task<IActionResult> Join(OnboardingViewModel vm)
    {
        if (string.IsNullOrWhiteSpace(vm.JoinCode))
        {
            vm.ErrorKey = "Family.Join.CodeRequired";
            return View(nameof(Onboarding), vm);
        }

        var family = await _families.JoinFamilyAsync(vm.JoinCode, UserId);
        if (family is null)
        {
            vm.ErrorKey = "Family.Join.InvalidCode";
            return View(nameof(Onboarding), vm);
        }

        ActiveFamilyId = family.Id;
        return RedirectToAction("Index", "Pantry");
    }

    [HttpGet]
    public async Task<IActionResult> Settings()
    {
        var family = await ResolveActiveFamilyAsync(_families);
        if (family is null) return RedirectToAction(nameof(Onboarding));

        var membership = await _families.GetMembershipAsync(family.Id, UserId);
        var members = await _families.GetMembersAsync(family.Id);

        var vm = new FamilySettingsViewModel
        {
            FamilyId = family.Id,
            Name = family.Name,
            JoinCode = family.JoinCode,
            AutoPromoteAdHoc = family.AutoPromoteAdHoc,
            IsOwner = membership?.Role == FamilyRole.Owner,
            Members = members.Select(m => new FamilyMemberViewModel
            {
                DisplayName = m.DisplayName,
                Role = m.Role
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSettings(FamilySettingsViewModel vm)
    {
        var ok = await _families.SetAutoPromoteAsync(vm.FamilyId, vm.AutoPromoteAdHoc, UserId);
        if (!ok) return Forbid();   // only the owner flips the switch
        return RedirectToAction(nameof(Settings));
    }
}
