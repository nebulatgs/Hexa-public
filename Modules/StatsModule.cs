using System;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using DSharpPlus.CommandsNext;
using DSharp​Plus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus;

namespace Hexa.Modules
{
    public class StatsModule : BaseCommandModule
    {
        [Command("stats")]
        [Aliases("botstats", "botinfo")]
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
    }
}