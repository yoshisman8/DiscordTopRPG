using Discord.Addon.InteractiveMenus;
using Discord.Commands;
using DiscordTopRPG.Database;
using DiscordTopRPG.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordTopRPG.Modules
{
	public class Skill_module : DiscordTopBase<SocketCommandContext>
	{
		#region Skills
		[Command("CustomSkill"), Alias("CreateCustomSkill", "NewSkill")]
		[RequireContext(ContextType.Guild)]
		public async Task NewSkill(string Name, AbilityScore Ability_Score)
		{
			var c = GetCharacter();
			if (c.GetSkills().Exists(x => x.Name.ToLower() == Name.ToLower()))
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
		[Command("DeleteSkill"), Alias("DeleteCustomSkill", "RemSkill")]
		[RequireContext(ContextType.Guild)]
		public async Task DelSkill([Remainder]string Skill)
		{
			var C = GetCharacter();
			var query = C.CustomSkills.Where(x => x.Name.StartsWith(Skill, StringComparison.CurrentCultureIgnoreCase)).OrderBy(x => x.Name).ToArray();
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
			if (prompt)
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
		[Command("RankUp"), Alias("SpendRanks", "SkillUp")]
		[RequireContext(ContextType.Guild)]
		public async Task UpSkill(Skill[] skill, int ranks)
		{
			ranks = Math.Abs(ranks);
			var c = GetCharacter();
			Skill s;
			if (skill.Length > 1)
			{
				var menu = new SelectorMenu("Multiple skills were found, please specify.", skill.Select(x => x.Name).ToArray(), skill);
				await MenuService.CreateMenu(Context, menu, true);
				s = (Skill)await menu.GetSelectedObject();
			}
			else s = skill[0];
			if (s.Ranks + ranks > c.Level)
			{
				await ReplyAsync("Skills cannot have more ranks than your character has levels.");
				return;
			}
			else if (c.GetRemainingRanks()-1 > 0)
			{
				await ReplyAsync("You don't have any more skill ranks. Consider using Upgrade Points to get the Skill Bonus upgrade");
				return;
			}
			else
			{

				string prompt = "Do you want to spend **" + ranks + "** Skill Ranks to increase your bonus to " + s.Name + " by +" + ranks + "?" +
				"\n```cs\nSkill Ranks " + c.GetRemainingRanks() + " ⇒ " + (c.GetRemainingRanks() - ranks) + "\n" +
				s.Name + " " + c.GetSkillBonus(s) + " ⇒ " + (ranks + c.GetSkillBonus(s)) + "```";

				var confirm = await SendConfirmationPrompt(prompt);
				if (!confirm)
				{
					await ReplyAsync(Context.User.Mention + ", Decided to not rank up.");
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
				await ReplyAsync(Context.User.Mention + ", Invested " + ranks + " ranks into skill **" + s.Name + "**.");
			}
		}
		[Command("RankDown"), Alias("GainRanks", "SkillDown")]
		[RequireContext(ContextType.Guild)]
		public async Task DownSkill(Skill[] skill, int ranks)
		{
			ranks = Math.Abs(ranks);
			var c = GetCharacter();
			Skill s;
			if (skill.Length > 1)
			{
				var menu = new SelectorMenu("Multiple skills were found, please specify.", skill.Select(x => x.Name).ToArray(), skill);
				await MenuService.CreateMenu(Context, menu, true);
				s = (Skill)await menu.GetSelectedObject();
			}
			else s = skill[0];
			if (s.Ranks - ranks < 0)
			{
				await ReplyAsync("This skill can't go any lower!");
				return;
			}
			else
			{

				string prompt = "Do you want to take back **" + ranks + "** Skill Ranks from your " + s.Name + " skill?"+
				"\n```cs\nSkill Ranks " + c.GetRemainingRanks()+ " ⇒ " + (c.GetRemainingRanks() + ranks) + "\n" +
				s.Name + " " + c.GetSkillBonus(s) + " ⇒ " + (ranks - c.GetSkillBonus(s)) + "```";

				var confirm = await SendConfirmationPrompt(prompt);
				if (!confirm)
				{
					await ReplyAsync(Context.User.Mention + ", Decided to not rank down.");
				}
				int I = 0;
				if (c.BaseSkills.Contains(s))
				{
					I = c.BaseSkills.IndexOf(s);
					c.BaseSkills[I].Ranks -= ranks;
				}
				else if (c.CustomSkills.Contains(s))
				{
					I = c.CustomSkills.IndexOf(s);
					c.CustomSkills[I].Ranks -= ranks;
				}
				SaveCharacter(c);
				await ReplyAsync(Context.User.Mention + ", Took back " + ranks + " ranks from skill **" + s.Name + "**.");
			}
		}
		#endregion
	}
}
