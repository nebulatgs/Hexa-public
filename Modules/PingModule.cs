using System.Linq;
using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Hexa.Attributes;

namespace Hexa.Modules
{ 
    // [Cooldown(3, 5, CooldownBucketType.User)]
    [HexaCooldown(5)]
    public class InfoModule : BaseCommandModule
    {   
        [Command("ping")]
        public async Task PingCommand(CommandContext ctx)
        {
            var startTime = DateTime.Now;
            var receiveTime = startTime - ctx.Message.Timestamp;
            var hEmbed = new HexaEmbed(ctx, "Pong!");
            hEmbed.embed.AddField(
                name: "send",
                value: "loading",
                inline: true
            );
            hEmbed.embed.AddField(
                name: "recieve",
                value: $"```{receiveTime.Milliseconds} ms```",
                inline: true
            );
            var message = await ctx.RespondAsync(embed: hEmbed.Build());
            var sendTime = DateTime.Now - startTime;
            hEmbed.embed.Fields[0].Value = $"```{sendTime.Milliseconds} ms```";
            await message.ModifyAsync(embed: hEmbed.Build());
        }

        [Command("serverinfo")]
        [Aliases("sinfo", "guildinfo")]
        [GuildOnly]
        public async Task ServerInfoCommand(CommandContext ctx)
        {
            var users = await ctx.Guild.GetAllMembersAsync();
            var hEmbed = new HexaEmbed(ctx, $"Server Info for {ctx.Guild.Name}");

            hEmbed.embed.AddField(
                name: "Member Count:",
                value: $"{ctx.Guild.MemberCount}",
                inline: false
            );
            hEmbed.embed.AddField(
                name: "Bots:",
                value: $"{users.Where((user) => user.IsBot).Count()}",
                inline: true
            );
            hEmbed.embed.AddField(
                name: "Humans:",
                value: $"{users.Where((user) => !user.IsBot).Count()}",
                inline: true
            );

            await ctx.RespondAsync(embed: hEmbed.Build());
        }
    }
}