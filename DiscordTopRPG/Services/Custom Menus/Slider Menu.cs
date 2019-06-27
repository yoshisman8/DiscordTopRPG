using Discord.Addon.InteractiveMenus;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Text;
using System.Linq;
using System;
using Discord.Rest;
using Discord.Commands;
using Discord;

namespace DiscordTopRPG.Services
{
	public class PointSpender : Menu
	{
		public Dictionary<string, int> Values;
		public string[] Names;
		public int Cost;
		public int Currency;
		public int Index = 0;
		public string Summary;
		public bool Active = true;
		public PointSpender(string message,string[]Options,int Cost,int Currency)
		{
			Summary = message;
			Names = Options;
			Values = new Dictionary<string, int>();
			foreach (var x in Names)
			{
				Values.Add(x,0);
			}
			this.Cost = Cost;
			this.Currency = Currency;
		}
		public async override Task<bool> HandleButtonPress(SocketReaction reaction)
		{
			if (!Buttons.TryGetValue(reaction.Emote, out Func<Task<bool>> Logic))
			{
				return false;
			}
			return await Logic();
		}
		public async override Task<RestUserMessage> Initialize(SocketCommandContext commandContext, MenuService menuService)
		{
			Message = await base.Initialize(commandContext, menuService);
			Buttons.Add(new Emoji("◀"), Decrease);
			Buttons.Add(new Emoji("🔼"), PrevOption);
			Buttons.Add(new Emoji("💾"), Save);
			Buttons.Add(new Emoji("🔽"), NextOption);
			Buttons.Add(new Emoji("▶"), Increase);
			await Message.AddReactionsAsync(Buttons.Select(x => x.Key).ToArray());
			await Refresh();
			return Message;
		}
		public async Task<bool> Decrease()
		{

			Values[Names[Index]]--;
			Currency += Cost;
			await Refresh();
			return false;
			
		}
		public async Task<bool> Increase()
		{
			if(Currency>=Cost)
			{
				Values[Names[Index]]++;
				Currency -= Cost;
				await Refresh();
			}
			return false;
		}
		public async Task<bool> PrevOption()
		{
			if (Index - 1 < 0) Index = Names.Length - 1;
			else Index--;
			await Refresh();
			return false;
		}
		public async Task<bool> NextOption()
		{
			if (Index + 1 >= Names.Length) Index = 0;
			else Index++;
			await Refresh();
			return false;
		}
		public async Task<bool> Save()
		{
			Active = false;
			return true;
		}
		public async Task Refresh()
		{
			if (Message.Content != " ") await Message.ModifyAsync(x => x.Content = " ");
			var embed = new EmbedBuilder()
				.WithColor(new Color(114, 137, 218))
				.WithDescription(Summary);
			var sb = new StringBuilder();
			foreach (var x in Values)
			{
				bool isindex = Names.ToList().IndexOf(x.Key) == Index;
				sb.AppendLine((isindex ? "> " : "") + x.Key + " (" + x.Value + ")" + (isindex ? " <" : ""));
			}
			embed.AddField("Points Remaining: "+Currency,sb.ToString());
			await Message.ModifyAsync(x => x.Embed = embed.Build());
		}
		public async Task<int[]> GetValues()
		{
			while (Active)
			{
				await Task.Delay(1000);
			}
			await Message.DeleteAsync();
			return Values != null?Values.Values.ToArray():null;
		}
	}
}
