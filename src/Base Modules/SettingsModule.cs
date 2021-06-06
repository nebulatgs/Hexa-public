using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;

using DSharp​Plus.CommandsNext.Attributes;

using Hexa.Attributes;
using Hexa.Database;
using Hexa.Helpers;

namespace Hexa.Modules
{

    [GuildOnly]
    [HexaCooldown(5)]
    [RequireUserPermissions(Permissions.ManageGuild)]
    public class SettingsModule : BaseCommandModule
    {

        public SettingsManager Manager { get; set; }


        //     public async Task SettingsCommand(CommandContext ctx, [Description("The setting to change")] string setting = null, [RemainingText, Description("The new value")] string action = null)
        //     {
        //         var hEmbed = new HexaEmbed(ctx, "settings");
        //         var toggles = HexaSettings.GetValues(ctx.Guild);
        //         if (setting is null)
        //         {
        //             if (toggles is null)
        //             {
        //                 await Manager.SetDefaults(ctx.Guild);
        //                 toggles = HexaSettings.GetValues(ctx.Guild);    
        //             }
        //             foreach (var toggle in toggles)
        //             {
        //                 string value = toggle.Value;
        //                 try { value = bool.Parse(toggle.Value) ? "Enabled" : "Disabled"; } catch { }
        //                 hEmbed.embed.AddField(
        //                     name: HexaSettings.StringFromSettingType((HexaSettings.SettingType)toggle.Setting),
        //                     value: value,
        //                     inline: true
        //                 );
        //             }
        //             await ctx.RespondAsync(embed: hEmbed.Build());
        //             return;
        //         }
        //         var parsedAction = action;
        //         await Manager.SetSetting(ctx.Guild, ((int)setting), parsedAction); 
        //         toggles = HexaSettings.GetValues(ctx.Guild);
        //         foreach (var toggle in toggles)
        //         {
        //             string value = toggle.Value;
        //             try { value = bool.Parse(toggle.Value) ? "Enabled" : "Disabled"; } catch { }
        //             hEmbed.embed.AddField(
        //                 name: HexaSettings.StringFromSettingType((HexaSettings.SettingType)toggle.Setting),
        //                 value: value,
        //                 inline: true
        //             );
        //         }

        //         await ctx.RespondAsync(embed: hEmbed.Build());
        //     }
        // }
        private async Task ShowSettingsAsync(CommandContext ctx)
        {
            await Manager.SetDefaults(ctx.Guild);
            var hEmbed = new HexaEmbed(ctx, "settings");
            var settings = (await Manager.GetSettings(ctx.Guild));
            var maxLength = 0;
            settings.ForEach(x => maxLength = maxLength < $"``{x.Setting.Name} ({x.Setting.Aliases.FirstOrDefault()})".Length + Math.Max(14, x.Value.Length + 3) + 4 ? $"``{x.Setting.Name} ({x.Setting.Aliases.FirstOrDefault()})".Length + Math.Max(14, x.Value.Length + 3) + 4 : maxLength);
            // maxLength += 15;
            var groupedSettings = settings.OrderByDescending(x => x.Value).GroupBy(x => x.Setting.Type);
            // var toggles = settings.Where(x => x.Setting.Type == "Toggle");
            foreach (var group in groupedSettings)
            {
                var name = group.FirstOrDefault().Setting.Type;
                name = name.Last() == 'y' ? name.TrimEnd('y') + "ies" : name + "s";
                var value = "```diff\n";
                foreach (var setting in group)
                {
                    var tempVal = setting.Value;
                    try { tempVal = bool.Parse(setting.Value) ? "Enabled" : "Disabled"; } catch { }
                    var diffChar = tempVal == "Enabled" ? "+" : tempVal == "Disabled" ? "-" : "•";
                    string beginning = $"{diffChar} {setting.Setting.Name} ({setting.Setting.Aliases.FirstOrDefault()})";
                    value += beginning + ($"{tempVal}\n".PadLeft(maxLength - beginning.Length));
                }
                value += "```";
                hEmbed.embed.AddField(
                    name: name,
                    value: value,
                    inline: false
                );
            }
            await ctx.RespondAsync(hEmbed.Build());
        }
        [Command("settings")]
        [Aliases("setting", "set")]
        [Description("Change server-specific settings")]
        // [Category(SettingsManager.HexaSetting.UtilityCategory)]
        public async Task SettingsCommand(CommandContext ctx)
        {
            await ShowSettingsAsync(ctx);
        }

        [Command("settings")]
        public async Task SettingsCommand(CommandContext ctx, [Description("The setting to get")] SettingsManager.HexaSetting setting)
        {
            await Manager.SetDefaults(ctx.Guild);
            var dbSetting = await Manager.GetSetting(ctx.Guild, setting);
            var hEmbed = new HexaEmbed(ctx, "setting info");
            hEmbed.embed.WithTitle($"setting ``{dbSetting.Setting.Aliases.First()}``");
            hEmbed.embed.AddField(
                "Name",
                dbSetting.Setting.Name,
                true
            );

            hEmbed.embed.AddField(
                "Type",
                dbSetting.Setting.Type,
                true
            );
            var tempVal = dbSetting.Value;
            try { tempVal = bool.Parse(dbSetting.Value) ? "Enabled" : "Disabled"; } catch { }
            var diffChar = tempVal == "Enabled" ? "+" : tempVal == "Disabled" ? "-" : "";
            // string beginning = $"{diffChar} {dbSetting.Setting.Name} ({dbSetting.Setting.Aliases.FirstOrDefault()})";
            hEmbed.embed.AddField(
                "Current Value",
                $"```diff\n{diffChar} {tempVal} \n```",
                false
            );
            await ctx.RespondAsync(hEmbed.Build());
        }

        [Command("settings")]
        public async Task SettingsCommand(CommandContext ctx, [Description("The setting to change")] SettingsManager.HexaSetting setting, [RemainingText, Description("The new value")] bool action)
        {
            var def = await Manager.GetSettingDefinitionAsync(setting);
            if(!(def.Type == "Toggle" || def.Type == "Category"))
                throw new ArgumentException("That setting is not a toggle");
            await Manager.SetSetting(ctx.Guild, ((int)setting), action.ToString());
            await ShowSettingsAsync(ctx);
        }

        [Command("settings")]
        [HelpHide]
        public async Task SettingsCommand(CommandContext ctx, [Description("The setting to change")] SettingsManager.HexaSetting setting, [RemainingText, Description("The new value")] int action)
        {
            var def = await Manager.GetSettingDefinitionAsync(setting);
            if((def.Type == "Toggle" || def.Type == "Category"))
                throw new ArgumentException("That setting is not a number");
            await Manager.SetSetting(ctx.Guild, ((int)setting), action.ToString());
            await ShowSettingsAsync(ctx);
        }

        [Command("settings")]
        [HelpHide]
        public async Task SettingsCommand(CommandContext ctx, [Description("The setting to change")] SettingsManager.HexaSetting setting, [RemainingText, Description("The new value")] string action)
        {
            var def = await Manager.GetSettingDefinitionAsync(setting);
            if((def.Type == "Toggle" || def.Type == "Category"))
                throw new ArgumentException("That setting is not a string");
            await Manager.SetSetting(ctx.Guild, ((int)setting), action);
            await ShowSettingsAsync(ctx);
        }

        [Command("settings")]
        [HelpHide]
        public Task SettingsCommand(CommandContext ctx, [RemainingText] string setting)
        {
            throw new ArgumentException("That's not a valid setting");
        }
    }
}