using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Hexa.Database;
using Microsoft.EntityFrameworkCore;

namespace Hexa.Helpers
{
    public class SettingsManager
    {
        public enum HexaSetting
        {
            GhostPing = 1,
            ServerPrefix,
            ModLog,
            // Categories
            GamesCategory,
            FunCategory,
            RandomCategory,
            UtilityCategory,
            InfoCategory,
            AdminCategory,
            DangerCategory
        }
        private static List<GuildSetting> guildSettings { get; set; }

        public SettingsManager()
        {
            using (var db = new HexaContext())
            {
                var settings = db.GuildSettings.Include(y => y.Setting);
                // return await settings.ToDictionaryAsync(x => x.SettingID);
            guildSettings =  settings.ToList();
            }
        }

        public async Task<List<GuildSetting>> GetSettings(DiscordGuild guild)
        {
            await SetDefaults(guild);
            var guildId = guild is null ? 835722357485994005 : guild.Id;
            return guildSettings.Where(x => x.GuildId == guildId).ToList();
            // using (var db = new HexaContext())
            // {
            //     var settings = db.GuildSettings.Include(y => y.Setting).Where(x => x.GuildId == guildId);
            //     // return await settings.ToDictionaryAsync(x => x.SettingID);
            //     return await settings.ToListAsync();
            // }
        }

        public async Task<GuildSetting> GetSetting(DiscordGuild guild, HexaSetting setting)
        {
            await SetDefaults(guild);
            var guildId = guild is null ? 835722357485994005 : guild.Id;
            int setting_int = ((int)setting);
            return guildSettings.Where(x => x.GuildId == guildId).First(x => x.SettingID == setting_int);
            // using (var db = new HexaContext())
            // {
            //     var settings = db.GuildSettings.Include(y => y.Setting);
            //     var first = await settings.Where(x => x.GuildId == guildId).FirstAsync(x => x.SettingID == setting_int);
            //     return first;
            // }
        }

        public async Task<Setting> GetSettingDefinitionAsync(HexaSetting setting)
        {
            using (var db = new HexaContext())
            {
                int setting_int = ((int)setting);
                var def = await db.Settings.FirstAsync(x => x.SettingID == setting_int);
                return def;
            }
        }

        public async Task<Setting> GetSettingDefinitionAsync(int setting)
        {
            using (var db = new HexaContext())
            {
                var def = await db.Settings.FirstAsync(x => x.SettingID == setting);
                return def;
            }
        }

        public async Task SetSetting(DiscordGuild guild, int setting, string value)
        {
            var guildId = guild is null ? 835722357485994005 : guild.Id;
            using (var db = new HexaContext())
            {
                var foundSetting = db.GuildSettings.SingleOrDefault(x => x.GuildId == guildId && x.SettingID == setting);
                var obj = guildSettings.SingleOrDefault(x => x.GuildId == guildId && x.SettingID == setting);
                foundSetting.
                    GuildId = guildId;
                obj.
                    GuildId = guildId;
                foundSetting.
                    SettingID = ((int)setting);
                obj.
                    SettingID = ((int)setting);
                foundSetting.
                    Value = value;
                obj.
                    Value = value;

                // db.Entry(foundSetting).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        public async Task SetDefaults(DiscordGuild guild)
        {
            var guildId = guild is null ? 835722357485994005 : guild.Id;
            using (var db = new HexaContext())
            {
                var allSettings = db.Settings.ToList();
                foreach (var setting in allSettings)
                {
                    var set1 = db.GuildSettings.Where(x => x.GuildId == guildId);
                    var set2 = set1.Where(x => x.SettingID == setting.SettingID);
                    // Console.WriteLine(set2.FirstOrDefault().Value);
                    // var set3 = set2 is null;
                    var set3 = set2.Any();
                    if (!set3)
                    {
                        var settingObj = new GuildSetting()
                        {
                            GuildId = guildId,
                            SettingID = setting.SettingID,
                            Value = setting.Default
                        };
                        db.Add(settingObj);
                        guildSettings.Add(settingObj);
                    }
                }
                await db.SaveChangesAsync();
            }
        }
    }
}