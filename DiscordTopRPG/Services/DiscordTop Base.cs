using Discord;
using Discord.WebSocket;
using Discord.Addon.InteractiveMenus;
using Discord.Commands;
using Discord.Rest;
using DiscordTopRPG.Database;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordTopRPG.Services
{
	public class DiscordTopBase : DiscordTopBase<SocketCommandContext>
	{
	}

	public class DiscordTopBase<T> : ModuleBase <T>
		where T : SocketCommandContext
	{
		public LiteDatabase Database { get; set; }
		public MenuService MenuService { get; set; }
		public CommandHandlingService HandlingService { get; set; }

		/// <summary>
		/// Current Player entry of invoking user.
		/// </summary>
		public Player Player
		{
			get
			{
				if (!Database.GetCollection<Player>("Players").Exists(x=>x.Id==Context.User.Id))
				{
					Database.GetCollection<Player>("Players").Insert(new Player { Id = Context.User.Id });
				}
				return	Database.GetCollection<Player>("Players").IncludeAll().FindOne(x => x.Id == Context.User.Id);
			}
			set
			{
				Database.GetCollection<Player>("Players").Update(value);
			}
		}
		/// <summary>
		/// Current Server. Returns null on non-guild contexts.
		/// </summary>
		public Server Server
		{
			get
			{
				if (Context.Guild == null) return null;
				else
				{
					return Database.GetCollection<Server>("Servers")
						.IncludeAll()
						.FindOne(x => x.Id == Context.Guild.Id);
				}
			}
			set
			{
				var col = Database.GetCollection<Server>("Servers");
				col.Update(value);
			}
		}
		/// <summary>
		/// The current character file active on this server.
		/// </summary>
		public Character Character
		{
			get
			{
				if (Context.Guild == null) return null;
				else
				{
					var player= Database.GetCollection<Player>("Players")
						.IncludeAll()
						.FindOne(x => x.Id == Context.User.Id);
					if (!player.ActiveCharacter.TryGetValue(Context.Guild.Id, out Character c))
					{
						return null;
					}
					else return c;
				}
			}
			set
			{
				var col = Database.GetCollection<Character>("Characters");
				col.Update(value);
			}
		}
		public async Task<RestUserMessage> ReplyAsync(string content, bool isTTS = false,Embed embed = null)
		{
			if(HandlingService.CommandCache.TryGetValue(Context.Message.Id,out ulong Message))
			{
				var msg = (RestUserMessage)await Context.Channel.GetMessageAsync(Message);
				await msg.ModifyAsync(x => x.Content = content);
				await msg.ModifyAsync(x => x.Embed = embed);
				return msg;
			}
			else
			{
				var msg = await Context.Channel.SendMessageAsync(content, isTTS, embed);
				HandlingService.CommandCache.Add(Context.Message.Id, msg.Id);
				return msg;
			}
		}
	}
}
