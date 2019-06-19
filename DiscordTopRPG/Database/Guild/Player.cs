using System;
using LiteDB;
using System.Collections.Generic;
using System.Text;

namespace DiscordTopRPG.Database
{
	public class Player
	{
		[BsonId]
		public ulong Id { get; set; }
		[BsonRef("Characters")]
		public Dictionary<ulong, Character> ActiveCharacter { get; set; } = new Dictionary<ulong, Character>();
	}
}
