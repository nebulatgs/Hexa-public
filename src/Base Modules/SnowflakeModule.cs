using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Hexa.Helpers;
using Hexa.Attributes;

namespace Hexa.Modules
{
    public class SnowflakeModule : BaseCommandModule
    {
        [Command("snowflake")]
        [Aliases("snow", "lookup")]
        [Description("Look up user/guild snowflakes")]
        [Category("Utilities")]
        public async Task Snow(CommandContext ctx, [Description("The snowflake of the user/guild you want to look up")] ulong? snowflake = null)
        {
            if (snowflake is null)
            {
                await ctx.RespondAsync("What snowflake should I look up?");
                return;
            }
            var hEmbed = new HexaEmbed(ctx, "snowflake information");
            var snowTime = DateTimeOffset.FromUnixTimeSeconds((long)(((snowflake >> 22) + 1420070400000) / 1000)).UtcDateTime;
            hEmbed.embed.Description = $"snowflake info on {snowflake}";
            hEmbed.embed.AddField(
                name: "created: ",
                value: $"{snowTime.ToString("U")} ({Math.Round((DateTime.Now - snowTime).TotalDays)} days ago)",
                inline: false
            );
            DiscordUser member;
            DiscordGuild guild;
            try
            {
                member = await ctx.Client.GetUserAsync(snowflake.Value);
                string badges = "";
                if (member.Flags == UserFlags.None)
                    badges += "None";
                if (member.Flags.Value.HasFlag(UserFlags.VerifiedBot))
                    badges += " <:verified1:848290146435858513><:verified2:848290146650161162>";
                if (member.Flags.Value.HasFlag(UserFlags.DiscordEmployee))
                    badges += " <:staff:846246922347610123>";
                if (member.Flags.Value.HasFlag(UserFlags.DiscordPartner))
                    badges += " <:partner:846268079741992980>";
                if (member.Flags.Value.HasFlag(UserFlags.HypeSquadEvents))
                    badges += " <:hypesquad_events:846247756766248971>";
                if (member.Flags.Value.HasFlag(UserFlags.BugHunterLevelOne))
                    badges += " <:bughunter:846247040102432778>";
                if (member.Flags.Value.HasFlag(UserFlags.BugHunterLevelTwo))
                    badges += " <:bug_hunter_lvl2:846375404444975114>";
                if (member.Flags.Value.HasFlag(UserFlags.HouseBravery))
                    badges += " <:bravery:845399344004857936>";
                if (member.Flags.Value.HasFlag(UserFlags.HouseBalance))
                    badges += " <:balance:845399343258927124>";
                if (member.Flags.Value.HasFlag(UserFlags.HouseBrilliance))
                    badges += " <:brilliance:845399344365699144>";
                if (member.Flags.Value.HasFlag(UserFlags.VerifiedBotDeveloper))
                    badges += " <:botdev:846230550741778452>";
                if (member.Flags.Value.HasFlag(UserFlags.EarlySupporter))
                    badges += " <:earlysupporter:846230028177375283>";

                hEmbed.embed.WithThumbnail(member.AvatarUrl, 32, 32);
                hEmbed.embed.AddField(
                    name: "user: ",
                    value: $"{member.Username}#{member.Discriminator}",
                    inline: true
                );
                hEmbed.embed.AddField(
                    name: "badges: ",
                    value: badges,
                    inline: true
                );
            }
            catch
            {
                try
                {
                    guild = await ctx.Client.GetGuildAsync(snowflake.Value);
                    hEmbed.embed.WithThumbnail(guild.IconUrl, 32, 32);
                    int roles = guild.Roles.Values.Where(x => x.Id != guild.EveryoneRole.Id).Count();
                    hEmbed.embed.AddField(
                        name: "guild: ",
                        value: $"{guild.Name}",
                        inline: true
                    );
                    hEmbed.embed.AddField(
                        name: "roles: ",
                        value: roles.ToString(),
                        inline: true
                    );
                }
                catch
                {
                    hEmbed.embed.AddField(
                    name: "Snowflake not found",
                    value: $"Unable to find a user/guild with this snowflake",
                    inline: false
                );
                }
            }

            await ctx.RespondAsync(embed: hEmbed.Build());
        }
    }
}