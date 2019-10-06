using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordTopRPG.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordTopRPG
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var host = CreateWebHostBuilder(args).Build();

			var client = host.Services.GetRequiredService<DiscordSocketClient>();
			var config = host.Services.GetService<IConfiguration>();

			var services = host.Services;
			services.GetRequiredService<LogService>();
			await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);

			await client.LoginAsync(TokenType.Bot, config["Tokens:Discord"]);
			await client.StartAsync();

			await host.RunAsync();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}
