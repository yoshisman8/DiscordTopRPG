using Discord.Commands;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB;
using DiscordTopRPG.Database;
using Discord.Addon.InteractiveMenus;

namespace DiscordTopRPG.Services
{
	public class TypeReaders
	{
		private CommandService CommandService;
		public TypeReaders(CommandService command)
		{
			CommandService = command;
			CommandService.AddTypeReader<Character[]>(new CharacterTypeReader());
			CommandService.AddTypeReader<Character>(new CharacterReader());
		}
	}
	public class CharacterTypeReader : TypeReader
	{
		public async  override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
		{
			LiteDatabase database = (LiteDatabase)services.GetService(typeof(LiteDatabase));
			if(context.Guild== null)
			{
				var Pcol = database.GetCollection<Player>("Players");
				var player = Pcol.FindOne(x => x.Id == context.User.Id);
				if (player == null)
				{
					Pcol.Insert(new Player() { Id = context.User.Id });
					return TypeReaderResult.FromError(CommandError.ObjectNotFound, "You have no characters to your name.");
				}
			}
			else
			{
				var guild = database.GetCollection<Server>("Servers").IncludeAll().FindOne(x=>x.Id==context.Guild.Id);
				var results = guild.Characters.Where(x => x.Name.StartsWith(input, StringComparison.InvariantCultureIgnoreCase)).ToArray();
				if (results.Length == 0) return TypeReaderResult.FromError(CommandError.ObjectNotFound, "No characters were found");
				else return TypeReaderResult.FromSuccess(results);
			}
			return TypeReaderResult.FromError(CommandError.ObjectNotFound,"No characters were found");
		}
	}
	public class CharacterReader : TypeReader
	{
		public async override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
		{
			LiteDatabase database = (LiteDatabase)services.GetService(typeof(LiteDatabase));
			if (context.Guild == null)
			{
				var Pcol = database.GetCollection<Player>("Players");
				var player = Pcol.FindOne(x => x.Id == context.User.Id);
				if (player == null)
				{
					Pcol.Insert(new Player() { Id = context.User.Id });
					return TypeReaderResult.FromError(CommandError.ObjectNotFound, "You have no characters to your name.");
				}
			}
			else
			{
				var guild = database.GetCollection<Server>("Servers").IncludeAll().FindOne(x => x.Id == context.Guild.Id);
				var results = guild.Characters.Where(x => x.Name.StartsWith(input, StringComparison.InvariantCultureIgnoreCase)).ToArray();
				if (results.Length == 0) return TypeReaderResult.FromError(CommandError.ObjectNotFound, "No characters were found");
				else if (results.Length > 1 && context.GetType()==typeof(SocketCommandContext))
				{
					var MenuService = (MenuService)services.GetService(typeof(MenuService));
					var menu = new SelectorMenu("Mutiple Characters were found, please pick one:", results.Select(x => x.Name).ToArray(), results);
					await MenuService.CreateMenu((SocketCommandContext)context, menu, true);
					var picked = (Character)await menu.GetSelectedObject();
					return TypeReaderResult.FromSuccess(picked);
				}
				else return TypeReaderResult.FromSuccess(results[0]);
			}
			return TypeReaderResult.FromError(CommandError.ObjectNotFound, "No characters were found");
		}
	}
}
