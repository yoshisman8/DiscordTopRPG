using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace DiscordTopRPG.Database
{
	public class Player : Actor
	{
		[BsonId]
		public ulong Id { get; set; }
		public int UpgradePoints { get; set; }
		public int TotalUP { get; set; }
		public List<Talent> Talents { get; set; } = new List<Talent>();
		public List<RPTalent> RolePlayTalents { get; set; } = new List<RPTalent>();
		public List<Skill> CustomSkills { get; set; } = new List<Skill>();
		public Inventory Inventory { get; set; } = new Inventory();
	}
	public class Talent
	{
		[BsonRef("Effects")]
		public List<Effect> Effects { get; set; } = new List<Effect>();
		[BsonRef("Modifiers")]
		public List<Modifier> Modifiers { get; set; } = new List<Modifier>();
	}
	public class RPTalent
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public AbilityScore Score {get;set;}
		public int Ranks { get; set; }
	}
}
