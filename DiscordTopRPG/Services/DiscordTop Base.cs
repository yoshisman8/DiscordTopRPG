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
		public Player GetPlayer()
		{
			if (!Database.GetCollection<Player>("Players").Exists(x=>x.Id==Context.User.Id))
			{
				Database.GetCollection<Player>("Players").Insert(new Player { Id = Context.User.Id });
			}
			return	Database.GetCollection<Player>("Players").IncludeAll().FindOne(x => x.Id == Context.User.Id);
		}
		/// <summary>
		/// Saves a player file.
		/// </summary>
		/// <param name="player">The player file.</param>
		/// <returns>Whether or not the save was successful.</returns>
		public bool SavePlayer(Player player)
		{
			var col = Database.GetCollection<Player>("Players");
			if (!col.Exists(x => x.Id == player.Id))
			{
				col.Insert(player);
				return true;
			}
			else
			{
				return col.Update(player);
			}
		}
		/// <summary>
		/// Current Server. Returns null on non-guild contexts.
		/// </summary>
		public Server GetServer()
		{
			if (Context.Guild == null) return null;
			else
			{
				return Database.GetCollection<Server>("Servers")
					.FindOne(x => x.Id == Context.Guild.Id);
			}
		}
		/// <summary>
		/// Updates a Server in the Database
		/// </summary>
		/// <param name="database">The LiteDatabase</param>
		/// <returns>Returns False if it could not be saved.</returns>
		public bool SaveServer(Server server)
		{
			var col = Database.GetCollection<Server>("Servers");
			if (!col.Exists(x => x.Id == server.Id))
			{
				col.Insert(server);
				return true;
			}
			else
			{
				return col.Update(server);
			}
		}
		/// <summary>
		/// The current character file active on this server.
		/// </summary>
		public Character GetCharacter()
		{
			if (Context.Guild == null) return null;
			else
			{
				var player = GetPlayer();
				if (!player.ActiveCharacter.TryGetValue(Context.Guild.Id, out int c))
				{
					return null;
				}
				else
				{
					var cha = Database.GetCollection<Character>("Characters").FindOne(x => x.Id == c);
					return cha;
				}
			}
		}
		/// <summary>
		/// Saves or adds a characters to the database.
		/// </summary>
		/// <param name="database">The character file.</param>
		/// <returns>Whether or not the file was saved.</returns>
		public bool SaveCharacter(Character character)
		{
			var col = Database.GetCollection<Character>("Characters");
			if (!col.Exists(x => x.Id == character.Id))
			{
				col.Insert(character);
				return true;
			}
			else
			{
				return col.Update(character);
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
		public async Task<RestUserMessage> CreateMenu(Menu Menu,bool FromUser)
		{
			return await MenuService.CreateMenu(Context, Menu,FromUser);
		}
		public async Task SendPagedMenu(string Name, Embed[] Pages)
		{
			var menu = new PagedEmbed(Name, Pages);
			await MenuService.CreateMenu(Context, menu, false);
		}
		public async Task SendPlayerSheet(Character character)
		{
			var menu = new SheetMenu(character.Name, character.GetSheet(Context));
			await MenuService.CreateMenu(Context, menu, false);
		}
		public void DeleteCharacter(Character character)
		{
			var col = Database.GetCollection<Character>("Characters");
			col.Delete(character.Id);
			var players = Database.GetCollection<Player>("Players");
			var p = players.FindOne(x => x.Id == Context.User.Id);
			if(p.ActiveCharacter.TryGetValue(character.Guild,out int C))
			{
				if( C == character.Id)
				{
					p.SetActive(character.Guild,-1);
					players.Update(p);
				}
			}
		}
	}
	
}
