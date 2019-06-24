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
		public string Prefix { get; set; } = "?";
		/// <summary>
		/// Updates this Server in the Database
		/// </summary>
		/// <param name="database">The LiteDatabase</param>
		/// <returns>Returns False if it could not be saved.</returns>
		public bool Save(LiteDatabase database)
		{
			return database.GetCollection<Server>("Servers").Update(this);
		}
	}
	public class Unlisted : Attribute { }
}
