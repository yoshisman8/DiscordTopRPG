using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord.Commands;
using Discord.Addon.InteractiveMenus;
using DiscordTopRPG.Database;
using DiscordTopRPG.Services;

namespace DiscordTopRPG.Modules
{
	public class Upgrade_Module : DiscordTopBase<SocketCommandContext>
	{
		#region Main Upgrades
		[Command("BuyStat"),Alias("IncreaseStat","UpgradeStat","GetAbilityScore")]
		[RequiresUP(5)] [RequireContext(ContextType.Guild)]
		public async Task UpgradeStat(AbilityScore Ability_Score, int Amount = 1)
		{
			var C = GetCharacter();
			switch (await SendConfirmationPrompt(
				"Do you want to spend **"+(Amount*10)+"** Upgrade Points to gain a +"+Amount+" to "+Ability_Score+"?"+
				"```cs\nUpgrade Points "+C.UpgradePoints+ " ⇒ "+(C.UpgradePoints-(5*Amount))+"\n"+
				Ability_Score+" "+C.AbilityScores[(int)Ability_Score].Get()+ " ⇒ "+ (C.AbilityScores[(int)Ability_Score].Get()+Amount)+"```"))
			{
				case true:
					C.UpgradePoints -= Amount * 5;
					C.AbilityScores[(int)Ability_Score].Investment += Amount;
					SaveCharacter(C);
					await ReplyAsync(Context.User.Mention + ", " + C.Name + "'s " + Ability_Score + " went " + (Amount > 0 ? "up" : "down") + " by " + Amount + ".");
					break;
				default:
					await ReplyAsync(Context.User.Mention + ", Decided to not spend the UP");
					break;
			}
		}

		[Command("BuyStamina"),Alias("IncreaseStamina")]
		[RequiresUP(3)] [RequireContext(ContextType.Guild)]
		public async Task BuyStamina(int Ranks)
		{
			var C = GetCharacter();
			string prompt = "Do you want to spend **" + (Ranks * 3) + "** Upgrade Points to gain +" + Ranks + " to Stamina?" +
				"\n```cs\nUpgrade Points " + C.UpgradePoints + " ⇒ " + (C.UpgradePoints - (3 * Ranks)) + "\n" +
				"Stamina "+C.GetResourceMax(C.Stamina)+ " ⇒ "+(Ranks+ C.GetResourceMax(C.Stamina))+"```";
			bool confirm = await SendConfirmationPrompt(prompt);
			switch(confirm)
			{
				case true:
					C.UpgradePoints -= Ranks*3;
					C.Stamina.Ranks += Ranks;
					SaveCharacter(C);
					await ReplyAsync(Context.User.Mention + ", " + C.Name + "'s stamina went " + (Ranks > 0 ? "up" : "down") + " by " + Ranks+ ".");
					break;
				default:
					await ReplyAsync(Context.User.Mention + ", Decided to not spend the UP");
					break;
			}
		}
		[Command("BuyFocus"),Alias("IncreaseFocus")]
		[RequiresUP(3)] [RequireContext(ContextType.Guild)]
		public async Task BuyFocus(int Ranks)
		{
			var C = GetCharacter();
			string prompt = "Do you want to spend **" + (Ranks * 3) + "** Upgrade Points to gain +" + Ranks + " to Focus?" +
				"\n```cs\nUpgrade Points " + C.UpgradePoints + " ⇒ " + (C.UpgradePoints - (3 * Ranks)) + "\n" +
				"Focus " + C.GetResourceMax(C.Stamina) + " ⇒ " + (Ranks + C.GetResourceMax(C.Focus)) + "```";
			bool confirm = await SendConfirmationPrompt(prompt);
			switch (confirm)
			{
				case true:
					C.UpgradePoints -= Ranks * 3;
					C.Focus.Ranks += Ranks;
					SaveCharacter(C);
					await ReplyAsync(Context.User.Mention + ", " + C.Name + "'s max focus went " + (Ranks > 0 ? "up" : "down") + " by " + Ranks + ".");
					break;
				default:
					await ReplyAsync(Context.User.Mention + ", Decided to not spend the UP");
					break;
			}
		}
		[Command("BuyPain"), Alias("IncreasePain")]
		[RequiresUP(4)]
		[RequireContext(ContextType.Guild)]
		public async Task BuyPain(int Ranks)
		{
			var C = GetCharacter();
			string prompt = "Do you want to spend **" + (Ranks * 4) + "** Upgrade Points to increase your maximum Pain by +" + Ranks + "?" +
				"\n```cs\nUpgrade Points " + C.UpgradePoints + " ⇒ " + (C.UpgradePoints - (4 * Ranks)) + "\n" +
				"Pain " + C.GetResourceMax(C.Pain) + " ⇒ " + (Ranks + C.GetResourceMax(C.Pain)) + "```";
			bool confirm = await SendConfirmationPrompt(prompt);
			switch (confirm)
			{
				case true:
					C.UpgradePoints -= Ranks * 4;
					C.Pain.Ranks += Ranks;
					SaveCharacter(C);
					await ReplyAsync(Context.User.Mention + ", " + C.Name + "'s max pain went "+(Ranks>0?"up":"down" )+" by " + Ranks + ".");
					break;
				default:
					await ReplyAsync(Context.User.Mention + ", Decided to not spend the UP");
					break;
			}
		}
		[Command("BuyBurnout"), Alias("IncreaseBurnout")]
		[RequiresUP(4)]
		[RequireContext(ContextType.Guild)]
		public async Task BuyBurnout(int Ranks)
		{
			var C = GetCharacter();
			string prompt = "Do you want to spend **" + (Ranks * 5) + "** Upgrade Points to increase your maximum Burnout by +" + Ranks + "?" +
				"\n```cs\nUpgrade Points " + C.UpgradePoints + " ⇒ " + (C.UpgradePoints - (4 * Ranks)) + "\n" +
				"Burnout " + C.GetResourceMax(C.Burnout) + " ⇒ " + (Ranks + C.GetResourceMax(C.Burnout)) + "```";
			bool confirm = await SendConfirmationPrompt(prompt);
			switch (confirm)
			{
				case true:
					C.UpgradePoints -= Ranks * 4;
					C.Burnout.Ranks += Ranks;
					SaveCharacter(C);
					await ReplyAsync(Context.User.Mention + ", " + C.Name + "'s max Burnout went " + (Ranks > 0 ? "up" : "down") + " by " + Ranks + ".");
					break;
				default:
					await ReplyAsync(Context.User.Mention + ", Decided to not spend the UP");
					break;
			}
		}
		#endregion

		
	}
}
