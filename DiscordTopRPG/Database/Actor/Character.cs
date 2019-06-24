using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.Commands;
using LiteDB;

namespace DiscordTopRPG.Database
{
	public class Character : Actor
	{
		[BsonId]
		public int Id { get; set; }
		public ulong Owner { get; set; }
		public ulong Guild { get; set; }
		public string Bio { get; set; } = null;
		public int UpgradePoints { get; set; } = 100;
		public int TotalUP { get; set; } = 0;
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public List<Skill> CustomSkills { get; set; } = new List<Skill>();
		public Inventory Inventory { get; set; } = new Inventory();
		public List<Skill> Skills
		{
			get
			{
				return BaseSkills.Concat(CustomSkills).ToList();
			}
			set
			{
				Skills = value;
			}
		}
		public Embed[] GetSheet(SocketCommandContext Context)
		{
			var user = Context.Client.GetUser(Owner);
			var page1 = new EmbedBuilder()
				.WithColor(new Color(114,137,218))
				.WithTitle(this.Name)
				.WithTimestamp(this.CreatedAt)
				.WithFooter(user.Username ?? "Someone Unkown", user.GetAvatarUrl() ?? null)
				.WithDescription(Bio)
				.AddField("Ability Scores", "```css\n💪 Strength     [" + AbilityScores[0].Score + "]\n👋 Dexterity    [" + AbilityScores[1].Score + "]\n🤸 Agility      [" + AbilityScores[2].Score + "]\n💗 Constitution [" + AbilityScores[3].Score + "]\n📖 Memory       [" + AbilityScores[4].Score + "]\n🧠 Intution     [" + AbilityScores[5].Score + "]\n👥 Charisma     [" + AbilityScores[6].Score + "]```", true)
				.AddField("Stats", "```css\nStamina     [" + Stamina.Current + "/" + Stamina.Max + "]\nPain        [" + Pain.Current + "/" + Pain.Max + "]\nFortitude    " + GetSkillBonus(Fortitude).ToString("+#;-#;0") + "\nUpgrade Pts  " + UpgradePoints + "\nWillpower    " + GetSkillBonus(WillPower).ToString("+#;-#;0") + "\nFocus       [" + Focus.Current + "/" + Focus.Max + "]\nBurnout     [" + Burnout.Current + "/" + Burnout.Max + "]```", true);
			var sb = new StringBuilder().AppendLine("Name                      Ranks    Total");
			sb.AppendLine("========================================");
			foreach (var x in Skills)
			{
				// Name is max 27 chars
				string name = string.Format("{0,-26}", x.Name + "(" + (AblityShort)x.Score + ")");
				string ranks = string.Format("{0," + ((10 + x.Ranks.ToString().Length) / 2).ToString() + "}", x.Ranks);
				string total = string.Format("{0,4}", GetSkillBonus(x).ToString("+#;-#;0"));
				sb.AppendLine(name + ranks + total);
			}
			var page2 = new EmbedBuilder()
				.WithTitle(this.Name + "'s Inventory")
				.WithTimestamp(this.CreatedAt)
				.WithColor(new Color(114,137,218))
				.WithFooter(user.Username ?? "Someone Unkown", user.GetAvatarUrl() ?? null)
				.WithDescription("Money: $" + Inventory.Money.ToString("F"));
			if (Inventory.Worn.Count > 0)
			{
				sb.Clear();
				foreach (var x in Inventory.Worn)
				{
					sb.AppendLine("[" + x.Slot + "] " + x.Name + " (" + x.Block.ToString("+#;*#;0") + " Block)" + " (" + x.Penalty.ToString("+#;*#;0") + " Agility)");
				}
				if (Inventory.Weapon != null)
				{
					sb.Append("[Weapon] " + Inventory.Weapon.Name + "(" + Inventory.Weapon.DamageDice + "+ " + (AblityShort)Inventory.Weapon.Score + ")");
				}
			}
			if (sb.Length > 0) page2.AddField("Worn Gear", sb.ToString());
			sb.Clear();
			foreach (var x in Inventory.Items.OrderBy(x => x.Name))
			{
				if (x.GetType() == typeof(Wearable))
				{
					var y = (Wearable)x;
					sb.AppendLine("• [" + y.Slot + "] " + y.Name + " (" + y.Block.ToString("+#;*#;0") + " Block)" + " (" + y.Penalty.ToString("+#;*#;0") + " Agility)");
				}
				else if (x.GetType() == typeof(Weapon))
				{
					var y = (Weapon)x;
					sb.AppendLine("• " + y.Name + " (" + y.DamageDice + " + " + (AblityShort)y.Score + ")");
				}
				else if (x.GetType() == typeof(Consumable))
				{
					var y = (Consumable)x;
					sb.AppendLine("• " + y.Name + " x" + y.Quantity);
				}
				else
				{
					sb.AppendLine("• " + x.Name);
				}
			}
			if (sb.Length > 0) page2.AddField("Items", sb.ToString());

			return new Embed[] { page1.Build(), page2.Build() };
		}
		public int GetSkillBonus(Skill skill)
		{
			int sc = AbilityScores[(int)skill.Score].Score;
			if ((int)skill.Score == (int)AbilityScore.Agility)
			{
				int pen = Inventory.Worn.Select(x => x.Penalty).Sum();
				return skill.Ranks + sc + pen;
			}
			return skill.Ranks + sc;
		}
		public bool Save(LiteDatabase database)
		{
			var col = database.GetCollection<Character>("Characters");
			return col.Update(this);
		}
	}
}
