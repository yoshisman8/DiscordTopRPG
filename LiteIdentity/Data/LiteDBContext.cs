using System;
using System.Linq;
using LiteDB;
using Microsoft.Extensions.Configuration;

namespace LiteDiscordIdentity
{
	public class LiteDbContext : ILiteDbContext
	{
		public LiteDbContext(IConfiguration configuration)
		{
			string connectionString;
			try
			{
				connectionString = configuration.GetConnectionString("LiteDb");
			}
			catch (NullReferenceException)
			{
				throw new NullReferenceException("No connection string defined in appsettings.json");
			}

			LiteDatabase = new LiteDatabase(connectionString);
		}

		public LiteDbContext(LiteDatabase liteDatabase)
		{
			LiteDatabase = liteDatabase;
		}

		public LiteDatabase LiteDatabase { get; protected set; }

	}
}