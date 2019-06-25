using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.Commands;
using LiteDB;

namespace DiscordTopRPG.Database
{
	public class Character 
	{
		[BsonId]
		public int Id { get; set; }
		public string Name { get; set; }
		public ulong Owner { get; set; }
		public ulong Guild { get; set; }
		public string Icon { get; set; } = "https://image.flaticon.com/icons/png/512/64/64096.png";
		public string Bio { get; set; } = null;
		public int UpgradePoints { get; set; } = 100;
		public int TotalUP { get; set; } = 0;
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public Inventory Inventory { get; set; } = new Inventory();
		public Resource Stamina { get; set; } = new Resource() { Score = AbilityScore.Con };
		public Resource Pain { get; set; } = new Resource() { Score = AbilityScore.Int };
		public Resource Focus { get; set; } = new Resource() { Max = 3 };
		public Resource Burnout { get; set; } = new Resource() { Max = 3 };
		public Stat[] AbilityScores { get; set; } = new Stat[7]
			{
				new Stat(),
				new Stat(),
				new Stat(),
				new Stat(),
				new Stat(),
				new Stat(),
				new Stat()
			};
		public Skill Fortitude { get; set; } = new Skill() { Name = "Fortitude", Score = AbilityScore.Str };
		public Skill WillPower { get; set; } = new Skill() { Name = "Willpower", Score = AbilityScore.Cha };
		public List<Skill> BaseSkills { get; set; } = new List<Skill>()
		{
			new Skill(){Name="Aim",Score=AbilityScore.Dex},
			new Skill(){Name="Acrobatics",Score=AbilityScore.Agi},
			new Skill(){Name="Athletics",Score=AbilityScore.Str},
			new Skill(){Name="Block",Score=AbilityScore.Str},
			new Skill(){Name="Crush",Score=AbilityScore.Str},
			new Skill(){Name="Diplomacy",Score=AbilityScore.Cha},
			new Skill(){Name="Evade",Score=AbilityScore.Agi},
			new Skill(){Name="Heal",Score=AbilityScore.Int},
			new Skill(){Name="Intimidate",Score=AbilityScore.Cha},
			new Skill(){Name="Perception",Score=AbilityScore.Int},
			new Skill(){Name="Recall",Score=AbilityScore.Mem},
			new Skill(){Name="Ride",Score=AbilityScore.Agi},
			new Skill(){Name="Sense Motive",Score=AbilityScore.Int},
			new Skill(){Name="Slash",Score=AbilityScore.Str},
			new Skill(){Name="Stab",Score=AbilityScore.Str},
			new Skill(){Name="Stealth",Score=AbilityScore.Agi},
			new Skill(){Name="Survival",Score=AbilityScore.Int},
			new Skill(){Name="Throw",Score=AbilityScore.Str},
		};
		public List<Skill> CustomSkills { get; set; } = new List<Skill>();
	
		public List<Skill> GetSkills()
		{
			return BaseSkills.Concat(CustomSkills).ToList();
		}

		public Embed[] GetSheet(SocketCommandContext Context)
		{
			var user = Context.Client.GetUser(Owner);
			var page1 = new EmbedBuilder()
				.WithColor(new Color(114, 137, 218))
				.WithTitle(this.Name)
				.WithTimestamp(this.CreatedAt)
				.WithFooter(user.Username ?? "Someone Unkown", user.GetAvatarUrl() ?? null)
				.WithDescription(Bio ?? "No Bio")
				.WithThumbnailUrl(Icon)
				.AddField("Ability Scores", "```css\nStrength    [" + AbilityScores[0].Get() + "]\nDexterity   [" + AbilityScores[1].Get() + "]\nAgility     [" + AbilityScores[2].Get() + "]\nConstitution[" + AbilityScores[3].Get() + "]\nMemory      [" + AbilityScores[4].Get() + "]\nIntution    [" + AbilityScores[5].Get() + "]\nCharisma    [" + AbilityScores[6].Get() + "]```", true)
				.AddField("Stats", "```css\nStamina [" + Stamina.Current + "/" + Stamina.Max + "]\nPain [" + Pain.Current + "/" + Pain.Max + "]\nFortitude   " + GetSkillBonus(Fortitude).ToString("+#;-#;0") + "\nUpgrade Pts " + UpgradePoints + "\nWillpower " + GetSkillBonus(WillPower).ToString("+#;-#;0") + "\nFocus [" + Focus.Current + "/" + Focus.Max + "]\nBurnout [" + Burnout.Current + "/" + Burnout.Max + "]```", true);
			var sb = new StringBuilder().AppendLine("```md\nName                      Ranks    Total");
			sb.AppendLine("========================================");
			foreach (var x in GetSkills())
			{
				// Name is max 27 chars
				string name = string.Format("{0,-25}", x.Name + " <" + (AblityShort)x.Score + ">");
				string ranks = string.Format("{0, -5}", x.Ranks);
				string total = string.Format("{0,9}", GetSkillBonus(x).ToString("+#;-#;0"));
				sb.AppendLine(name + ranks + total);
			}
			page1.AddField("Skills", sb.ToString()+"```");
			sb.Clear();

			var page2 = new EmbedBuilder()
				.WithTitle(this.Name + "'s Inventory")
				.WithTimestamp(this.CreatedAt)
				.WithThumbnailUrl("https://static.thenounproject.com/png/2551-200.png")
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
			foreach (var x in Inventory.Items().OrderBy(x => x.Name))
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
			int sc = AbilityScores[(int)skill.Score].Get();
			if ((int)skill.Score == (int)AbilityScore.Agility)
			{
				int pen = Inventory.Worn.Select(x => x.Penalty).Sum();
				return skill.Ranks + sc + pen;
			}
			return skill.Ranks + sc;
		}
		
	}
	public class Stat
	{
		public int Investment { get; set; } = 0;
		public int Get() => 1+Investment;
	}
	public class Resource
	{
		public int Max { get; set; }
		public int Current { get; set; }
		public int Ranks { get; set; } = 0;
		public AbilityScore Score { get; set; } = AbilityScore.None;
	}
	public class Skill
	{
		public string Name { get; set; }
		public int Ranks { get; set; }
		public AbilityScore Score { get; set; }
	}
	public enum AbilityScore
	{
		None = -1,
		Strength = 0,
		Str = 0,
		Dexterity = 1,
		Dex = 1,
		Agility = 2,
		Agi = 2,
		Constitution = 3,
		Con = 3,
		Memory = 4,
		Mem = 4,
		Intution = 5,
		Int = 5,
		Charisma = 6,
		Cha = 6
	}
	public enum AblityShort { None = -1, Str, Dex, Agi, Con, Mem, Int, Cha }
}
