using DiscordTopRPG.Data;
using LiteDB;
using LiteDiscordIdentity;
using Microsoft.AspNetCore.Blazor.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DiscordTopRPG.Services
{
	public class CharacterService
	{
		private LiteCollection<Character> Characters { get; set; }

		public CharacterService(LiteDbContext _context)
		{
			Characters = _context.LiteDatabase.GetCollection<Character>("Characters");
		}
		public async Task<Character> GetById(int Id)=>  await Task.Run(() => 
		{ 
			return Characters.FindOne(x => x.Id == Id); 
		});

		public async Task<List<Character>> FindCharacter(string Name) => await Task.Run(()=>
		{
			var chars = Characters.Find(x => x.Name.StartsWith(Name.ToLower()));

			return chars.ToList();
		});

		public async Task<List<Character>> GetUserCharacters(DiscordUser User) => await Task.Run(() => 
		{
			var chars = Characters.IncludeAll().Find(x => x.Owner == User.Id);

			return chars.ToList();
		});
		
		public async Task<List<Character>> GetUserCharacters(ulong User) => await Task.Run(() =>
		{
			var chars = Characters.IncludeAll().Find(x => x.Owner == User);

			return chars.ToList();
		});
		
		public async Task<int> NewCharacter(Character character) => await Task.Run(()=>
		{
			var val = Characters.Insert(character);
			return val.AsInt32;
		});
		public async Task UpdateCharacter(Character character) => await Task.Run(() => Characters.Update(character));
		public async Task Deletechar(int Id) => await Task.Run(() =>
		{
			var ch = Characters.FindOne(x=>x.Id == Id);
			Characters.Delete(x=>x.Id== Id);
		});
	}
}
