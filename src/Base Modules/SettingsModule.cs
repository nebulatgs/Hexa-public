using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;

using DSharp​Plus.CommandsNext.Attributes;

using Hexa.Attributes;
using Hexa.Helpers;

namespace Hexa.Modules
{

    [GuildOnly]
    [HexaCooldown(5)]
    [RequireUserPermissions(Permissions.ManageGuild)]
    public class SettingsModule : BaseCommandModule
    {
        [Command("settings")]
        [Aliases("set")]
        [Description("Change server-specific settings")]
        [Category("Utilities")]
        public async Task SettingsCommand(CommandContext ctx, [Description("The setting to change")] string setting = null, [RemainingText, Description("The new value")]string action = null)
        {
            var hEmbed = new HexaEmbed(ctx, "settings");
            var toggles = await HexaSettings.GetValuesAsync(ctx.Guild);
            if (setting is null)
            {
                if(toggles.Count() == 0)
                {
                    
                }
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
            try { await HexaSettings.SetValue(ctx.Guild, setting, parsedAction); }
            catch (Exception ex) { await ctx.RespondAsync(ex.Message); return; }
            toggles = await HexaSettings.GetValuesAsync(ctx.Guild);
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
    }
}