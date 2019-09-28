using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTopRPG.Models
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
    }
}
