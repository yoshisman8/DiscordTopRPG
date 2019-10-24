using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DiscordTopRPG.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiscordTopRPG.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class IdentityController : ControllerBase
    {
        private SignInManager<ApplicationUser> signInManager { get; set; }
        private UserManager<ApplicationUser> UserManager { get; set; }
        public IdentityController(SignInManager<ApplicationUser> _signInManager, UserManager<ApplicationUser> _UserManager)
        {
            signInManager = _signInManager;
            UserManager = _UserManager;
        }
        [AllowAnonymous]
        public ActionResult Login(string provider, string returnUrl)
        {
            string redirecturl = Url.Action("ExternalLoginCallback", "Identity", new { returnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirecturl);
            return Challenge(properties,provider);
        }
        [HttpGet("api/login")]
		[AllowAnonymous]
        public ActionResult LoginRedirect(string returnUrl)
        {
            return Login("discord", returnUrl);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback()
        {
            string returnUrl = Url.Content("~/");
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return Redirect(returnUrl);
            }
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }
            else
            {
                var username = info.Principal.FindFirstValue(ClaimTypes.Name);
                if (username != null)
                {
                    var user = await UserManager.FindByNameAsync(info.Principal.Identity.Name);
                    if (user == null)
                    {
                        user = new ApplicationUser(info.Principal.FindFirstValue(ClaimTypes.Name),Convert.ToUInt64(info.ProviderKey));
                        await UserManager.CreateAsync(user);
                    }
                    await UserManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, true);
                }
                return Redirect(returnUrl);
            }
        }
    }
}