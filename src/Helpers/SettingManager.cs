using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using Hexa.Database;

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
            DangerCategory,
            VoiceCategory
        }
        private static List<GuildSettingAddable> guildSettings { get; set; }
        public static List<SettingDef> guildSettingDefs { get; set; }

        public SettingsManager()
        {
            // using (var db = new HexaContext())
            // {
            // var settings = db.GuildSettings.Include(y => y.Setting);
            // return await settings.ToDictionaryAsync(x => x.SettingID);
            // guildSettings =  settings.ToList();
            // }
            var instance = Supabase.Client.Instance;
            var channels = instance.From<GuildSettingAddable>();
            var def_channels = instance.From<SettingDef>();
            var guild_setting_table = channels.Get().GetAwaiter().GetResult();
            var guild_setting_def_table = def_channels.Get().GetAwaiter().GetResult();
            guildSettings = guild_setting_table.Models;
            guildSettingDefs = guild_setting_def_table.Models;
        }

        public async Task<List<GuildSetting>> GetSettings(DiscordGuild guild)
        {
            await SetDefaults(guild);
            var guildId = guild is null ? 847891805185245217 : guild.Id;
            return guildSettings.
                Select(x => x.ToGuildSetting()).
                Where(x => x.GuildId == guildId).
                Join(// outer sequence 
                    guildSettingDefs,  // inner sequence 
                    guildSetting => guildSetting.SettingID,    // outerKeySelector
                    guildSettingDef => guildSettingDef.SettingID,  // innerKeySelector
                    (guildSetting, guildSettingDef) =>  // result selector
                    {
                        guildSetting.Setting = guildSettingDef;
                        return guildSetting;
                    }).
                ToList();
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
            var guildId = guild is null ? 847891805185245217 : guild.Id;
            int setting_int = ((int)setting);
            return guildSettings.
                Select(x => x.ToGuildSetting()).
                Where(x => x.GuildId == guildId).
                Join(// outer sequence 
                    guildSettingDefs,  // inner sequence 
                    guildSetting => guildSetting.SettingID,    // outerKeySelector
                    guildSettingDef => guildSettingDef.SettingID,  // innerKeySelector
                    (guildSetting, guildSettingDef) =>  // result selector
                    {
                        guildSetting.Setting = guildSettingDef;
                        return guildSetting;
                    }).
                First(x => x.SettingID == setting_int);
        }

        public async Task<SettingDef> GetSettingDefinitionAsync(HexaSetting setting)
        {
            // using (var db = new HexaContext())
            // {
            int setting_int = ((int)setting);
            // var def = await db.Settings.FirstAsync(x => x.SettingID == setting_int);
            var def = guildSettingDefs.First(x => x.SettingID == setting_int);
            return def;
            // }
        }

        public async Task<SettingDef> GetSettingDefinitionAsync(int setting)
        {
            var def = guildSettingDefs.First(x => x.SettingID == setting);
            return def;
        }

        public async Task SetSetting(DiscordGuild guild, int setting, string value)
        {
            var guildId = guild is null ? 847891805185245217 : guild.Id;
            // using (var db = new HexaContext())
            // {
            // var foundSetting = guildSettings.SingleOrDefault(x => x.GuildId == guildId && x.SettingID == setting);
            var obj = guildSettings.SingleOrDefault(x => x.GuildId == guildId && x.SettingID == setting);
            // foundSetting.
            //     GuildId = guildId;
            obj.
                GuildId = guildId;
            // foundSetting.
            //     SettingID = ((int)setting);
            obj.
                SettingID = ((int)setting);
            // foundSetting.
            //     Value = value;
            obj.
                Value = value;

            var instance = Supabase.Client.Instance;
            var channels = instance.From<GuildSettingAddable>();
            await channels.Update(obj);

            // db.Entry(foundSetting).State = EntityState.Modified;
            // await db.SaveChangesAsync();
            // }
        }

        public async Task SetDefaults(DiscordGuild guild)
        {
            var guildId = guild is null ? 847891805185245217 : guild.Id;
            // using (var db = new HexaContext())
            // {
            // var allSettings = guildSettingDefs;
            var instance = Supabase.Client.Instance;
            var channels = instance.From<GuildSettingAddable>();
            // await channels.Update(obj);
            foreach (var setting in guildSettingDefs)
            {
                var set1 = guildSettings.Where(x => x.GuildId == guildId);
                var set2 = set1.Where(x => x.SettingID == setting.SettingID);
                // Console.WriteLine(set2.FirstOrDefault().Value);
                // var set3 = set2 is null;
                var set3 = set2.Any();
                if (!set3)
                {
                    var settingObj = new GuildSettingAddable()
                    {
                        GuildId = guildId,
                        SettingID = setting.SettingID,
                        Value = setting.Default
                    };
                    
                    // db.Add(settingObj);
                    await channels.Insert(settingObj);
                    guildSettings.Add(settingObj);
                }
            }
            // await db.SaveChangesAsync();
            // }
        }
    }
}