using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTopRPG.Data
{
	public class ApplicationUser : IdentityUser
	{
		public ApplicationUser() { }
		public ApplicationUser(string UserName, ulong DiscordId) : base(UserName)
		{
			this.UserName = UserName;
			this.DiscordId = DiscordId;
		}
		public ulong DiscordId { get; set; }
		public string AvatarUrl { get; set; }
		public bool IsAuthenticated { get; set; }
	}
}
