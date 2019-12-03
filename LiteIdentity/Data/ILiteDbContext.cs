using LiteDB;

namespace LiteDiscordIdentity
{
   public interface ILiteDbContext
   {
      LiteDatabase LiteDatabase { get; }
   }
}