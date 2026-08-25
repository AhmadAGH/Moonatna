using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Moonatna.Services.Users;
using Moonatna.ViewModels.Auth;
using System.Security.Claims;

namespace Moonatna.Controllers;

public class AuthController : Controller
{
    private readonly IUsersService _users;

    public AuthController(IUsersService users) => _users = users;

    [HttpGet]
    public IActionResult Login() => View();

    // The Vue login page signs in with Google via Firebase JS, then posts the ID token here.
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
            return Unauthorized();
        }

        var name = decoded.Claims.TryGetValue("name", out var n) ? n?.ToString() ?? "User" : "User";
        var user = await _users.GetOrCreateAsync(decoded.Uid, name);

        var claims = new List<Claim>
        {
            new("LocalUserId", user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

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
