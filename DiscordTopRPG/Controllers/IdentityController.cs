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
		private static ApplicationUser LoggedOutUser = new ApplicationUser { IsAuthenticated = false };
		private SignInManager<ApplicationUser> signInManager { get; set; }
        private UserManager<ApplicationUser> UserManager { get; set; }
        public IdentityController(SignInManager<ApplicationUser> _signInManager, UserManager<ApplicationUser> _UserManager)
        {
            signInManager = _signInManager;
            UserManager = _UserManager;
        }

		[HttpGet("user")]
		public ApplicationUser GetUser()
		{
			return User.Identity.IsAuthenticated ? new ApplicationUser { UserName = User.Identity.Name, IsAuthenticated = true } : LoggedOutUser;
		}
		[HttpGet("login")]
		public async Task login(string redirectUrl)
		{
			string redirectUri = Url.Action("ExternalLoginCallback","Identity",new { returnUrl = redirectUrl});
			
			var properties = await signInManager.GetExternalAuthenticationSchemesAsync();

			await HttpContext.ChallengeAsync(properties.FirstOrDefault().Name, new AuthenticationProperties { RedirectUri = "/" });
		}
		[HttpGet("signout")]
		public async Task<IActionResult> SignOut()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Redirect("~/");
		}
		//[AllowAnonymous]
		//public ActionResult Login(string provider, string returnUrl)
		//{
		//	string redirecturl = Url.Action("ExternalLoginCallback", "Identity", new { returnUrl = returnUrl });

		//	return Challenge(properties, provider);
		//}
		//[HttpGet("login")]
		//[AllowAnonymous]
		//public ActionResult LoginRedirect(string returnUrl)
		//{
		//	return Login("discord", returnUrl);
		//}
		[HttpPost("logout")]
		[Authorize]
		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();
			return Redirect("~/");
		}

		[HttpGet("signin-discord")]
		public async Task ExternalLoginCallback(string returnUrl)
		{
			returnUrl = returnUrl ?? Url.Content("~/");
			var info = await signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				HttpContext.Response.Redirect(returnUrl);
			}
			var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
			if (result.Succeeded)
			{
				HttpContext.Response.Redirect(returnUrl);
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
			}
		}
	}
}