using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DiscordTopRPG.Data;
using DiscordTopRPG.Services;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using LiteDB;
using LiteDiscordIdentity;

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

			services.AddIdentity<DiscordUser, DiscordRole>(options =>
			{
				 options.User.RequireUniqueEmail = false;
				 options.Password.RequireDigit = false;
				 options.Password.RequiredLength = 4;
				 options.Password.RequiredUniqueChars = 0;
				 options.Password.RequireNonAlphanumeric = false;
				 options.Password.RequireLowercase = false;
				 options.Password.RequireUppercase = false;
				 options.Lockout.AllowedForNewUsers = false;
			 }).AddUserStore<LiteDbUserStore>()
			 .AddRoleStore<LiteDbRoleStore>()
			 .AddDefaultTokenProviders();
			services.AddAuthorization();

			// Adding LiteDb Identity Storage classes
			services.AddSingleton<LiteDbContext>();

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
