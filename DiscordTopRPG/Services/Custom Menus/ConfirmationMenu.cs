using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Addon.InteractiveMenus;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Rest;
using System.Linq;

namespace DiscordTopRPG.Services
{
	public class ConfirmationMenu : Menu
	{
		private bool Confirmation;
		private bool Active = true;
		string Prompt;
		public ConfirmationMenu(string Message)
		{
			Prompt = Message;
		}
		public async override Task<RestUserMessage> Initialize(SocketCommandContext commandContext, MenuService service)
		{
			Message = await base.Initialize(commandContext, service);
			await Message.ModifyAsync(x => x.Content = Prompt);
			Buttons.Add(new Emoji("✅"), Confirm);
			Buttons.Add(new Emoji("⛔"), Reject);
			await Message.AddReactionsAsync(Buttons.Select(x => x.Key).ToArray());
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
		public async Task<bool> Confirm()
		{
			Confirmation = true;
			Active = false;
			return true;
		}
		public async Task<bool> Reject()
		{
			Confirmation = false;
			Active = false;
			return true;
		}
		public async Task<bool> GetSelection()
		{
			while (Active)
			{
				await Task.Delay(1000);
			}
			await Message.DeleteAsync();
			return Confirmation;
		}
	}
}
