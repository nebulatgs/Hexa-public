using System.IO;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using DSharp​Plus.CommandsNext.Attributes;

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
        [Category("Danger")]
        public async Task StatsCommand(CommandContext ctx)
        {
            using FileStream file = File.OpenRead(Logger.LogFile);
            var message = new DiscordMessageBuilder().WithFile(file).WithReply(ctx.Message.Id);
            await message.SendAsync(ctx.Message.Channel);
        }
    }
}