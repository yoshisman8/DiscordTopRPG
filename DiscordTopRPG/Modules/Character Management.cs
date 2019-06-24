using System;
using System.Collections.Generic;
using System.Text;
using DiscordTopRPG.Services;
using DiscordTopRPG.Database;
using Discord.Commands;
using LiteDB;
using System.Threading.Tasks;
using Discord.Addon.InteractiveMenus;

namespace DiscordTopRPG.Modules
{
	[Name("Character Management")] [Summary("Create, Delete, Rename and Manage characters.")]
	public class Character_Management : DiscordTopBase<SocketCommandContext>
	{
		[Command("Char")] [Alias("Character","Sheet")]
		[Summary("Shows your current active character.")]
		[RequireContext(ContextType.Guild)]
		public async Task CurrChar()
		{
			if(GetCharacter() == null)
			{
				await ReplyAsync(Context.User.Mention + ", You don't have an active character on this server.");
				return;
			}
			await SendPagedMenu(GetCharacter().Name+"'s Character Sheet",GetCharacter().GetSheet(Context));
		}
		[Command("Char")]
		[Alias("Character", "Sheet")]
		[Summary("Find and display a character on the server. If used through DMs it will look through all your created characters.")]
		public async Task GetCharr([Remainder]Character Character)
		{
			await SendPagedMenu(Character.Name + "'s Character Sheet", Character.GetSheet(Context));
		}
		[Command("CreateCharacter"),Alias("CreateChar","NewChar","NewCharacter","CC")]
		[RequireContext(ContextType.Guild)]
		public async Task NewCharG([Remainder]string Name)
		{
			var col = Database.GetCollection<Character>("Characters");
			var chr = new Character() { Name = Name, Owner = Context.User.Id, CreatedAt = DateTime.Now };
			chr.Id = col.Insert(chr);
			col.EnsureIndex("Name", "LOWER($.Name)");
			var p = GetPlayer();
			p.SetActive(Context.Guild.Id, chr);
			p.Save(Database);
			await ReplyAsync("Created **" + Name + "**'s character sheet. You have been granted 100 UP to start creating your character. This character has also been set as your active character.");
		}
		[Command("DeleteCharacter"),Alias("DelChar","RemChar","RemoveCharacer","DelCharacter","RemCharacter")]
		public async Task DelChar([Remainder]Character Character)
		{
			if (Character.Owner != Context.User.Id)
			{
				await ReplyAsync(Context.User.Mention + ", You do not own " + Character.Name + ".");
				return;
			}
			var menu = new SelectorMenu("Are you sure you want to delete " + Character.Name + "?", new string[] { "No", "Yes" }, new object[] { false, true });
			await CreateMenu(menu, true);
			bool confirmation = (bool)await menu.GetSelectedObject();
			if (confirmation)
			{
				var col = Database.GetCollection<Character>("Characters");
				col.Delete(Character.Id);
				await ReplyAsync(Context.User.Mention + ", you have successfully deleted " + Character.Name + "'s sheet.");
				return;
			}
			else
			{
				await ReplyAsync(Context.User.Mention + ", you have decided to keep " + Character.Name + "'s sheet.");
				return;
			}
		}
		[Command("RenameCharacter"),Alias("RenChar","RenameChar")]
		[Summary("Renames your current active character.")]
		[RequireContext(ContextType.Guild)]
		public async Task Rename([Remainder]string New_Name)
		{
			if (GetCharacter() == null)
			{
				await ReplyAsync(Context.User.Mention + ", You don't have an active character on this server.");
				return;
			}
			var c = GetCharacter();
			string old = c.Name;
			c.Name = New_Name;
			c.Save(Database);
			await ReplyAsync(Context.User.Mention + ", Renamed "+old+" to "+New_Name);
		}
	}
}
