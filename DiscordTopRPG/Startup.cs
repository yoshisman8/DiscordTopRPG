﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DiscordTopRPG.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using ERA20.Services;
using DiscordTopRPG.Services;
using DiscordTopRPG.Models;

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
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));


            services.AddAuthentication().AddDiscord(x =>
            {
                x.AppId = Configuration["AppId"];
                x.AppSecret = Configuration["AppSecret"];
                x.Scope.Add("guilds");
                x.Scope.Add("identify");
            });

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
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            services.ConfigureApplicationCookie(x =>
            {
                x.SlidingExpiration = true;
                x.LoginPath = "/Identity/Login";
                x.LogoutPath = "/Home/Index";
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
