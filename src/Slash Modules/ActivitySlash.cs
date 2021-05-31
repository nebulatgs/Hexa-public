using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Hexa.Helpers;
using Microsoft.Extensions.Configuration;

namespace Hexa.Modules
{

    public class ActivitySlash : SlashCommandModule
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

        public enum ActivityEnum
        {
            [ChoiceName("Youtube Together")]
            yt,
            [ChoiceName("Fishington.io")]
            fish,
            [ChoiceName("Poker Night")]
            poker,
            [ChoiceName("Betrayal.io")]
            betrayal,
            [ChoiceName("Chess in the Park")]
            chess
        }

        public static string[] ActivityIds = {
            // Youtube Together
            "755600276941176913",
            // Fishington.io
            "814288819477020702",
            // Poker Night
            "755827207812677713",
            // Betrayal.io
            "773336526917861400",
            // Chess in the Park
            "832012586023256104"
        };

        public static ulong[] EmojiIds = {
            // Youtube Together
            847668867751739435,
            // Fishington.io
            847667655922024498,
            // Poker Night
            847667610279215144,
            // Betrayal.io
            847667494202507264,
            // Chess in the Park
            847669270409904159
        };

        [SlashCommand("activity", "Start an activity in a voice channel")]
        public async Task ActivityCommand(InteractionContext ctx, [Option("channel", "The voice channel to start the activity in")] DiscordChannel channel, [Option("activity", "The activity to start")] ActivityEnum activity)
        {
            if (channel.Type != ChannelType.Voice)
                return;
            var activity_id = (ulong)activity;
            var code = RequestActivity(ActivityIds[(int)activity], channel);
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithComponents(
                    new DiscordComponent[] 
                        {
                            new DiscordLinkButtonComponent(
                                $"https://discord.gg/{code}",
                                $" Start {activity.GetAttributeOfType<ChoiceNameAttribute>().Name}",
                                false,
                                new DiscordComponentEmoji(EmojiIds[(int)activity])
                            )
                        }
                ).WithContent(channel.Mention)
            );  
        }

        [SlashCommand("youtube", "Watch Youtube Together in a voice channel")]
        public async Task YoutubeCommand(InteractionContext ctx, [Option("channel", "The voice channel to start Youtube Together in")] DiscordChannel channel)
        {
            if (channel.Type != ChannelType.Voice)
                return;
            // var activity_id = (ulong)activity;
            var code = RequestActivity(ActivityIds[0], channel);
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithComponents(
                    new DiscordComponent[] 
                        {
                            new DiscordLinkButtonComponent(
                                $"https://discord.gg/{code}",
                                $" Watch Youtube Together",
                                false,
                                new DiscordComponentEmoji(EmojiIds[0])
                            )
                        }
                ).WithContent(channel.Mention)
            );  
        }
    }
}