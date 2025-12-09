using Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Admin.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signIn;

    public AccountController(SignInManager<ApplicationUser> signIn)
    {
        _signIn = signIn;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var result = await _signIn.PasswordSignInAsync(
            email,
            password,
            isPersistent: true,
            lockoutOnFailure: false
        );

        if (!result.Succeeded)
        {
            ViewBag.Error = "Credenciales inválidas";
            return View();
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
}