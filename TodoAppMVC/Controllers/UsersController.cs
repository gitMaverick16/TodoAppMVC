using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoAppMVC.Models;

namespace TodoAppMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UsersController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (!ModelState.IsValid) { 
                return View(model);
            }
            var user = new IdentityUser()
            {
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(user, password: model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Login(string message = null)
        {
            if(message is not null)
            {
                ViewData["message"] = message;
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorrecto");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public ChallengeResult ExternalLogin(string provider, string urlReturn = null)
        {
            var urlRedirection = Url.Action("RegisterExternalUser", values: new { urlReturn });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, urlRedirection);
            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RegisterExternalUser(string urlReturn = null, string remoteError = null)
        {
            urlReturn = urlReturn ?? Url.Content("~/");

            var message = "";

            if(remoteError is not null)
            {
                message = $"Error del proveedor externo: {remoteError}";
                return RedirectToAction("login", routeValues: new { message});
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if(info is null)
            {
                message = "Error cargando la data del login externo";
                return RedirectToAction("login", routeValues: new { message });
            }

            var resultExternalLogin = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            if (resultExternalLogin.Succeeded)
            {
                return LocalRedirect(urlReturn);
            }

            var email = string.Empty;

            if(info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }
            else
            {
                message = "Error leyendo el email del usuario del proveedor";
                return RedirectToAction("login", routeValues: new { message });
            }

            var user = new IdentityUser
            {
                Email = email,
                UserName = email
            };

            var resultCreateUser = await _userManager.CreateAsync(user);

            if (!resultCreateUser.Succeeded)
            {
                message = resultCreateUser.Errors.First().Description;
                return RedirectToAction("login", routeValues: new { message });
            }

            var resultadoAddLogin = await _userManager.AddLoginAsync(user, info);

            if (resultadoAddLogin.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: true, info.LoginProvider);
                return LocalRedirect(urlReturn);
            }

            message = "Ha ocurrido un error agregando el login";
            return RedirectToAction("login", routeValues: new { message });
        }
    }
}
