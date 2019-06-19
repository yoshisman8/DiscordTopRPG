using Discord.Addon.InteractiveMenus;
using Discord.Commands;
using DiscordTopRPG.Database;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordTopRPG.Services
{
	public class DiscordTopBase<T> : ModuleBase
		where T : SocketCommandContext
	{
		public LiteDatabase Database { get; set; }
		public MenuService MenuService { get; set; }
		public Character Character
		{
			get
			{
				if (Context.Guild == null) return null;
				else
				{
					var player= Database.GetCollection<Player>("Players").FindOne(x => x.Id == Context.User.Id);
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
		public DiscordTopBase(T context)
		{
			var col = Database.GetCollection<Server>("Servers");
			
		}
	}
}
