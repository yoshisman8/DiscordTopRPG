using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTopRPG.Data
{
	public class Actionable 
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public Dictionary<string, Property> Properties { get; set; } = new Dictionary<string, Property>();
		public List<Command> Macros { get; set; } = new List<Command>();
		public List<Modifier> Modifiers { get; set; } = new List<Modifier>();
		public string[] Tags { get; set; } 
	}
}
