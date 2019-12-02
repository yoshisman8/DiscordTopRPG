using DiscordTopRPG.Data;
using LiteDB;
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
		[Inject]
		public LiteDatabase database { get; set; }
		[Inject]
		public SignInManager<ApplicationUser> SignInManager { get; set; }
		[Inject]
		public UserManager<ApplicationUser> UserManager { get; set; }

		private LiteCollection<Character> Characters { get; set; }
		public CharacterService()
		{
			Characters = database.GetCollection<Character>("Characters");
		}
		public async Task<Character> GetById(int Id)
		{
			return Characters.FindOne(x => x.Id == Id);
		}
		public async Task<List<Character>> FindCharacter(string Name)
		{
			var chars = Characters.Find(x => x.Name.StartsWith(Name.ToLower()));

			return chars.ToList();
		}
		public async Task<List<Character>> GetUserCharacters(ClaimsPrincipal User)
		{
			var id = await UserManager.GetUserAsync(User);

			var chars = Characters.Find(x => x.Owner == id.DiscordId);

			return chars.ToList();
		}
		public async Task<List<Character>> GetUserCharacters(ulong User)
		{
			var chars = Characters.Find(x => x.Owner == User);

			return chars.ToList();
		}
		public async Task<int> NewCharacter(Character character)
		{
			var val = Characters.Insert(character);
			return val.AsInt32;
		}
		public async Task UpdateCharacter(Character character)
		{
			Characters.Update(character);
		}
		public async Task Deletechar(int Id)
		{
			var ch = Characters.FindOne(x=>x.Id == Id);
			Characters.Delete(x=>x.Id== Id);
		}
	}
}
