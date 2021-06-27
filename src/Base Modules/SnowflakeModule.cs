using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Hexa.Attributes;
using Hexa.Helpers;

namespace Hexa.Modules
{
    public class SnowflakeModule : BaseCommandModule
    {
        private static Dictionary<string, UserFlags> BadgeDictionary = new()
        {
            {"<:bot1:850513690650083369><:bot2:850513720567136256>", UserFlags.VerifiedBot},
            {"<:staff:846246922347610123>", UserFlags.DiscordEmployee},
            {"<:partner:846268079741992980>", UserFlags.DiscordPartner},
            {"<:hypesquad_events:846247756766248971>", UserFlags.HypeSquadEvents},
            {"<:bughunter:846247040102432778>", UserFlags.BugHunterLevelOne},
            {"<:bug_hunter_lvl2:846375404444975114>", UserFlags.BugHunterLevelTwo},
            {"<:bravery:845399344004857936>", UserFlags.HouseBravery},
            {"<:brilliance:845399344365699144>", UserFlags.HouseBrilliance},
            {"<:botdev:846230550741778452>", UserFlags.VerifiedBotDeveloper},
            {"<:earlysupporter:846230028177375283>", UserFlags.EarlySupporter}
        };

        [Command("snowflake")]
        [Aliases("snow", "lookup")]
        [Description("Look up user/guild snowflakes")]
        [Category(SettingsManager.HexaSetting.UtilityCategory)]
        public async Task Snow(CommandContext ctx, [Description("The snowflake of the user/guild you want to look up")] ulong? snowflake = null)
        {
            if (snowflake is null) throw new ArgumentException("What snowflake should I look up?");
            var hEmbed = new HexaEmbed(ctx, "snowflake information");
            var snowTime = DateTimeOffset.FromUnixTimeSeconds((long)(((snowflake >> 22) + 1420070400000) / 1000)).UtcDateTime;
            hEmbed.embed.Description = $"snowflake info on {snowflake}";
            hEmbed.embed.AddField(
                name: "created: ",
                value: $"{snowTime.ToString("U")} ({Math.Floor((DateTime.Now - snowTime).TotalDays)} days ago)",
                inline: false
            );
            DiscordUser member;
            DiscordGuild guild;

            try
            {
                member = await ctx.Client.GetUserAsync(snowflake.Value);
                string badges = string.Empty;
                if (member.Flags.HasValue)
                {
                    if (member.Flags == UserFlags.None && !member.IsBot) badges = "None ";
                    badges += string.Join(" ", BadgeDictionary.Where(x => member.Flags.Value.HasFlag(x.Value)).Select(x => x.Key));
                }
                else if (member.IsBot) badges += " <:bot1:850513690650083369><:bot2:850513720567136256>";
                else badges += "None";

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
                // if (member.IsBot)
                // {
                //     var response = GuildCountRequester.Request(member.Id);
                //     var guildCount = response.Bot.ApproximateGuildCount;
                //     Console.WriteLine(guildCount);
                //     hEmbed.embed.AddField(
                //         name: "guild count: ",
                //         value: guildCount.ToString(),
                //         inline: true
                //     );
                // }
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