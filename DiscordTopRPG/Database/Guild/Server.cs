using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;

namespace DiscordTopRPG.Database
{
	public class Server
	{
		[BsonId]
		public ulong Id { get; set; }
		public string Prefix { get; set; }
		[BsonRef("Players")]
		public List<Player> Players { get; set; }
		[BsonRef("Characters")]
		public List<Character> Characters { get; set; }
	}
	public class Unlisted : Attribute { }
}
