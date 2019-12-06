using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.Win32.SafeHandles;

namespace LiteDiscordIdentity
{
   [SuppressMessage("ReSharper", "UnusedMember.Global")]
   public class LiteDbRoleStore: IQueryableRoleStore<DiscordRole>, IRoleStore<DiscordRole>
   {
      private readonly LiteCollection<CancellationToken> _cancellationTokens;
      private readonly LiteCollection<DiscordRole> _roles;

      public LiteDbRoleStore(LiteDbContext dbContext)
      {
         _roles = dbContext.LiteDatabase.GetCollection<DiscordRole>("roles");
         _cancellationTokens = dbContext.LiteDatabase.GetCollection<CancellationToken>("cancellationtokens");
      }

      public virtual IQueryable<DiscordRole> Roles => _roles.FindAll().AsQueryable();

      public async Task<IdentityResult> CreateAsync(DiscordRole role, CancellationToken cancellationToken)
      {
         cancellationToken.ThrowIfCancellationRequested();
         ThrowIfDisposed();

         if (role == null) throw new ArgumentNullException(nameof(role));

         await Task.Run(() => { _roles.Insert(role); }, cancellationToken);

         return IdentityResult.Success;
      }

      public async Task<IdentityResult> UpdateAsync(DiscordRole role, CancellationToken cancellationToken)
      {
         cancellationToken.ThrowIfCancellationRequested();
         ThrowIfDisposed();

         if (role == null) throw new ArgumentNullException(nameof(role));

         await Task.Run(() => { _roles.Update(role.Id, role); }, cancellationToken);

         return IdentityResult.Success;
      }

      public async Task<IdentityResult> DeleteAsync(DiscordRole role, CancellationToken cancellationToken)
      {
         cancellationToken.ThrowIfCancellationRequested();
         ThrowIfDisposed();

         if (role == null) throw new ArgumentNullException(nameof(role));

         await Task.Run(() => { _roles.Delete(role.Id); }, cancellationToken);

         return IdentityResult.Success;
      }

      public Task<string> GetRoleIdAsync(DiscordRole role, CancellationToken cancellationToken)
      {
         cancellationToken.ThrowIfCancellationRequested();
         ThrowIfDisposed();

         if (role == null) throw new ArgumentNullException(nameof(role));

         return Task.FromResult(role.Id);
      }

      public Task<string> GetRoleNameAsync(DiscordRole role, CancellationToken cancellationToken)
      {
         cancellationToken.ThrowIfCancellationRequested();
         ThrowIfDisposed();

         if (role == null) throw new ArgumentNullException(nameof(role));

         return Task.FromResult(role.Name);
      }

      public Task SetRoleNameAsync(DiscordRole role, string roleName, CancellationToken cancellationToken)
      {
         cancellationToken.ThrowIfCancellationRequested();
         ThrowIfDisposed();

         if (role == null) throw new ArgumentNullException(nameof(role));
         role.Name = roleName ?? throw new ArgumentNullException(nameof(roleName));

         return Task.CompletedTask;
      }

      public Task<string> GetNormalizedRoleNameAsync(DiscordRole role, CancellationToken cancellationToken)
      {
         cancellationToken.ThrowIfCancellationRequested();
         ThrowIfDisposed();

         if (role == null) throw new ArgumentNullException(nameof(role));

         return Task.FromResult(role.NormalizedName);
      }

      public Task SetNormalizedRoleNameAsync(DiscordRole role, string normalizedName, CancellationToken cancellationToken)
      {
         cancellationToken.ThrowIfCancellationRequested();
         ThrowIfDisposed();

         if (role == null) throw new ArgumentNullException(nameof(role));
         role.NormalizedName = normalizedName ?? throw new ArgumentNullException(nameof(normalizedName));

         return Task.CompletedTask;
      }

      public Task<DiscordRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
      {
         cancellationToken.ThrowIfCancellationRequested();
         ThrowIfDisposed();

         return Task.FromResult(_roles.FindOne(u => u.Id == roleId));
      }

      public Task<DiscordRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
      {
         cancellationToken.ThrowIfCancellationRequested();
         ThrowIfDisposed();

         var query = _roles.Find(r => r.NormalizedName == normalizedRoleName).FirstOrDefault();

         return Task.FromResult(query);
      }

      public Task SaveChanges(
         CancellationToken cancellationToken = default(CancellationToken)
      )
      {
         _cancellationTokens.Insert(cancellationToken);
         return Task.FromResult(cancellationToken);
      }

      public virtual Task CreateAsync(DiscordRole role) => Task.FromResult(_roles.Insert(role));

      public virtual Task UpdateAsync(DiscordRole role) => Task.FromResult(_roles.Update(role.Id, role));

      public virtual Task DeleteAsync(DiscordRole role) => Task.FromResult(_roles.Delete(role.Id));

      public virtual Task<DiscordRole> FindByIdAsync(string roleId)
      {
         return Task.FromResult(_roles.FindOne(r => r.Id == roleId));
      }

      public virtual Task<DiscordRole> FindByNameAsync(string roleName)
      {
         return Task.FromResult(_roles.FindOne(r => r.Name == roleName));
      }

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

         if (disposing)
         {
            _handle.Dispose();
         }

         _disposed = true;
      }

      #endregion
   }
}