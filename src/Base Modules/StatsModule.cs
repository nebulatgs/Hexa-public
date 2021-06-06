using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharp​Plus.CommandsNext.Attributes;

using Hexa.Helpers;
using Hexa.Attributes;

namespace Hexa.Modules
{
    public class StatsModule : BaseCommandModule
    {
        [Command("stats")]
        // [Aliases("botstats", "botinfo")]
        [Description("Get Hexa's statistics")]
        [Category(SettingsManager.HexaSetting.InfoCategory)]
        public async Task StatsCommand(CommandContext ctx)
        {
            var hEmbed = new HexaEmbed(ctx, "bot info");
            hEmbed.embed.AddField(
                name: "Server Count",
                value: ctx.Client.Guilds.Count().ToString(),
                inline: true
            );
            hEmbed.embed.AddField(
                name: "User Count",
                value: ctx.Client.Guilds.Sum(x => x.Value.MemberCount).ToString(),
                inline: true
            );

            await ctx.RespondAsync(embed: hEmbed.Build());
        }

        [Command("uptime")]
        [Aliases("up")]
        [Description("Get Hexa's session uptime")]
        [Category(SettingsManager.HexaSetting.InfoCategory)]
        public async Task UptimeCommand(CommandContext ctx)
        {
            var hEmbed = new HexaEmbed(ctx, $"hexa's session uptime");

            hEmbed.embed.AddField(
                name: "Up for:",
                value: $"{(DateTime.Now - Program.LaunchTime).ToString(@"hh\:mm\:ss")}",
                inline: false
            );

            await ctx.RespondAsync(embed: hEmbed.Build());
        }
    }
}