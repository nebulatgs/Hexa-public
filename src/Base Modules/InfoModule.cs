using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Hexa.Attributes;
using Hexa.Helpers;

namespace Hexa.Modules
{ 
    // [Cooldown(3, 5, CooldownBucketType.User)]
    [HexaCooldown(5)]
    [Description("Info")]
    public class InfoModule : BaseCommandModule
    {   
        [Command("ping")]
        [Description("Pong!")]
        [Category("Fun")]
        public async Task PingCommand(CommandContext ctx)
        {
            TimeSpan serverTimeDifference = ServerTime.ServerTimeDifference;
            DateTime actualTime = DateTime.Now + serverTimeDifference;
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
            hEmbed.embed.AddField(
                name: "ws",
                value: $"```{ctx.Client.Ping} ms```",
                inline: true
            );
            var message = await ctx.RespondAsync(embed: hEmbed.Build());
            var sendTime = DateTime.Now + serverTimeDifference - startTime;
            hEmbed.embed.Fields[0].Value = $"```{sendTime.Milliseconds} ms```";
            await message.ModifyAsync(embed: hEmbed.Build());
        }

        [Command("serverinfo")]
        [Aliases("sinfo", "guildinfo")]
        [GuildOnly]
        [Description("Get info about a server")]
        [Category("Utilities")]
        public async Task ServerInfoCommand(CommandContext ctx, [Description("The snowflake of the server")] ulong? server = null)
        {
            DiscordGuild guild = ctx.Guild;
            var tryGuild = ctx.Client.GetGuildAsync(server != null ? server.Value : 0);
            if(tryGuild.IsCompletedSuccessfully)
                guild = await tryGuild;
            var users = await guild.GetAllMembersAsync();
            var channels = guild.Channels;
            string roles = "";
            guild.Roles.Values.Where(x => x.Id != guild.EveryoneRole.Id).OrderByDescending(x => x.Position).ToImmutableList().ForEach(x => roles += x.Mention + ", ");
            var hEmbed = new HexaEmbed(ctx, $"Server Info for {guild.Name}");

            hEmbed.embed.Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = guild.IconUrl
            };
            //--------------------//
            hEmbed.embed.AddField(
                name: "Server Owner:",
                value: $"{guild.Owner.Mention}",
                inline: false
            );
            //--------------------//
            hEmbed.embed.AddField(
                name: "Member Count:",
                value: $"{guild.MemberCount}\n",
                inline: false
            );
            //--------------------//
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
            hEmbed.embed.AddField(
                name: "\u200B",
                value: $"\u200B",
                inline: true
            );
            //--------------------//
            hEmbed.embed.AddField(
                name: "Channels:",
                value: $"{guild.MemberCount}",
                inline: false
            );
            //--------------------//
            hEmbed.embed.AddField(
                name: "Text:",
                value: $"{channels.Values.Where(x => x.Type == DSharpPlus.ChannelType.Text).Count()}",
                inline: true
            );
            hEmbed.embed.AddField(
                name: "Voice:",
                value: $"{channels.Values.Where(x => x.Type == DSharpPlus.ChannelType.Voice).Count()}",
                inline: true
            );
            hEmbed.embed.AddField(
                name: "Stage:",
                value: $"{channels.Values.Where(x => (int)x.Type == 13).Count()}",
                inline: true
            );
            //-------------------//
            hEmbed.embed.AddField(
                name: "Roles:",
                value: roles + "@everyone",
                inline: false
            );

            await ctx.RespondAsync(embed: hEmbed.Build());
        }
    }
}