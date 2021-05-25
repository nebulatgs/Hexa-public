using System.IO;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using DSharpPlus.CommandsNext;
using DSharp​Plus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus;
using Hexa.Attributes;

namespace Hexa.Modules
{
    [DevOnly]
    [Hidden]
    [Description("Bot Logs")]
    public class LogsModule : BaseCommandModule
    {
        public HexaLogger Logger { private get; set; }
        [Command("logs")]
        [Aliases("log")]
        public async Task StatsCommand(CommandContext ctx)
        {
            using FileStream file = File.OpenRead(Logger.LogFile);
            var message = new DiscordMessageBuilder().WithFile(file).WithReply(ctx.Message.Id);
            await message.SendAsync(ctx.Message.Channel);
        }
    }
}