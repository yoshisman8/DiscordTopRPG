using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordTopRPG.Database
{
	public abstract class Actor
	{
		public string Name { get; set; }
		public Resource Stamina { get; set; } = new Resource() { Score = AbilityScore.Con };
		public Resource Pain { get; set; } = new Resource() { Score = AbilityScore.Int };
		public Resource Focus { get; set; } = new Resource() { Max = 3 };
		public Resource Burnout { get; set; } = new Resource() { Max = 3 };
		public Stat[] AbilityScores { get; set; } = new Stat[7];
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
	}
	public class Stat
	{
		public int Score { get { return Ranks + 1; } set { Score = value; } }
		public int Ranks { get; set; } = 0;
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
