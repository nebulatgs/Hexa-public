using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace Hexa
{
    public class HexaSettings
    {
        public enum SettingType { GhostPing, ModLog, ServerPrefix, ToggleIndex = 2 }

        public static SettingType SettingTypeFromString(string setting_type)
        {
            switch (setting_type)
            {
                case "ghost":
                case "ghostping":
                case "gping":
                case "pingdetect":
                    return SettingType.GhostPing;

                case "modlog":
                case "log":
                case "logs":
                case "logging":
                    return SettingType.ModLog;
                case "prefix":
                case "serverprefix":
                case "commandprefix":
                    return SettingType.ServerPrefix;
            }

            throw new Exception("That setting does not exist");
        }

        public static string StringFromSettingType(SettingType setting_type)
        {
            switch (setting_type)
            {
                case SettingType.GhostPing:
                    return "Ghost Ping Detection";
                case SettingType.ModLog:
                    return "Mod Log";
                case SettingType.ServerPrefix:
                    return "Server Prefix";
            }
            throw new Exception("That setting does not exist");
        }
        public static async Task<string> GetValue(DiscordGuild guild, SettingType setting_type)
        {
            var instance = Supabase.Client.Instance;
            var channels = instance.From<GuildSetting>();
            var guildSettings = await channels.Get();
            var foundSettings = guildSettings.Models.Where(x => x.GuildId == guild.Id && x.SettingTypeId == (int)setting_type);
            if (foundSettings.Count() == 0)
                return null;
            else
                return foundSettings.First().Value;
        }
        public static async Task<IEnumerable<GuildSetting>> GetValuesAsync(DiscordGuild guild)
        {
            var instance = Supabase.Client.Instance;
            var channels = instance.From<GuildSetting>();
            var guildSettings = await channels.Get();
            var foundSettings = guildSettings.Models.Where(x => x.GuildId == guild.Id);
            return foundSettings;
        }
        public static async Task SetValue(DiscordGuild guild, string find, string value)
        {
            var setting_type = SettingTypeFromString(find);
            var instance = Supabase.Client.Instance;
            var new_setting = new GuildSetting
            {
                GuildId = guild.Id,
                SettingTypeId = (int)setting_type,
                Value = value
            };
            var channels = instance.From<GuildSetting>();
            var guildSettings = await channels.Get();
            var foundSettings = guildSettings.Models.Where(x => x.GuildId == guild.Id);
            if (foundSettings.Where(x => x.SettingTypeId == (int)setting_type).Count() == 0)
            {
                if (guildSettings.Models.Count() != 0)
                    new_setting.SettingId = (int)guildSettings.Models.Last().PrimaryKeyValue + 1;
                await channels.Insert(new_setting);
            }
            else
            {
                new_setting.SettingId = (int)foundSettings.Where(x => x.SettingTypeId == (int)setting_type).First().PrimaryKeyValue;
                await channels.Update(new_setting);
            }
        }
    }
}