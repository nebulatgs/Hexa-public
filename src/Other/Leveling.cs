using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Hexa.Helpers;
using Hexa.Modules;
using Hexa.Other;
using Postgrest.Attributes;
using Postgrest.Models;

namespace Hexa.Other
{
    [Table("GuildLevels")]
    public class GuildLevelRow : BaseModel
    {
        [PrimaryKey("guild_id")]
        public ulong GuildId { get; set; }

        [Column("commands")]
        public int CommandLevel { get; set; }

        [Column("messages")]
        public int MessageLevel { get; set; }

        public override bool Equals(object obj)
        {
            return obj is GuildLevelRow message &&
                    GuildId == message.GuildId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GuildId);
        }
    }
    [Table("UserLevels")]
    public class UserLevelRow : BaseModel
    {
        [PrimaryKey("user_id")]
        public ulong UserId { get; set; }

        [Column("commands")]
        public int CommandLevel { get; set; }

        [Column("messages")]
        public int MessageLevel { get; set; }

        public override bool Equals(object obj)
        {
            return obj is UserLevelRow message &&
                    UserId == message.UserId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId);
        }
    }

    public static class LevelDBInterface
    {
        public static async Task<GuildLevelRow> GetValue(DiscordGuild guild)
        {
            if (guild is null)
                return null;
            var instance = Supabase.Client.Instance;
            var channels = instance.From<GuildLevelRow>();
            var guild_levels = await channels.Get();
            var this_guild = guild_levels.Models.Where(x => x.GuildId == guild.Id).FirstOrDefault();
            return this_guild;
        }
        public static async Task<List<GuildLevelRow>> GetGuildValues()
        {
            var instance = Supabase.Client.Instance;
            var channels = instance.From<GuildLevelRow>();
            var guild_levels = await channels.Get();
            var guilds = guild_levels.Models;
            return guilds;
        }
        public static async Task SetValue(DiscordGuild guild, int command_level, int message_level)
        {
            var instance = Supabase.Client.Instance;
            var new_row = new GuildLevelRow
            {
                GuildId = guild.Id,
                CommandLevel = command_level,
                MessageLevel = message_level
            };
            var channels = instance.From<GuildLevelRow>();
            var guild_levels = await channels.Get();
            bool foundSettings = guild_levels.Models.Where(x => x.GuildId == guild.Id).Count() > 0;
            if (!foundSettings)
            {
                await channels.Insert(new_row);
            }
            else
            {
                await channels.Update(new_row);
            }
        }

        public static async Task<UserLevelRow> GetValue(DiscordUser user)
        {
            var instance = Supabase.Client.Instance;
            var channels = instance.From<UserLevelRow>();
            var guild_levels = await channels.Get();
            var this_guild = guild_levels.Models.Where(x => x.UserId == user.Id).FirstOrDefault();
            return this_guild;
        }
        public static async Task<List<UserLevelRow>> GetUserValues()
        {
            var instance = Supabase.Client.Instance;
            var channels = instance.From<UserLevelRow>();
            var user_levels = await channels.Get();
            var users = user_levels.Models;
            return users;
        }
        public static async Task SetValue(DiscordUser user, int command_level, int message_level)
        {
            var instance = Supabase.Client.Instance;
            var new_row = new UserLevelRow
            {
                UserId = user.Id,
                CommandLevel = command_level,
                MessageLevel = message_level
            };
            var channels = instance.From<UserLevelRow>();
            var user_levels = await channels.Get();
            bool foundSettings = user_levels.Models.Where(x => x.UserId == user.Id).Any();
            if (!foundSettings)
            {
                await channels.Insert(new_row);
            }
            else
            {
                await channels.Update(new_row);
            }
        }
    }
    public class GuildLevels
    {
        public async Task MessageSent(DiscordClient client, MessageCreateEventArgs args)
        {
            if (args.Author.IsBot || args.Guild is null)
                return;
            var this_guild = await LevelDBInterface.GetValue(args.Guild);
            await LevelDBInterface.SetValue(args.Guild, this_guild is null ? 0 : this_guild.CommandLevel, this_guild is null ? 1 : this_guild.MessageLevel + 1);
        }
        public async Task CommandExecuted(CommandsNextExtension commands, CommandExecutionEventArgs args)
        {
            if (args.Context.Message.Author.IsBot || args.Context.Guild is null)
                return;
            var this_guild = await LevelDBInterface.GetValue(args.Context.Guild);
            await LevelDBInterface.SetValue(args.Context.Guild, this_guild is null ? 0 : this_guild.CommandLevel + 1, this_guild is null ? 1 : this_guild.MessageLevel);
        }
    }
    public class UserLevels
    {
        public async Task MessageSent(DiscordClient client, MessageCreateEventArgs args)
        {
            if (args.Author.IsBot)
                return;
            var this_user = await LevelDBInterface.GetValue(args.Author);
            await LevelDBInterface.SetValue(args.Author, this_user is null ? 0 : this_user.CommandLevel, this_user is null ? 1 : this_user.MessageLevel + 1);
        }
        public async Task CommandExecuted(CommandsNextExtension commands, CommandExecutionEventArgs args)
        {
            if (args.Context.Message.Author.IsBot)
                return;
            var this_user = await LevelDBInterface.GetValue(args.Context.Message.Author);
            await LevelDBInterface.SetValue(args.Context.Message.Author, this_user is null ? 0 : this_user.CommandLevel + 1, this_user is null ? 1 : this_user.MessageLevel);
        }
    }
}