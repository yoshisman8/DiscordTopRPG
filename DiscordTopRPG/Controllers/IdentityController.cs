using DiscordTopRPG.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DiscordTopRPG.Controllers
{
	[ApiController]
	public class IdentityController : Controller
    {
		private SignInManager<ApplicationUser> signInManager { get; set; }
        private UserManager<ApplicationUser> UserManager { get; set; }
        public IdentityController(SignInManager<ApplicationUser> _signInManager, UserManager<ApplicationUser> _UserManager)
        {
            signInManager = _signInManager;
            UserManager = _UserManager;
        }
		[HttpGet("login")]
		public async Task<IActionResult> login(string redirectUrl)
		{
			redirectUrl = redirectUrl ?? "/";

			string redirectUri = Url.Action("ExternalLoginCallback", "Identity",new { returnUrl = redirectUrl });


			// var properties = await signInManager.GetExternalAuthenticationSchemesAsync();

			var props = signInManager.ConfigureExternalAuthenticationProperties("Discord", redirectUri);

			return Challenge(props, "Discord");
		}
		[HttpPost("logout"), HttpGet("logout")]
		[Authorize]
		public async Task Logout()
		{
			await HttpContext.SignOutAsync(new AuthenticationProperties { RedirectUri = "/" });
			HttpContext.Response.Redirect("/");
		}
		//[HttpGet("signin")]
		//public async Task<IActionResult> SiginAsync(string redirectUrl)
		//{
		//	redirectUrl = redirectUrl ?? "/";

		//	var info = await signInManager.GetExternalLoginInfoAsync();

		//	await signInManager.ExternalLoginSignInAsync("Discord", info.ProviderKey , true);

		//	// await HttpContext.SignInAsync("Discord", User);

		//	return Redirect(redirectUrl);
		//}
		[HttpGet("signin")]
		public async Task ExternalLoginCallback(string returnUrl)
		{
			returnUrl = returnUrl ?? "/";
			var info = await signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				HttpContext.Response.Redirect(returnUrl);
				return;
			}
			var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
			if (result.Succeeded)
			{
				HttpContext.Response.Redirect(returnUrl);
				return;
			}
			else
			{
				var username = info.Principal.FindFirstValue(ClaimTypes.Name);
				if (username != null)
				{
					var user = await UserManager.FindByNameAsync(info.Principal.Identity.Name);
					if (user == null)
					{
						user = new ApplicationUser(info.Principal.FindFirstValue(ClaimTypes.Name), Convert.ToUInt64(info.ProviderKey));
						await UserManager.CreateAsync(user);
					}
					await UserManager.AddLoginAsync(user, info);
					await signInManager.SignInAsync(user, true);
				}
				HttpContext.Response.Redirect(returnUrl);
				return;
			}
		}
	}
}