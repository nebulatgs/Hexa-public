// using System;
// using System.Collections.Generic;
// using System.Data.Entity;
// using System.Linq;
// using System.Threading.Tasks;

// using DSharpPlus.Entities;
// using Hexa.Database;

// namespace Hexa
// {
//     public class HexaSettings
//     {
//         public enum SettingType { GhostPing = 1, ServerPrefix, ModLog, ToggleIndex = 2 }
//         // public static List<GuildSetting>
//         public static SettingType SettingTypeFromString(string setting_type)
//         {
//             switch (setting_type)
//             {
//                 case "ghost":
//                 case "ghostping":
//                 case "gping":
//                 case "pingdetect":
//                     return SettingType.GhostPing;

//                 case "modlog":
//                 case "log":
//                 case "logs":
//                 case "logging":
//                     return SettingType.ModLog;

//                 case "prefix":
//                 case "serverprefix":
//                 case "commandprefix":
//                     return SettingType.ServerPrefix;
//             }

//             throw new Exception("That setting does not exist");
//         }

//         public static string StringFromSettingType(SettingType setting_type)
//         {
//             switch (setting_type)
//             {
//                 case SettingType.GhostPing:
//                     return "Ghost Ping Detection";
//                 case SettingType.ModLog:
//                     return "Mod Log";
//                 case SettingType.ServerPrefix:
//                     return "Server Prefix";
//             }
//             throw new Exception("That setting does not exist");
//         }
//         public static string GetValue(DiscordGuild guild, SettingType setting_type)
//         {
//             // var instance = Supabase.Client.Instance;
//             // var channels = instance.From<GuildSetting>();
//             // var GuildSettings = await channels.Get();
//             // var foundSettings = GuildSettings.Models.Where(x => x.GuildId == guild.Id && x.SettingType == (int)setting_type);
//             using (var db = new HexaContext())
//             {
//                 var foundSettings = db.GuildSettings.Where(x => x.GuildId == guild.Id && x.SettingID == (int)setting_type);
//                 // if (foundSettings.Count() == 0)
//                     // return null;
//                 // else
//                 // {
//                     var first = foundSettings.FirstOrDefault();
//                     return first.Value;
//                 // }
//             }
//         }
//         public static List<GuildSetting> GetValues(DiscordGuild guild)
//         {
//             using (var db = new HexaContext())
//             {
//                 var foundSettings = db.GuildSettings.Include("Settings");//.Where(x => x.GuildId == guild.Id);
//                 if (foundSettings.Count() == 0)
//                     return null;
//                 return foundSettings.ToList();
//             }
//         }
//         public static async Task SetValueAsync(DiscordGuild guild, string key, string value)
//         {
//             var setting_type = SettingTypeFromString(key);
//             // var instance = Supabase.Client.Instance;
//             var new_setting = new GuildSetting
//             {
//                 GuildId = guild.Id,
//                 SettingID = ((int)setting_type),
//                 Value = value
//             };
//             using (var db = new HexaContext())
//             {
//                 var foundSettings = db.GuildSettings.Where(x => x.GuildId == guild.Id && x.Setting == new_setting.Setting);
//                 // if (foundSettings.Count() == 0)
//                     // await db.GuildSettings.AddAsync(new_setting);
//                 // else
//                     db.GuildSettings.Update(new_setting);
//                     // await channels.Update(new_setting);
//                     // new_setting.SettingID = (int)foundSettings.Where(x => x.SettingType == (int)setting_type).First().SettingID;

//                 await db.SaveChangesAsync();
//             }
//         }
//     }
// }