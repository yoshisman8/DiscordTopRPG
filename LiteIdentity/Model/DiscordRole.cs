using System.Diagnostics.CodeAnalysis;
using LiteDB;
using Microsoft.AspNetCore.Identity;

namespace LiteDiscordIdentity
{
   [SuppressMessage("ReSharper", "UnusedMember.Global")]
   public class DiscordRole : IdentityRole<string>
   {
      public DiscordRole() => Id = ObjectId.NewObjectId().ToString();

      public DiscordRole(string roleName) : this() => Name = roleName;

      [BsonId] public new string Id { get; set; }

      public new string Name { get; set; }
   }
}