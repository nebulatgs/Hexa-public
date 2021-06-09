using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Hexa.Attributes;
using Hexa.Helpers;
using ForecastIO;

namespace Hexa.Modules
{
    [HexaCooldown(60)]
    [Hidden, Disabled]
    public class WeatherModule : BaseCommandModule
    {
        [Command("weather")]
        [Description("Get the current weather")]
        [Category(SettingsManager.HexaSetting.UtilityCategory)]
        public async Task WeatherCommand(CommandContext ctx)        
        {
            // var request = new ForecastIORequest(Program.DSKEY, 37.8267f, -122.423f, Unit.si);
            // var response = request.Get();
            var request = new ForecastIORequest(Program.DSKEY, 43.4499376f, -79.7880999f, Unit.si);
            var response = await request.GetAsync();
            var hEmbed = new HexaEmbed(ctx, "weather");
            hEmbed.embed.WithDescription(response.currently.temperature.ToString());
            await ctx.RespondAsync(hEmbed.Build());
            // await ShowSettingsAsync(ctx);
        }
    }
}