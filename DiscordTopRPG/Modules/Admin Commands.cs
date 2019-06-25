using Discord;
using Discord.Commands;
using DiscordTopRPG.Database;
using DiscordTopRPG.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordTopRPG.Modules
{
	public class Admin_Commands : DiscordTopBase<SocketCommandContext>
	{
		[Command("SetPrefix"),Alias("Prefix")]
		[RequireContext(ContextType.Guild)] [RequireUserPermission(GuildPermission.ManageGuild)]
		[Summary("Set the prefix for this server. Only the first character is used.")]
		public async Task SetPrefix([Remainder]string Prefix)
		{
			var g = GetServer();
			g.Prefix = Prefix.Substring(0, 1);
			SaveServer(g);
			await ReplyAsync(Context.User.Mention + ", Prefix changed to `" + g.Prefix + "`.");
		}
		[Command("Server"),Alias("Guild")]
		[RequireContext(ContextType.Guild)]
		[Summary("Get the summary for this server.")]
		public async Task GetGuildSummary()
		{
			var g = GetServer();
			var embed = new EmbedBuilder()
				.WithThumbnailUrl(Context.Guild.IconUrl)
				.WithTitle(Context.Guild.Name + "'s summary")
				.WithCurrentTimestamp()
				.WithColor(new Color(114, 137, 218))
				.WithDescription(
					"Server Prefix: `" + g.Prefix + "`." + 
					"\nCharacters in this Server: " + Database.GetCollection<Character>("Characters").Find(x => x.Guild == Context.Guild.Id).Count()
				);
			await ReplyAsync("", false, embed.Build());
		}
	}
}
