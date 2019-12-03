using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using LiteDB;
using Microsoft.AspNetCore.Identity;

namespace LiteDiscordIdentity
{
	public class DiscordUser : IIdentity
	{
		public DiscordUser() { }
		public DiscordUser(ulong _id, string _displayname, int _identifier)
		{
			Id = _id;
			Name = _displayname;
			Discriminator = _identifier;
		}

		[BsonId] public ulong Id { get; set; }
		public string Name { get; set; }
		public int Discriminator { get; set; }
		public List<string> Roles { get; set; } = new List<string>();
		public List<UserToken<string>> Tokens { get; set; } = new List<UserToken<string>>();
		public List<SerializableUserLoginInfo> SerializableLogins { get; set; } = new List<SerializableUserLoginInfo>();
		[BsonIgnore]
		public List<UserLoginInfo> Logins
		{
			get
			{
				return SerializableLogins?.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, "")).ToList() ??
					   new List<UserLoginInfo>();
			}
			set
			{
				SerializableLogins =
				   value?.Select(x => new SerializableUserLoginInfo(x.LoginProvider, x.ProviderKey)).ToList() ??
				   new List<SerializableUserLoginInfo>();
			}
		}
		public string SecurityStamp { get; set; }

		public string AuthenticationType { get; set; }

		public bool IsAuthenticated { get; set; }

		public virtual void AddRole(string role)
		{
			Roles.Add(role);
		}

		public virtual void RemoveRole(string role)
		{
			Roles.Remove(role);
		}

		public virtual void AddLogin(UserLoginInfo login)
		{
			SerializableLogins.Add(new SerializableUserLoginInfo(login.LoginProvider, login.ProviderKey));
		}

		public virtual void RemoveLogin(UserLoginInfo login)
		{
			var loginsToRemove = SerializableLogins
			   .Where(l => l.LoginProvider == login.LoginProvider)
			   .Where(l => l.ProviderKey == login.ProviderKey);

			SerializableLogins = SerializableLogins.Except(loginsToRemove).ToList();
		}
		public void AddToken(UserToken<string> token)
		{
			var existingToken =
			   Tokens.SingleOrDefault(t => t.LoginProvider == token.LoginProvider && t.TokenName == token.TokenName);
			if (existingToken == null)
				Tokens.Add(token);
			else
				existingToken.TokenValue = token.TokenValue;
		}

		public void RemoveToken(string loginProvider, string name)
		{
			Tokens = Tokens
			   .Except(Tokens.Where(t => t.LoginProvider == loginProvider && t.TokenName == name))
			   .ToList();
		}
	}
}
