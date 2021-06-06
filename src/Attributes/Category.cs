using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharp​Plus.CommandsNext.Attributes;
using Hexa.Helpers;

namespace Hexa.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class CategoryAttribute : CheckBaseAttribute
    {
        public SettingsManager Manager { get; }
        SettingsManager.HexaSetting Category { get; }
        public CategoryAttribute(SettingsManager.HexaSetting setting)
        {
            Manager = new SettingsManager();
            Category = setting;
            var settingResult = Manager.GetSettingDefinitionAsync(Category).GetAwaiter().GetResult();
            Name = settingResult.Name;
            Description = settingResult.Description;
        }

        public string Name { get; }
        public string Description { get; }

        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            await Manager.SetDefaults(ctx.Guild);
            var dbSetting = await Manager.GetSetting(ctx.Guild, Category);
            bool check = bool.Parse(dbSetting.Value);
            if (!check && !help)
                await ctx.RespondAsync("That category has been disabled by an admin");
            return check;
        }
    }
}