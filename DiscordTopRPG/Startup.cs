using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DiscordTopRPG.Data;
using Discord.WebSocket;
using Discord.Commands;
using DiscordTopRPG.Services;
using Discord.Addons.Interactive;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Blazorise;
using Microsoft.Extensions.Options;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using LiteDB;

namespace DiscordTopRPG
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }
		public DiscordSocketClient _client { get; } = new DiscordSocketClient();

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("DefaultConnection")));


			services.AddIdentity<ApplicationUser, IdentityRole>(options =>
			 {
				 options.User.RequireUniqueEmail = false;
				 options.Password.RequireDigit = false;
				 options.Password.RequiredLength = 4;
				 options.Password.RequiredUniqueChars = 0;
				 options.Password.RequireNonAlphanumeric = false;
				 options.Password.RequireLowercase = false;
				 options.Password.RequireUppercase = false;
				 options.Lockout.AllowedForNewUsers = false;
			 }).AddEntityFrameworkStores<ApplicationDbContext>()
			 .AddDefaultTokenProviders();

			services.AddAuthorization();

			services
				.AddAuthentication()
				.AddCookie()
				.AddDiscord(x =>
				{
					x.ClientId = Configuration["AppId"];
					x.ClientSecret = Configuration["AppSecret"];
					x.Scope.Add("guilds");
					x.Scope.Add("identify");
					x.Validate();
				});

			services.AddRazorPages(o =>
			{
				o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
			});
			services.AddServerSideBlazor();

			services.AddHttpContextAccessor();
			services.AddScoped<HttpContextAccessor>();
			services.AddHttpClient();
			services.AddScoped<HttpClient>();

			services.AddBlazorise( Options =>
			{
				Options.ChangeTextOnKeyPress = true;
			}).AddBootstrapProviders().AddFontAwesomeIcons();

			// Discord bot singletons
			services.AddSingleton(_client);
			services.AddSingleton<CommandService>();
			services.AddSingleton<CommandHandlingService>();
			services.AddSingleton<InteractiveService>();
			// Logging
			services.AddLogging();
			services.AddSingleton<LogService>();
			// Extra
			services.AddSingleton<Random>();
			services.AddSingleton(new LiteDatabase("database.db"));
			services.AddSingleton<CharacterService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseCookiePolicy();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapBlazorHub();
				endpoints.MapFallbackToPage("/_Host");
			});

		}
	}
}
