using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using Hexa.Database;
using Hexa.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Hexa.Converters
{
    public class HexaSettingConverter : IArgumentConverter<SettingsManager.HexaSetting>
    {
        public async Task<Optional<SettingsManager.HexaSetting>> ConvertAsync(string value, CommandContext ctx)
        {
            using(var db = new HexaContext())
            {
                var setting = db.Settings.Where(x => x.Aliases.Contains(value));
                if (setting.Count() > 0)
                    return Optional.FromValue((SettingsManager.HexaSetting)(await setting.FirstAsync()).SettingID);
                else
                    throw new Exception("Invalid setting name");
                    // return Optional.FromNoValue<SettingsManager.HexaSetting>();
            }
        }
    }
}