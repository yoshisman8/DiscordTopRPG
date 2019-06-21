using System;
using System.Collections.Generic;
using System.Text;
using DiscordTopRPG.Services;
using DiscordTopRPG.Database;
using Discord.Commands;
using LiteDB;
using System.Threading.Tasks;

namespace DiscordTopRPG.Modules
{
	
	public class Character_Management : DiscordTopBase<SocketCommandContext>
	{
		[Command("CreateCharacter"),Alias("CreateChar","NewChar","NewCharacter","CC")]
		[RequireContext(ContextType.Guild)]
		public async Task NewCharG([Remainder]string Name)
		{
			if (Server.Characters.Exists(x=>x.Name.ToLower()==Name.ToLower()))
			{
				await ReplyAsync(Context.User.Mention + ", There's already a character in this server with that exact name.");
				return;
			}
			var chr = new Character() { Name = Name, Owner = Context.User.Id, CreatedAt = DateTime.Now };
			int id = Database.GetCollection<Character>("Characters").Insert(chr);
			chr.Id = id;
			Player.SetActive(Context.Guild.Id, chr);
			await ReplyAsync("Created **" + Name + "**'s character sheet. You have been granted 100 UP to start creating your character. This character has also been set as your active character.");
		}
	}
}
