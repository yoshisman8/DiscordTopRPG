using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.Win32.SafeHandles;

namespace LiteDiscordIdentity
{
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	[SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
	public class LiteDbUserStore : 
		IDisposable,
		IUserLoginStore<DiscordUser>,
		IUserRoleStore<DiscordUser>,
		IUserStore<DiscordUser>
	{
		private const string AuthenticatorStoreLoginProvider = "[AspNetAuthenticatorStore]";
		private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
		private const string RecoveryCodeTokenName = "RecoveryCodes";
		private readonly LiteCollection<CancellationToken> _cancellationTokens;

		private readonly LiteCollection<DiscordUser> _users;

		public LiteDbUserStore(ILiteDbContext dbContext)
		{
			_users = dbContext.LiteDatabase.GetCollection<DiscordUser>("users");
			_cancellationTokens = dbContext.LiteDatabase.GetCollection<CancellationToken>("cancellationtokens");
		}

		public Task SaveChanges(
		   CancellationToken cancellationToken = default(CancellationToken)
		)
		{
			_cancellationTokens.Insert(cancellationToken);
			return Task.FromResult(cancellationToken);
		}

		#region IUserStore

		public Task<string> GetUserIdAsync(DiscordUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (user == null) throw new ArgumentNullException(nameof(user));

			return Task.FromResult(user.Id.ToString());
		}

		public Task<string> GetUserNameAsync(DiscordUser user, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			if (user == null) throw new ArgumentNullException(nameof(user));

			return Task.FromResult(user.Name);
		}

		public Task SetUserNameAsync(DiscordUser user, string userName,
		   CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			if (user == null) throw new ArgumentNullException(nameof(user));
			user.Name = userName ?? throw new ArgumentNullException(nameof(userName));

			return Task.CompletedTask;
		}

		public Task<string> GetNormalizedUserNameAsync(DiscordUser user,
		   CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			if (user == null) throw new ArgumentNullException(nameof(user));

			return Task.FromResult(user.Name.ToUpper());
		}

		/// <summary>
		/// NOT IMPLEMENTED
		/// </summary>
		/// <param name="user"></param>
		/// <param name="normalizedName"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task SetNormalizedUserNameAsync(DiscordUser user, string normalizedName,
		   CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			return Task.CompletedTask;
		}

		public async Task<IdentityResult> CreateAsync(
		   DiscordUser user,
		   CancellationToken cancellationToken = default(CancellationToken)
		)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			if (user == null) throw new ArgumentNullException(nameof(user));

			await Task.Run(() => 
			{ 
				_users.Insert(user);
				_users.EnsureIndex(x => x.Id); 
				_users.EnsureIndex(x => x.Name);
				_users.EnsureIndex("Name","LOWER($.Name)");
				_users.EnsureIndex("Name", "UPPER($.Name)");
			}, cancellationToken);

			return IdentityResult.Success;
		}

		public async Task<IdentityResult> UpdateAsync(DiscordUser user,
		   CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			if (user == null) throw new ArgumentNullException(nameof(user));

			await Task.Run(() => { _users.Update(user.Id, user); }, cancellationToken);


			return IdentityResult.Success;
		}

		public async Task<IdentityResult> DeleteAsync(
		   DiscordUser user,
		   CancellationToken cancellationToken = default(CancellationToken)
		)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			if (user == null) throw new ArgumentNullException(nameof(user));

			await Task.Run(() => { _users.Delete(user.Id); }, cancellationToken);

			return IdentityResult.Success;
		}

		public Task<DiscordUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				
				var id = Convert.ToUInt64(userId);
				
				return Task.FromResult(_users.FindOne(u => u.Id == id));
			}
			catch(FormatException e)
			{
				throw e;
			}
		}

		public Task<DiscordUser> FindByNameAsync(string normalizedUserName,
		   CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			var query = _users.Find(u => u.Name == normalizedUserName).FirstOrDefault();

			return Task.FromResult(query);
		}

		#endregion

		#region IUserLoginStore

		public Task AddLoginAsync(DiscordUser user, UserLoginInfo login,
		   CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			if (user == null) throw new ArgumentNullException(nameof(user));

			if (login == null) throw new ArgumentNullException(nameof(login));

			user.AddLogin(login);

			return Task.CompletedTask;
		}

		public Task RemoveLoginAsync(DiscordUser user, string loginProvider, string providerKey,
		   CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			if (user == null) throw new ArgumentNullException(nameof(user));

			if (loginProvider == null) throw new ArgumentNullException(nameof(loginProvider));

			if (providerKey == null) throw new ArgumentNullException(nameof(providerKey));

			user.RemoveLogin(new UserLoginInfo(loginProvider, providerKey, loginProvider));

			return Task.CompletedTask;
		}

		public Task<IList<UserLoginInfo>> GetLoginsAsync(DiscordUser user,
		   CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			if (user == null) throw new ArgumentNullException(nameof(user));

			return Task.FromResult<IList<UserLoginInfo>>(user.Logins.ToList());
		}

		public Task<DiscordUser> FindByLoginAsync(string loginProvider, string providerKey,
		   CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();

			if (loginProvider == null) throw new ArgumentNullException(nameof(loginProvider));

			if (providerKey == null) throw new ArgumentNullException(nameof(providerKey));

			var query = _users.Find(l =>
			   l.Logins.Any(s => (s.LoginProvider == loginProvider) & (s.ProviderKey == providerKey)));

			return Task.FromResult(query.FirstOrDefault());
		}

		#endregion

		#region IDisposable

		private void ThrowIfDisposed()
		{
			if (_disposed) throw new ObjectDisposedException(GetType().Name);
		}

		private bool _disposed;
		private readonly SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing) _handle.Dispose();

			_disposed = true;
		}

		#endregion

		#region IUserRoleStore

		public Task AddToRoleAsync(DiscordUser user, string roleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (user == null) throw new ArgumentNullException(nameof(user));
			if (roleName == null) throw new ArgumentNullException(nameof(roleName));

			user.Roles.Add(roleName);
			return Task.CompletedTask;
		}

		public Task RemoveFromRoleAsync(DiscordUser user, string roleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (user == null) throw new ArgumentNullException(nameof(user));
			if (roleName == null) throw new ArgumentNullException(nameof(roleName));

			user.Roles.Remove(roleName);
			return Task.CompletedTask;
		}

		public Task<IList<string>> GetRolesAsync(DiscordUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (user == null) throw new ArgumentNullException(nameof(user));
			var result = user.Roles as IList<string>;
			return Task.FromResult(result);
		}

		public Task<bool> IsInRoleAsync(DiscordUser user, string roleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (user == null) throw new ArgumentNullException(nameof(user));
			if (roleName == null) throw new ArgumentNullException(nameof(roleName));
			return Task.FromResult(user.Roles.Contains(roleName));
		}

		public Task<IList<DiscordUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ThrowIfDisposed();
			if (roleName == null) throw new ArgumentNullException(nameof(roleName));
			return Task.FromResult((IList<DiscordUser>)_users.Find(u => u.Roles.Contains(roleName)).ToList());
		}
		#endregion
	}
}