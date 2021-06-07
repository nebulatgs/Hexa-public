using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Hexa.Attributes;
using Hexa.Database;
using Hexa.Helpers;
using Hexa.Other;

namespace Hexa.Modules
{
    [HexaCooldown(5)]
    [GuildOnly]
    public class LevelsModule : BaseCommandModule
    {
        [Command("rank")]
        // [Aliases("lv", "lvl")]
        [Description("Get a user/guild's rank")]
        [Category(SettingsManager.HexaSetting.UtilityCategory)]
        public async Task LevelsCommand(CommandContext ctx, [Description("The snowflake of the user/guild you want to look up")] ulong? snowflake = null)
        {
            if (snowflake is null)
                snowflake = ctx.Guild.Id;
            var hEmbed = new HexaEmbed(ctx, "rank");
            DiscordUser member;
            DiscordGuild guild;
            try
            {
                member = await ctx.Client.GetUserAsync(snowflake.Value);
                hEmbed.embed.Title = $"**{member.Username}#{member.Discriminator}**'s global rank";
                var value = await LevelDBInterface.GetValue(member);
                if (value is null)
                    await LevelDBInterface.SetValue(member, 0, 0);
                value = await LevelDBInterface.GetValue(member);
                var values = await LevelDBInterface.GetUserValues();
                var instance = Supabase.Client.Instance;
                var channels = instance.From<PastUserState>();
                var states = (await channels.Get()).Models;
                // using (var db = new HexaContext())
                // {
                    hEmbed.embed.AddField($"messages:", $"#{(values.OrderByDescending(x => x.MessageLevel).Select(y => y.UserId).ToList().IndexOf(member.Id) + 1)}\n{value.MessageLevel} messages sent", true);
                    hEmbed.embed.AddField($"commands:", $"#{(values.OrderByDescending(x => x.CommandLevel).Select(y => y.UserId).ToList().IndexOf(member.Id) + 1)}\n{value.CommandLevel} commands used", true);
                    hEmbed.embed.AddField("past names:", /*zws*/$"​{string.Join(", ", states.Where(x => x.UserId == member.Id).Select(x => x.Username))}");
                    hEmbed.embed.WithThumbnail(member.AvatarUrl, 32);
                // }
            }
            catch
            {
                try
                {
                    guild = await ctx.Client.GetGuildAsync(snowflake.Value);
                    hEmbed.embed.Description = $"**{guild.Name}**'s global rank";
                    var value = await LevelDBInterface.GetValue(guild);
                    if (value is null)
                        await LevelDBInterface.SetValue(guild, 0, 0);
                    value = await LevelDBInterface.GetValue(guild);
                    var values = await LevelDBInterface.GetGuildValues();
                    hEmbed.embed.AddField($"messages:", $"#{(values.OrderByDescending(x => x.MessageLevel).Select(y => y.GuildId).ToList().IndexOf(guild.Id) + 1)}\n{value.MessageLevel} messages sent", true);
                    hEmbed.embed.AddField($"commands:", $"#{(values.OrderByDescending(x => x.CommandLevel).Select(y => y.GuildId).ToList().IndexOf(guild.Id) + 1)}\n{value.CommandLevel} commands used", true);
                    hEmbed.embed.WithThumbnail(guild.IconUrl, 32, 32);
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

        [Command("rank")]
        public async Task LevelsCommand(CommandContext ctx, [Description("The user you want to look up")] DiscordMember member)
        {
            var hEmbed = new HexaEmbed(ctx, "rank");
            try
            {
                hEmbed.embed.Title = $"**{member.DisplayName}**'s global rank";
                var value = await LevelDBInterface.GetValue(member);
                if (value is null)
                    await LevelDBInterface.SetValue(member, 0, 0);
                value = await LevelDBInterface.GetValue(member);
                var values = await LevelDBInterface.GetUserValues();
                var instance = Supabase.Client.Instance;
                var channels = instance.From<PastUserState>();
                var states = (await channels.Get()).Models;
                // using (var db = new HexaContext())
                // {
                    hEmbed.embed.AddField($"messages:", $"#{(values.OrderByDescending(x => x.MessageLevel).Select(y => y.UserId).ToList().IndexOf(member.Id) + 1)}\n{value.MessageLevel} messages sent", true);
                    hEmbed.embed.AddField($"commands:", $"#{(values.OrderByDescending(x => x.CommandLevel).Select(y => y.UserId).ToList().IndexOf(member.Id) + 1)}\n{value.CommandLevel} commands used", true);
                    hEmbed.embed.AddField("past names:", /*zws*/$"​{string.Join(", ", states.Where(x => x.UserId == member.Id).Select(x => x.Username).Distinct())}");
                    hEmbed.embed.WithThumbnail(member.AvatarUrl, 32);
                // }
            }
            catch
            {
                hEmbed.embed.AddField(
                name: "User not found",
                value: $"Unable to find that user",
                inline: false
            );
            }

            await ctx.RespondAsync(embed: hEmbed.Build());
        }
    }
}