using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Addon.InteractiveMenus;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace DiscordTopRPG.Services
{
	public class SheetMenu : Menu
	{
		private int Index = 0;
		private Embed[] Sheet;
		private string Name;
		private bool Minimized { get; set; } = false;
		public SheetMenu(string CharacterName,Embed[] Pages)
		{
			Sheet = Pages;
			Name = CharacterName;
		}
		public async override Task<RestUserMessage> Initialize(SocketCommandContext commandContext, MenuService menuService)
		{
			Message = await base.Initialize(commandContext, menuService);
			Buttons.Add(new Emoji("🔄"), FlipSheet);
			Buttons.Add(new Emoji("⏯"), Minimize);
			await Message.AddReactionsAsync(Buttons.Select(x=>x.Key).ToArray());
			await Refresh();
			return Message;
		}
		public override async Task<bool> HandleButtonPress(SocketReaction reaction)
		{
			if (!Buttons.TryGetValue(reaction.Emote, out Func<Task<bool>> Logic))
			{
				return false;
			}
			return await Logic();
		}
		public async Task<bool> Minimize()
		{
			Minimized ^= true;
			await Refresh();
			return false;
		}
		public async Task<bool> FlipSheet()
		{
			switch (Index)
			{
				case 0:
					Index = 1;
					break;
				case 1:
					Index = 0;
					break;
			}
			await Refresh();
			return false;
		}
		public async Task Refresh()
		{
			await Message.ModifyAsync(x => x.Content = " ");
			if (Minimized)
			{
				await Message.ModifyAsync(x => x.Embed = new EmbedBuilder().WithDescription("Minimized " + Name + "'s Sheet.").Build());
			}
			else
			{
				await Message.ModifyAsync(x => x.Embed = Sheet[Index]);
			}
		}
	}
}
