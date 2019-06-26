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

		#region Skills
		[Command("CustomSkill"),Alias("CreateCustomSkill","NewSkill")]
		[RequiresUP(1)] [RequireContext(ContextType.Guild)]
		public async Task NewSkill(string Name, AbilityScore Ability_Score)
		{
			var c = GetCharacter();
			if (c.GetSkills().Exists(x=>x.Name.ToLower()==Name.ToLower()))
			{
				await ReplyAsync(Context.User.Mention + ", You already have a skill named " + Name + ".");
				return;
			}
			Skill S = new Skill() { Name = Name, Score = Ability_Score, Ranks = 1 };
			c.UpgradePoints -= 1;
			c.CustomSkills.Add(S);
			SaveCharacter(c);
			await ReplyAsync("You have created the custom skill **" + Name + "**.");
		}
		[Command("DeleteSkill"),Alias("DeleteCustomSkill","RemSkill")]
		[RequireContext(ContextType.Guild)]
		public async Task DelSkill([Remainder]string Skill)
		{
			var C = GetCharacter();
			var query = C.CustomSkills.Where(x => x.Name.StartsWith(Skill, StringComparison.CurrentCultureIgnoreCase)).OrderBy(x=>x.Name).ToArray();
			Skill S;
			if (query.Length == 0)
			{
				await ReplyAsync(Context.User.Mention + ", " + C.Name + " doesn't have any custom skill whose name starts with \"" + Skill + "\".");
				return;
			}
			else if (query.Length > 1)
			{
				var menu = new SelectorMenu("Multiple Skills were found, please specify which one want to remove.", query.Select(x => x.Name).ToArray(), query);
				await MenuService.CreateMenu(Context, menu, true);
				S = (Skill)await menu.GetSelectedObject();
			}
			else S = query[0];

			bool prompt = await SendConfirmationPrompt("Are you sure you want to delete the custom skill \"" + S.Name + "\"?\nYou will be refunded all the UP you have invested on this skill.");
			if(prompt)
			{
				C.CustomSkills.Remove(S);
				C.UpgradePoints += S.Ranks;
				SaveCharacter(C);
				await ReplyAsync(Context.User.Mention + ", Deleted custom skill **" + S.Name + "**.");
			}
			else
			{
				await ReplyAsync(Context.User.Mention + ", Kept custom skill.");
			}
		}
		[Command("UpgradeSkill"), Alias("InvestSkill", "RankUp")]
		[RequireContext(ContextType.Guild)]
		[RequiresUP(1)]
		public async Task UpSkill(Skill[] skill, int ranks)
		{
			var c = GetCharacter();
			Skill s;
			if (skill.Length > 1)
			{
				var menu = new SelectorMenu("Multiple skills were found, please specify.", skill.Select(x => x.Name).ToArray(), skill);
				await MenuService.CreateMenu(Context, menu, true);
				s = (Skill)await menu.GetSelectedObject();
			}
			else s = skill[0];
			if (s.Ranks + ranks > (c.AbilityScores[(int)s.Score].Get() * 2))
			{
				await ReplyAsync("A skill's ranks cannot surpass twice the bonus from its ability score. You have to increase your character's " + s.Score + " if you want to invest more ranks into " + s.Name);
				return;
			}
			else if (c.AbilityScores[(int)s.Score].Get() < 1 && s.Ranks + ranks > 2)
			{
				await ReplyAsync("You cannot invet more than 2 ranks into a skill with a negative ability score.");
				return;
			}
			else
			{

				string prompt = "Do you want to spend **" + ranks + "** Upgrade Points to increase your bonus to "+s.Name+" by +" + ranks + "?" +
				"\n```cs\nUpgrade Points " + c.UpgradePoints + " ⇒ " + (c.UpgradePoints - ranks) + "\n" +
				s.Name+": " + c.GetSkillBonus(s) + " ⇒ " + (ranks+ c.GetSkillBonus(s)) + "```";

				var confirm = await SendConfirmationPrompt(prompt);
				if(!confirm)
				{
					await ReplyAsync(Context.User.Mention + ", Decided to not invest the UP.");
				}
				int I = 0;
				if (c.BaseSkills.Contains(s))
				{
					I = c.BaseSkills.IndexOf(s);
					c.BaseSkills[I].Ranks += ranks;
				}
				else if (c.CustomSkills.Contains(s))
				{
					I = c.CustomSkills.IndexOf(s);
					c.CustomSkills[I].Ranks += ranks;
				}
				SaveCharacter(c);
				await ReplyAsync(Context.User.Mention + ", Invested " + ranks + " into skill **" + s.Name + "**.");
			}
		}
		#endregion
	}
}
