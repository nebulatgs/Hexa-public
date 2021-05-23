using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharp​Plus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Hexa.Attributes;

namespace Hexa.Modules
{

    [GuildOnly]
    [HexaCooldown(5)]
    [RequireUserPermissions(Permissions.ManageGuild)]
    public class SettingsModule : BaseCommandModule
    {
        [Command("settings")]
        [Aliases("set")]
        public async Task SettingsCommand(CommandContext ctx, string setting = null, string action = null)
        {
            var hEmbed = new HexaEmbed(ctx, "Settings");
            var toggles = await HexaSettings.GetTogglesAsync(ctx.Guild);
            if (setting == null)
            {
                foreach (var toggle in toggles)
                {
                    string value = toggle.Value;
                    try { value = bool.Parse(toggle.Value) ? "Enabled" : "Disabled"; } catch { }
                    hEmbed.embed.AddField(
                        name: HexaSettings.StringFromSettingType((HexaSettings.SettingType)toggle.SettingTypeId),
                        value: value,
                        inline: true
                    );
                }
                await ctx.RespondAsync(embed: hEmbed.Build());
                return;
            }
            var parsedAction = action;
            HexaSettings.SettingType setting_type;
            try { setting_type = HexaSettings.SettingTypeFromString(setting); }
            catch (Exception ex) { await ctx.RespondAsync(ex.Message); return; }
            if (HexaSettings.SettingTypeFromString(setting) < HexaSettings.SettingType.ToggleIndex)
            {
                switch (action)
                {
                    case "enable":
                    case "on":
                    case "true":
                    case "1":
                    case "one":
                    case "yes":
                        parsedAction = "true";
                        break;
                    case "disable":
                    case "off":
                    case "false":
                    case "0":
                    case "zero":
                    case "no":
                        parsedAction = "false";
                        break;
                    default:
                        await ctx.RespondAsync($"Invalid value for setting {setting}");
                        return;
                }
            }
            try { await HexaSettings.SetToggle(ctx.Guild, setting, parsedAction); }
            catch (Exception ex) { await ctx.RespondAsync(ex.Message); return; }
            toggles = await HexaSettings.GetTogglesAsync(ctx.Guild);
            foreach (var toggle in toggles)
            {
                string value = toggle.Value;
                try { value = bool.Parse(toggle.Value) ? "Enabled" : "Disabled"; } catch { }
                hEmbed.embed.AddField(
                    name: HexaSettings.StringFromSettingType((HexaSettings.SettingType)toggle.SettingTypeId),
                    value: value,
                    inline: true
                );
            }

            await ctx.RespondAsync(embed: hEmbed.Build());
        }

        // [Command("enable")]
        // private async Task EnableCommand(CommandContext ctx, string name)
        // {
        //     try { await HexaSettings.SetToggle(ctx.Guild, name, "true"); }
        //     catch (Exception ex) { await ctx.RespondAsync(ex.Message); return; }

        //     // var hEmbed = new HexaEmbed(ctx, "Settings");

        //     //  hEmbed.embed.AddField(
        //     //         name: HexaSettings.StringFromSettingType((HexaSettings.SettingType)toggle.SettingTypeId),
        //     //         value: "Enabled",
        //     //         inline: true
        //     //  );

        //     // await ctx.RespondAsync(embed: hEmbed.Build());
        // }

        // // [Command("disable")]
        // private async Task DisableCommand(CommandContext ctx, string name)
        // {
        //     try { await HexaSettings.SetToggle(ctx.Guild, name, "false"); }
        //     catch (Exception ex) { await ctx.RespondAsync(ex.Message); return; }

        //     var hEmbed = new HexaEmbed(ctx, "Settings");

        //     hEmbed.embed.AddField(
        //            name: "test",
        //            value: "Disabled",
        //            inline: true
        //     );

        //     await ctx.RespondAsync(embed: hEmbed.Build());
        // }
    }
}