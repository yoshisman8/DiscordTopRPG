using DiscordTopRPG.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordTopRPG.Services
{
	public class IdentityService : ControllerBase
	{
		private SignInManager<ApplicationUser> signInManager { get; set; }
		private UserManager<ApplicationUser> UserManager { get; set; }
		private readonly HttpClient http;
		public IdentityService(SignInManager<ApplicationUser> _signInManager, UserManager<ApplicationUser> _UserManager, HttpClient _client)
		{
			http = _client;
			signInManager = _signInManager;
			UserManager = _UserManager;
		}

		public ActionResult Login(string provider, string returnUrl)
		{
			string redirecturl = Url.Action("ExternalLoginCallback", "Identity", new { returnUrl = returnUrl });
			var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirecturl);
			return Challenge(properties, provider);
		}
		public ActionResult LoginRedirect(string returnUrl)
		{
			return Login("discord", returnUrl);
		}
		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
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
						user = new ApplicationUser(info.Principal.FindFirstValue(ClaimTypes.Name), Convert.ToUInt64(info.ProviderKey));
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
