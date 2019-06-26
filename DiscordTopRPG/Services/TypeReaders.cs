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
			
		}
	}
	public class RequiresUP : PreconditionAttribute
	{
		private int UP;
		public RequiresUP(int Amount)
		{
			UP = Amount;
		}
		public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			var col = ((LiteDatabase)services.GetService(typeof(LiteDatabase))).GetCollection<Player>("Players");
			var player = col.IncludeAll().FindOne(x => x.Id == context.User.Id);
			if (player == null)
			{
				col.Insert(new Player() { Id = context.User.Id });
				player = col.IncludeAll().FindOne(x => x.Id == context.User.Id);
			}
			if (context.Guild == null) return PreconditionResult.FromError("This command can only be run inside servers.");
			if (!player.ActiveCharacter.TryGetValue(context.Guild.Id, out int C))
			{
				return PreconditionResult.FromError("You have no active character in this server.");
			}
			if (C==-1) return PreconditionResult.FromError("You have no active character in this server."); 
			var ccol = ((LiteDatabase)services.GetService(typeof(LiteDatabase))).GetCollection<Character>("Characters");
			var character = ccol.FindOne(x => x.Id == C);
			if (character.UpgradePoints < UP)
			{
				return PreconditionResult.FromError("This character doesn't have enough Upgrade Points for this. (Need " + UP + ", Have " + character.UpgradePoints + ")");
			}
			return PreconditionResult.FromSuccess();
		}
	}
	public class CharacterTypeReader : TypeReader
	{
		public async  override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
		{
			LiteDatabase database = (LiteDatabase)services.GetService(typeof(LiteDatabase));
			if (context.Guild == null)
			{
				var collection = database.GetCollection<Character>("Characters");
				var results = collection.Find(x => x.Owner == context.User.Id && x.Name.StartsWith(input, StringComparison.InvariantCultureIgnoreCase)).ToArray();
				if (results.Length == 0) return TypeReaderResult.FromError(CommandError.ObjectNotFound, "No characters whoes name start with \""+input+"\" were found");
				else return TypeReaderResult.FromSuccess(results);
			}
			else
			{
				var col = database.GetCollection<Character>("Characters");
				var results = col.Find(x => x.Guild == context.Guild.Id && x.Name.StartsWith(input, StringComparison.InvariantCultureIgnoreCase)).ToArray();
				if (results.Length == 0) return TypeReaderResult.FromError(CommandError.ObjectNotFound, "No characters were found");
				else return TypeReaderResult.FromSuccess(results);
			}
		}
	}
	public class SkillTypereader : TypeReader
	{
		public async override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
		{
			LiteDatabase database = (LiteDatabase)services.GetService(typeof(LiteDatabase));
			if (context.Guild == null) return TypeReaderResult.FromError(CommandError.UnmetPrecondition,"This command can only be used inside a server");
			var Pcol = database.GetCollection<Player>("Players");
			var player = Pcol.FindOne(x => x.Id == context.User.Id);
			if (player == null)
			{
				Pcol.Insert(new Player() { Id = context.User.Id });
				player = Pcol.IncludeAll().FindOne(x => x.Id == context.User.Id);
			}

			if (!player.ActiveCharacter.TryGetValue(context.Guild.Id, out int C))
			{
				return TypeReaderResult.FromError(CommandError.ObjectNotFound,"You have no active character in this server.");
			}
			if (C == -1) return TypeReaderResult.FromError(CommandError.ObjectNotFound,"You have no active character in this server.");
			var ccol = database.GetCollection<Character>("Characters");
			var character = ccol.FindOne(x => x.Id == C);
			var skills = character.GetSkills().Where(x => x.Name.StartsWith(input, StringComparison.CurrentCultureIgnoreCase)).OrderBy(x=>x.Name);
			if (skills.Count() == 0) return TypeReaderResult.FromError(CommandError.ObjectNotFound, "A skill whose name starts with \"" + input + "\" could not be found.");
			return TypeReaderResult.FromSuccess(skills.ToArray());
		}
	}

}
