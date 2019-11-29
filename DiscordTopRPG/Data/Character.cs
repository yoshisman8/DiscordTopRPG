using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTopRPG.Data
{
	public class Character 
	{
		[Key]
		public int Id { get; set; }
		public bool Public { get; set; }
		[ForeignKey("ApplicationUser")]
		public string ApplicationUserId { get; set; }
		public string Name { get; set; }
		/// <summary>
		/// All of the numbers that the system tracks
		/// ie: ability scores, hp, armor, etc
		/// </summary>
		public List<Property> Properties { get; set; } = new List<Property>();
		/// <summary>
		/// Individual actions that can be called by the user 
		/// Mostly there to be pre-populated by the template with basic rolls
		/// And for the user to make their own special rolls
		/// </summary>
		public List<Command> Macros { get; set; } = new List<Command>();
		/// <summary>
		/// List of attacks for quick rolling.
		/// </summary>
		public List<Actionable> Attacks { get; set; } = new List<Actionable>();
		public List<Actionable> Abilities { get; set; } = new List<Actionable>();
	}
}
