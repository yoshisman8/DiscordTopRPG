using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using LiteDB;
using LiteDiscordIdentity;
using Microsoft.Extensions.ObjectPool;

namespace DiscordTopRPG.Data
{
	public class Character 
	{
		[BsonId]
		public int Id { get; set; }
		public bool Public { get; set; } = true;
		public ulong Owner { get; set; }
		public string Name { get; set; } = "Untitled Character";
		public bool Legacy { get; set; } = false;
		public Dictionary<string,Property> Properties { get; set; } = new Dictionary<string, Property>();

		public List<Command> Macros { get; set; } = new List<Command>();

		public List<Actionable> Attacks { get; set; } = new List<Actionable>();

		public List<Actionable> Abilities { get; set; } = new List<Actionable>();

		public List<Actionable> Items { get; set; } = new List<Actionable>();

		public Character() { }	
		public Character(bool Legacy)
		{
			this.Legacy = Legacy;
		}
	}
}
