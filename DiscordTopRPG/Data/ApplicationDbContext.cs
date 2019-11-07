using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiscordTopRPG.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		DbSet<Character> Characters { get; set; }
		DbSet<Item> Items { get; set; }
		DbSet<Skill> Skills { get; set; }
		DbSet<SkillAction> Actions { get; set; }

	}
}
