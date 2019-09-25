using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordTopRPG.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DiscordTopRPG.Controllers
{
    public class IdentityController : Controller
    {
        private SignInManager<IdentityUser> signInManager { get; set; }
        private UserManager<IdentityUser> UserManager { get; set; }
        public IdentityController(SignInManager<IdentityUser> _signInManager, UserManager<IdentityUser> _UserManager)
        {
            signInManager = _signInManager;
            UserManager = _UserManager;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string provider, string returnUrl)
        {
            string redirecturl = Url.Action("ExternalLoginCallback", "Identity", new { returnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirecturl);
            return Challenge(properties,provider);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public async Task<ActionResult> ExternalLoginCallback(string returnUrl,string code)
        {
            var result = await signInManager.ExternalLoginSignInAsync("discord","", true);
            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }
            else
            {
                var error = new ErrorViewModel() {RequestId = Request.ToString() };
                return View("Error", error);
            }
        }
    }
}