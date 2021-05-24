using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
// using Discord.Commands;
// using Discord.WebSocket;
using DSharpPlus.Entities;
using Hexa.Attributes;
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

    [Group("activity")]
    [Aliases("act")]
    [HexaCooldown(5)]
    public class ActivitiesModule : BaseCommandModule
    {
        private string RequestActivity(string activity, DiscordChannel channel)
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
        public async Task StartActivityAsync(CommandContext ctx)
        {
            await ctx.RespondAsync("Please select a valid activity");
        }

        [Command("yt")]
        [Description("Start Youtube Together")]
        [Aliases("youtube")]
        public async Task StartYoutubeAsync(CommandContext ctx, DiscordChannel channel = null)
        {
            if (channel == null)
            {
                await ctx.RespondAsync("Please select a voice channel");
                return;
            }
            var activity_id = "755600276941176913";
            var code = RequestActivity(activity_id, channel);
            await ctx.RespondAsync($"https://discord.gg/{code}");
        }

        [Command("fish")]
        [Description("Start Fishington.io")]
        [Aliases("fishington")]
        public async Task StartFishingtonAsync(CommandContext ctx, DiscordChannel channel = null)
        {
            if (channel == null)
            {
                await ctx.RespondAsync("Please select a voice channel");
                return;
            }
            var activity_id = "814288819477020702";
            var code = RequestActivity(activity_id, channel);
            await ctx.RespondAsync($"https://discord.gg/{code}");
        }

        [Command("poker")]
        [Description("Start Poker Night")]
        public async Task StartPokerAsync(CommandContext ctx, DiscordChannel channel = null)
        {
            if (channel == null)
            {
                await ctx.RespondAsync("Please select a voice channel");
                return;
            }
            var activity_id = "755827207812677713";
            var code = RequestActivity(activity_id, channel);
            await ctx.RespondAsync($"https://discord.gg/{code}");
        }

        [Command("betrayal")]
        [Description("Start Betrayal.io")]
        [Aliases("amogus", "amongus")]
        public async Task StartBetrayalAsync(CommandContext ctx, DiscordChannel channel = null)
        {
            if (channel == null)
            {
                await ctx.RespondAsync("Please select a voice channel");
                return;
            }
            var activity_id = "773336526917861400";
            var code = RequestActivity(activity_id, channel);
            await ctx.RespondAsync($"https://discord.gg/{code}");
        }
    }
}