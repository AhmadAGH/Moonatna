using Microsoft.AspNetCore.Mvc;
using Moonatna.Models;
using Moonatna.Services.Families;
using System.Security.Claims;

namespace Moonatna.Controllers;

public abstract class BaseController : Controller
{
    private const string ActiveFamilyCookie = "Moonatna.ActiveFamily";

    protected int UserId => int.Parse(User.FindFirstValue("LocalUserId")!);

    protected int? ActiveFamilyId
    {
        get => int.TryParse(Request.Cookies[ActiveFamilyCookie], out var id) ? id : null;
        set
        {
            if (value.HasValue)
                Response.Cookies.Append(ActiveFamilyCookie, value.Value.ToString(),
                    new CookieOptions { HttpOnly = true, Expires = DateTimeOffset.UtcNow.AddYears(1) });
        }
    }

    // Resolves the active family FROM THE USER'S OWN LIST — membership is
    // guaranteed by construction, so no page can render another family's data.
    protected async Task<Family?> ResolveActiveFamilyAsync(IFamiliesService families)
    {
        var mine = (await families.GetMyFamiliesAsync(UserId)).ToList();
        if (mine.Count == 0) return null;

        var active = mine.FirstOrDefault(f => f.Id == ActiveFamilyId) ?? mine.First();
        ActiveFamilyId = active.Id;
        return active;
    }
}
