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
		public Dictionary<ulong, int> ActiveCharacter { get; set; } = new Dictionary<ulong, int>();
		public void SetActive(ulong Server, int character)
		{
			if(ActiveCharacter.ContainsKey(Server))
			{
				ActiveCharacter[Server] = character;
			}
			else
			{
				ActiveCharacter.Add(Server, character);
			}
		}
	}
}
