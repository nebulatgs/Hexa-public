using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Hexa.Attributes;
using Hexa.Helpers;
using Microsoft.Extensions.Configuration;

namespace Hexa.Modules
{
    public class ActivityRequestBody
    {
        public ActivityRequestBody(string application_id)
        {
            target_application_id = application_id;
        }
        [JsonInclude]
        public int max_age = 86400;
        [JsonInclude]
        public int max_uses = 0;
        [JsonInclude]
        public string target_application_id;
        [JsonInclude]
        public int target_type = 2;
        [JsonInclude]
        public bool temporary = false;
        [JsonInclude]
        public string validate = null;
    }
    [HexaCooldown(5)]
    [Description("Watch Youtube Together")]
    public class YoutubeActivityModule : BaseCommandModule
    {
        [Command("youtube")]
        [Description("Start Youtube Together")]
        [Aliases("yt")]
        [Category(SettingsManager.HexaSetting.GamesCategory)]
        public async Task StartYoutubeAsync(CommandContext ctx, DiscordChannel channel = null)
        {
            var vstat = ctx.Member?.VoiceState;
            if (vstat?.Channel is null && channel is null)
            {
                await ctx.RespondAsync("Please provide a voice channel to start Youtube Together in");
                return;
            }
            if(channel is null)
                channel = vstat.Channel;
            var activity_id = "755600276941176913";
            var code = ActivitiesModule.RequestActivity(activity_id, channel);
            var b = new DiscordMessageBuilder().AddComponents(new DiscordComponent[] { new DiscordLinkButtonComponent($"https://discord.gg/{code}", $" Start Youtube Together", false, new DiscordComponentEmoji(847668867751739435)) }).WithContent(channel.Mention);
            await ctx.RespondAsync(b);
        }
    }

    [Group("activity")]
    [Aliases("act")]
    [HexaCooldown(5)]
    [Description("Start an activity in a voice channel")]
    [Category(SettingsManager.HexaSetting.GamesCategory)]
    public class ActivitiesModule : BaseCommandModule
    {
        public static string RequestActivity(string activity, DiscordChannel channel)
        {
            var reqBody = new ActivityRequestBody(activity);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"https://discord.com/api/v8/channels/{channel.Id}/invites");
            httpWebRequest.ContentType = "application/json";
            var headers = new WebHeaderCollection();

            headers.Add("Authorization", $"Bot {Program.TOKEN}");
            headers.Add("Content-Type", "application/json");
            httpWebRequest.Headers = headers;
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonSerializer.Serialize<ActivityRequestBody>(reqBody);

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var builder = new ConfigurationBuilder().AddJsonStream(httpResponse.GetResponseStream());
            var config = builder.Build();
            return config["code"];
        }

        [GroupCommand]
        [Category(SettingsManager.HexaSetting.GamesCategory)]
        [Description("Start an activity in a voice channel")]
        public async Task StartActivityAsync(CommandContext ctx)
        {
            await ctx.RespondAsync("Please provide an activity to start");
        }

        [Command("yt")]
        [Description("Start Youtube Together")]
        [Aliases("youtube")]
        [Category(SettingsManager.HexaSetting.GamesCategory)]
        public async Task StartYoutubeAsync(CommandContext ctx, DiscordChannel channel = null)
        {
            var vstat = ctx.Member?.VoiceState;
            if (vstat?.Channel is null && channel is null)
            {
                await ctx.RespondAsync("Please provide a voice channel to start Youtube Together in");
                return;
            }
            if(channel is null)
                channel = vstat.Channel;
            var activity_id = "755600276941176913";
            var code = RequestActivity(activity_id, channel);
            var b = new DiscordMessageBuilder().AddComponents(new DiscordComponent[] { new DiscordLinkButtonComponent($"https://discord.gg/{code}", $" Start Youtube Together", false, new DiscordComponentEmoji(847668867751739435)) }).WithContent(channel.Mention);
            await ctx.RespondAsync(b);
        }

        [Command("fish")]
        [Description("Start Fishington.io")]
        [Aliases("fishington")]
        [Category(SettingsManager.HexaSetting.GamesCategory)]
        public async Task StartFishingtonAsync(CommandContext ctx, DiscordChannel channel = null)
        {
            var vstat = ctx.Member?.VoiceState;
            if (vstat?.Channel is null && channel is null)
            {
                await ctx.RespondAsync("Please provide a voice channel to start Fishington.io in");
                return;
            }
            if (channel is null)
                channel = vstat.Channel;
            var activity_id = "814288819477020702";
            var code = RequestActivity(activity_id, channel);
            var b = new DiscordMessageBuilder().AddComponents(new DiscordComponent[] { new DiscordLinkButtonComponent($"https://discord.gg/{code}", $" Start Fishington.io", false, new DiscordComponentEmoji(847667655922024498)) }).WithContent(channel.Mention);
            await ctx.RespondAsync(b);
        }

        [Command("poker")]
        [Description("Start Poker Night")]
        [Category(SettingsManager.HexaSetting.GamesCategory)]
        public async Task StartPokerAsync(CommandContext ctx, DiscordChannel channel = null)
        {
            var vstat = ctx.Member?.VoiceState;
            if (vstat?.Channel is null && channel is null)
            {
                await ctx.RespondAsync("Please provide a voice channel to start Poker Night in");
                return;
            }
            if(channel is null)
                channel = vstat.Channel;
            var activity_id = "755827207812677713";
            var code = RequestActivity(activity_id, channel);
            var b = new DiscordMessageBuilder().AddComponents(new DiscordComponent[] { new DiscordLinkButtonComponent($"https://discord.gg/{code}", $" Start Poker Night", false, new DiscordComponentEmoji(847667610279215144)) }).WithContent(channel.Mention);
            await ctx.RespondAsync(b);
        }

        [Command("betrayal")]
        [Description("Start Betrayal.io")]
        [Aliases("amogus", "amongus")]
        [Category(SettingsManager.HexaSetting.GamesCategory)]
        public async Task StartBetrayalAsync(CommandContext ctx, DiscordChannel channel = null)
        {
            var vstat = ctx.Member?.VoiceState;
            if (vstat?.Channel is null && channel is null)
            {
                await ctx.RespondAsync("Please provide a voice channel to start Betrayal.io in");
                return;
            }
            if(channel is null)
                channel = vstat.Channel;
            var activity_id = "773336526917861400";
            var code = RequestActivity(activity_id, channel);
            var b = new DiscordMessageBuilder().AddComponents(new DiscordComponent[] { new DiscordLinkButtonComponent($"https://discord.gg/{code}", $" Start Betrayal.io", false, new DiscordComponentEmoji(847667494202507264)) }).WithContent(channel.Mention);
            await ctx.RespondAsync(b);
        }

        [Command("chess")]
        [Description("Start Chess in the Park")]
        [Category(SettingsManager.HexaSetting.GamesCategory)]
        public async Task StartChessAsync(CommandContext ctx, DiscordChannel channel = null)
        {
            var vstat = ctx.Member?.VoiceState;
            if (vstat?.Channel is null && channel is null)
            {
                await ctx.RespondAsync("Please provide a voice channel to start Chess in the Park in");
                return;
            }
            if(channel is null)
                channel = vstat.Channel;
            var activity_id = "832012586023256104";
            var code = RequestActivity(activity_id, channel);
            var b = new DiscordMessageBuilder().AddComponents(new DiscordComponent[] { new DiscordLinkButtonComponent($"https://discord.gg/{code}", $" Start Chess in the Park (Beta)", false, new DiscordComponentEmoji(847669270409904159)) }).WithContent(channel.Mention);
            await ctx.RespondAsync(b);
        }
    }
}