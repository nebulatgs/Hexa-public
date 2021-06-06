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

        public async Task<List<GuildSetting>> GetSettings(DiscordGuild guild)
        {
            using (var db = new HexaContext())
            {
                var settings = db.GuildSettings.Include(y => y.Setting).Where(x => x.GuildId == guild.Id);
                // return await settings.ToDictionaryAsync(x => x.SettingID);
                return await settings.ToListAsync();
            }
        }

        public async Task<GuildSetting> GetSetting(DiscordGuild guild, HexaSetting setting)
        {
            using (var db = new HexaContext())
            {
                int setting_int = ((int)setting);
                var settings = await db.GuildSettings.Include(y => y.Setting).FirstOrDefaultAsync(x => x.GuildId == guild.Id && x.SettingID == setting_int);
                return settings;
            }
        }

        public async Task<Setting> GetSettingDefinitionAsync(HexaSetting setting)
        {
            using (var db = new HexaContext())
            {
                int setting_int = ((int)setting);
                var def = await db.Settings.FirstOrDefaultAsync(x => x.SettingID == setting_int);
                return def;
            }
        }

        public async Task<Setting> GetSettingDefinitionAsync(int setting)
        {
            using (var db = new HexaContext())
            {
                var def = await db.Settings.FirstOrDefaultAsync(x => x.SettingID == setting);
                return def;
            }
        }

        public async Task SetSetting(DiscordGuild guild, int setting, string value)
        {
            using (var db = new HexaContext())
            {
                var foundSetting = db.GuildSettings.SingleOrDefault(x => x.GuildId == guild.Id && x.SettingID == setting);
                foundSetting.
                    GuildId = guild.Id;
                foundSetting.
                    SettingID = ((int)setting);
                foundSetting.
                    Value = value;

                // db.Entry(foundSetting).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        public async Task SetDefaults(DiscordGuild guild)
        {
            using (var db = new HexaContext())
            {
                // var prefix = ((int)HexaSetting.ServerPrefix);
                // if (!(db.GuildSettings.Where(x => x.GuildId == guild.Id && x.SettingID == prefix).Count() > 0))
                //     db.Add(new GuildSetting()
                //     {
                //         GuildId = guild.Id,
                //         SettingID = prefix,
                //         Value = Environment.GetEnvironmentVariable("PROD") is not null ? "-" : "+"
                //     });
                // var modlog = ((int)HexaSetting.ServerPrefix);
                // if (!(db.GuildSettings.Where(x => x.GuildId == guild.Id && x.SettingID == modlog).Count() > 0))
                // db.Add(new GuildSetting()
                // {
                //     GuildId = guild.Id,
                //     SettingID = (int)HexaSetting.ModLog,
                //     Value = "false"
                // });
                // db.Add(new GuildSetting()
                // {
                //     GuildId = guild.Id,
                //     SettingID = (int)HexaSetting.GhostPing,
                //     Value = "false"
                // });
                var allSettings = await db.Settings.ToListAsync();
                foreach (var setting in allSettings)
                {
                    if (!(db.GuildSettings.Where(x => x.GuildId == guild.Id && x.SettingID == setting.SettingID).Count() > 0))
                        db.Add(new GuildSetting()
                        {
                            GuildId = guild.Id,
                            SettingID = setting.SettingID,
                            Value = setting.Default
                        });
                }
                await db.SaveChangesAsync();
            }
        }
    }
}