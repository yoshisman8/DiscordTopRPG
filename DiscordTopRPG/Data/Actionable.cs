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
		public List<Property> Properties { get; set; }
		public List<Command> Macros { get; set; } = new List<Command>();


	}
}
