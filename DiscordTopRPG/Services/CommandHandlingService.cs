using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using System.IO;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ERA20.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private IConfiguration _config;
        private IServiceProvider _provider;

        public CommandHandlingService(IServiceProvider provider, DiscordSocketClient discord, IConfiguration config, CommandService commands)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;
            _config = config;

            _discord.MessageReceived += MessageReceived;
            _discord.MessageUpdated += OnMessageUpdate;
        }


        private async Task OnMessageUpdate(Cacheable<IMessage, ulong> original, SocketMessage edit, ISocketMessageChannel channel)
        {
            if (edit == null) return;
            var msg = await original.DownloadAsync() as SocketUserMessage;
            var msg2 = edit as SocketUserMessage;
            int argPos = 0;

            if (msg2.HasStringPrefix(_config["prefix"], ref argPos))
            {
                var messages = await channel.GetMessagesAsync(msg, Direction.After, 2, CacheMode.AllowDownload).FlattenAsync();
                var lastreply = messages.Where(x => x.Author.Id == _discord.CurrentUser.Id).FirstOrDefault();
                await lastreply.DeleteAsync();
                await MessageReceived(edit);
            }
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(),provider);
            // Add additional initialization code here...
        }

        private async Task MessageReceived(SocketMessage rawMessage)
        {
            // Ignore system messages and messages from bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Author == _discord.CurrentUser) return;
            var msg = rawMessage as SocketUserMessage;
            int argPos = 0;
            var context = new SocketCommandContext(_discord, message);

            var cmds = "";
            if (msg.Content != "")
            {
                cmds = msg.Content.Substring(1).Split(' ').FirstOrDefault();
            }

            if (msg.HasStringPrefix(_config["prefix"], ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _provider);     // Execute the command

                if (!result.IsSuccess && ((result.Error != CommandError.UnknownCommand) && (result.Error != CommandError.BadArgCount)))
                {     // If command error, reply with the error and send error to Crash log.
                    await msg.AddReactionAsync(new Discord.Emoji("💥"));
                    var cnnl = context.Guild.GetTextChannel(495267183518285835);
                    await cnnl.SendMessageAsync("[" + DateTime.Now.ToShortDateString() + "] " + DateTime.Now.ToShortTimeString() + " " + result.Error.Value.ToString());
                }
                if (!result.IsSuccess && result.Error == CommandError.UnknownCommand)
                {     // If not a command, reply with the Emote.
                    await msg.AddReactionAsync(new Discord.Emoji("❓"));
                }
                if (!result.IsSuccess && result.Error == CommandError.BadArgCount)
                {
                    // if incorrect arguments, DM command help.
                    var DMs = await context.User.GetOrCreateDMChannelAsync();
                    string command = msg.Content.Split(' ')[0].Substring(1);
                    var res = _commands.Search(context, command);
                    if (!res.IsSuccess)
                    {
                        await DMs.SendMessageAsync($"Sorry, I couldn't find a command like **{command}**.");
                        return;
                    }
                    string prefix = _config["prefix"];
                    var builder = new EmbedBuilder()
                    {
                        Color = new Color(114, 137, 218),
                        Description = $"Here is how you use **{command}**:"
                    };
                    foreach (var match in res.Commands)
                    {
                        var cmd = match.Command;
                        builder.AddField(x =>
                        {
                            x.Name = string.Join(", ", cmd.Aliases);
                            x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                                    $"Summary: {cmd.Summary}";
                            x.IsInline = false;
                        });
                    }
                    await DMs.SendMessageAsync("", false, builder.Build());
                }
            }
        }

    }
}