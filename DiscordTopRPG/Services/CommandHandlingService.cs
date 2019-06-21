using System;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using Discord.Addons.CommandCache;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using DiscordTopRPG.Database;

namespace DiscordTopRPG.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private IServiceProvider _provider;
        private InteractiveService _interactive;
        private readonly IConfiguration _config;
        private CommandCacheService _cache;
        private bool Ready = false;

		public Dictionary<ulong,ulong> CommandCache { get; set; }
        public CommandHandlingService(IConfiguration config, IServiceProvider provider, DiscordSocketClient discord, CommandService commands, CommandCacheService cache,InteractiveService interactive)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;
            _config = config;
            _interactive = interactive;
            _cache = cache;

			CommandCache = new Dictionary<ulong, ulong>();
            
            _discord.MessageReceived += MessageReceived;
            _discord.MessageUpdated += OnMessageUpdated;
            _discord.JoinedGuild += OnJoinedGuild;
            _discord.Ready += OnReady;
        }

		public async Task OnJoinedGuild(SocketGuild arg)
        {
            var col = Program.Database.GetCollection<Server>("Guilds");
			if (col.Exists(x => x.Id == arg.Id)) return;
            col.Insert(new Server() {Id=arg.Id});
        }


        public async Task OnReady()
        {
            await InitializeGuildsDB(_discord, Program.Database);
            Ready = true;
        }

        private async Task InitializeGuildsDB(DiscordSocketClient discord, LiteDatabase database)
        {
            var col = database.GetCollection<Server>("Guilds");
            var joined = discord.Guilds.Select(x=> x.Id).ToList();
            foreach (var x in joined)
            {
                if (!col.Exists(y =>y.Id == x))
                {
                    col.Insert(new Server()
                    {
                        Id = x
                    });
                }
            }
        }
		public async Task OnMessageUpdated(Cacheable<IMessage, ulong> _OldMsg, SocketMessage NewMsg, ISocketMessageChannel Channel)
        {
            var OldMsg = await _OldMsg.DownloadAsync();
            if (OldMsg== null||NewMsg==null) return;
            if (OldMsg.Source != MessageSource.User||NewMsg.Source != MessageSource.User) return;
            
            if(_cache.TryGetValue(NewMsg.Id, out var CacheMsg))
            {
                var reply = await Channel.GetMessageAsync(CacheMsg.First());
                await reply.DeleteAsync();
            }
            await MessageReceived(NewMsg);
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(),_provider);
            // Add additional initialization code here...
        }

        private async Task MessageReceived(SocketMessage rawMessage)
        {
            // Ignore system messages and messages from bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var context = new SocketCommandContext(_discord, message);
            var Guild = (context.Guild==null)?null:Program.Database.GetCollection<Server>("Guilds").FindOne(x=>x.Id==context.Guild.Id);

            int argPos = 0;
            if (Guild!= null && !message.HasStringPrefix(Guild.Prefix, ref argPos) && !message.HasMentionPrefix(_discord.CurrentUser, ref argPos)) return;

            if(DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
            {
                var chance = new Random().Next(0,100);
                if(chance <= 25)
                {
                    await context.Channel.SendMessageAsync("OOPSIE WOOPSIE!! Uwu We made a fucky wucky!! A wittle fucko boingo! The code monkeys at our headquarters are working VEWY HAWD to fix this!");
                    return;
                }
            }
            var result = await _commands.ExecuteAsync(context, argPos, _provider);
            
            if (result.Error.HasValue && (result.Error.Value != CommandError.UnknownCommand))
            {
                Console.WriteLine(result.Error+"\n"+result.ErrorReason); 
            }
            if (result.Error.HasValue && result.Error.Value == CommandError.ObjectNotFound)
            {
                var msg = await context.Channel.SendMessageAsync("Sorry. "+result.ErrorReason);
                _cache.Add(context.Message.Id,msg.Id);
            }
        }
    }
}