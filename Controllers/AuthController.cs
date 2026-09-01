using System.Security.Claims;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Moonatna.Services.Users;
using Moonatna.ViewModels.Auth;

namespace Moonatna.Controllers;

public class AuthController : Controller
{
    private readonly IUsersService _users;
    private readonly IConfiguration _config;

    public AuthController(IUsersService users, IConfiguration config)
    {
        _users = users;
        _config = config;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Pantry");

        return View(BuildFirebaseViewModel());
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Pantry");

        return View(BuildFirebaseViewModel());
    }

    private LoginViewModel BuildFirebaseViewModel() => new()
    {
        FirebaseApiKey = _config["Firebase:ApiKey"] ?? string.Empty,
        FirebaseAuthDomain = _config["Firebase:AuthDomain"] ?? string.Empty,
        FirebaseProjectId = _config["Firebase:ProjectId"] ?? string.Empty
    };

    // The login/register pages authenticate with Firebase JS (Google or email/password),
    // then post the resulting ID token here.
    [HttpPost]
    public async Task<IActionResult> Token([FromBody] AuthTokenViewModel vm)
    {
        FirebaseToken decoded;
        try
        {
            decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(vm.IdToken);
        }
        catch (FirebaseAuthException)
        {
            return Unauthorized(new { error = "Invalid Firebase ID token." });
        }

        var name = decoded.Claims.TryGetValue("name", out var n) ? n?.ToString() ?? "User" : "User";
        var user = await _users.GetOrCreateAsync(decoded.Uid, name);

        var claims = new List<Claim>
        {
            new("LocalUserId", user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14)
        };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);

        return Ok(new { redirect = Url.Action("Index", "Pantry") });
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete("Moonatna.ActiveFamily");
        return RedirectToAction(nameof(Login));
    }
}
