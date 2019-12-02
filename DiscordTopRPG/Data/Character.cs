using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;

namespace DiscordTopRPG.Data
{
	public class Character 
	{
		[Key]
		public int Id { get; set; }
		public bool Public { get; set; } = true;
		public ulong Owner { get; set; }
		public string Name { get; set; } = "Untitled Character";
		public bool Legacy { get; set; } = false;
		public string SheetJSON { get; set; }

		[NotMapped]
		public List<Property> Properties { get; set; } = new List<Property>();

		[NotMapped]
		public List<Command> Macros { get; set; } = new List<Command>();

		[NotMapped]
		public List<Actionable> Attacks { get; set; } = new List<Actionable>();

		[NotMapped]
		public List<Actionable> Abilities { get; set; } = new List<Actionable>();

		[NotMapped]
		public List<Actionable> Items { get; set; } = new List<Actionable>();

		public void LoadAll()
		{
			var s = JsonConvert.DeserializeObject<Sheet>(SheetJSON);
			Properties = s.Properties;
			Macros = s.Macros;
			Attacks = s.Attacks;
			Abilities = s.Abilities;
			Items = s.Items;
		}
		public void Save()
		{
			var s = new Sheet();
			s.Properties = Properties;
			s.Macros = Macros;
			s.Attacks = Attacks;
			s.Abilities = Abilities;
			s.Items = Items;

			SheetJSON = JsonConvert.SerializeObject(s);
		}
	}
	public class Sheet {
		public List<Property> Properties { get; set; } = new List<Property>();
		public List<Command> Macros { get; set; } = new List<Command>();
		public List<Actionable> Attacks { get; set; } = new List<Actionable>();
		public List<Actionable> Abilities { get; set; } = new List<Actionable>();
		public List<Actionable> Items { get; set; } = new List<Actionable>();
	}
}
