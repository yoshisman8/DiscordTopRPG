using System;
using System.Collections.Generic;
using System.Text;
using DiscordTopRPG.Services;
using DiscordTopRPG.Database;
using Discord.Commands;
using LiteDB;
using System.Threading.Tasks;
using Discord.Addon.InteractiveMenus;
using System.Linq;

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
			var c = GetCharacter();
			if (c == null)
			{
				await ReplyAsync(Context.User.Mention + ", You don't have an active character on this server.");
				return;
			}
			await SendPlayerSheet(GetCharacter());
		}
		[Command("Char")]
		[Alias("Character", "Sheet")]
		[Summary("Find and display a character on the server. If used through DMs it will look through all your created characters.")]
		public async Task GetCharr([Remainder]Character[] Character)
		{
			if(Character.Length > 1)
			{
				List<string> names = new List<string>();
				foreach (var x in Character)
				{
					var owner = Context.Client.GetUser(x.Owner);
					names.Add(x.Name + " (By: " +(owner.Username??"Somebody Unknown")+")");
				}
				var menu = new SelectorMenu("Mutiple Characters were found, please pick one:", names.ToArray(), Character);
				await MenuService.CreateMenu(Context, menu, true);
				var picked = (Character)await menu.GetSelectedObject();
				await SendPlayerSheet(picked);
			}
			else await SendPlayerSheet(Character[0]);
		}
		[Command("CreateCharacter"),Alias("CreateChar","NewChar","NewCharacter","CC")]
		[RequireContext(ContextType.Guild)]
		public async Task NewCharG([Remainder]string Name)
		{
			// Get the character collection
			var col = Database.GetCollection<Character>("Characters");

			// Creates new Character file allows duplicate names since selector handles multiple results.
			var chr = new Character() { Name = Name, Owner = Context.User.Id, CreatedAt = DateTime.Now, Guild = Context.Guild.Id };
			// Adds character to the database, returns id
			chr.FullResture();
			var id = col.Insert(chr);
			chr = col.FindOne(x=>x.Id==id.AsInt32);
			// Index stuff
			col.EnsureIndex("Name", "LOWER($.Name)");
			col.EnsureIndex(x => x.Guild);

			// Get the active player data and set this character as the active one for this server.
			var p = GetPlayer();
			p.SetActive(Context.Guild.Id, chr.Id);

			// Save the character
			SavePlayer(p);
			await ReplyAsync("Created **" + Name + "**'s character sheet. You have been granted 50 Upgrade Points to start creating your character. This character has also been set as your active character.");
		}
		[Command("DeleteCharacter"),Alias("DelChar","RemChar","RemoveCharacer","DelCharacter","RemCharacter")]
		public async Task DelChar([Remainder]Character[] Character)
		{
			Character picked;
			if (Character.Length > 1)
			{
				List<string> names = new List<string>();
				foreach (var x in Character)
				{
					var owner = Context.Client.GetUser(x.Owner);
					names.Add(x.Name + " (By: " + (owner.Username ?? "Somebody Unknown") + ")");
				}
				var menu = new SelectorMenu("Mutiple Characters were found, please pick one:", names.ToArray(), Character);
				await MenuService.CreateMenu(Context, menu, true);
				picked = (Character)await menu.GetSelectedObject();
				await SendPlayerSheet(picked);
			}
			else picked = Character[0];
			if (picked.Owner != Context.User.Id)
			{
				await ReplyAsync(Context.User.Mention + ", You do not own " + picked.Name + ".");
				return;
			}
			var confirmation = await SendConfirmationPrompt("Are you sure you want to delete " + picked.Name + "?");
			if (confirmation)
			{
				var col = Database.GetCollection<Character>("Characters");
				DeleteCharacter(picked);
				await ReplyAsync(Context.User.Mention + ", you have successfully deleted " + picked.Name + "'s sheet.");
				return;
			}
			else
			{
				await ReplyAsync(Context.User.Mention + ", you have decided to keep " + picked.Name + "'s sheet.");
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
			SaveCharacter(c);
			await ReplyAsync(Context.User.Mention + ", Renamed "+old+" to "+New_Name);
		}
	}
}
