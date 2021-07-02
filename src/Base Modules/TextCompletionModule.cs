using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity.Extensions;
using Hexa.Attributes;
using Hexa.Helpers;
using Microsoft.Extensions.Configuration;

namespace Hexa.Modules
{
    [HexaCooldown(60, 1)]
    public class TextCompletionModule : BaseCommandModule
    {
        public ProfanityFilter.ProfanityFilter filter { get; set; }
        private class Prompt
        {
            public string text { get; set; }
        }
        private class InferKitRequest
        {
            public Prompt prompt { get; set; }
            public int length { get; set; }
        }

        private string RequestAutoCompleteInferKit(string api_key, string text)
        {
            var request = new InferKitRequest
            {
                prompt = new Prompt { text = text },
                length = 200
            };
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.inferkit.com/v1/models/standard/generate");
            httpWebRequest.ContentType = "application/json";
            var headers = new WebHeaderCollection();

            headers.Add("Authorization", $"Bearer {api_key}");
            headers.Add("Content-Type", "application/json");
            httpWebRequest.Headers = headers;
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonSerializer.Serialize<InferKitRequest>(request);

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var builder = new ConfigurationBuilder().AddJsonStream(httpResponse.GetResponseStream());
            var config = builder.Build();
            return config["data:text"];
        }
        private string RequestAutoCompleteDeepAI(string api_key, string text)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"https://api.deepai.org/api/text-generator");
            httpWebRequest.ContentType = "application/json";
            var headers = new WebHeaderCollection();

            string data = $"text={text}";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.ContentLength = Encoding.ASCII.GetBytes(data).Length;
            httpWebRequest.Headers["Api-Key"] = api_key;
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string form = data;

                streamWriter.Write(form);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var builder = new ConfigurationBuilder().AddJsonStream(httpResponse.GetResponseStream());
            var config = builder.Build();
            return config["output"];
        }

        public TextCompletionModule()
        {
        }
        [Command("autocomplete")]
        [Aliases("ai", "complete")]
        [Description("Autocomplete text using GPT-2")]
        [Category(SettingsManager.HexaSetting.FunCategory)]
        public async Task TextCompletionCommand(CommandContext ctx, [RemainingText, Description("The text to complete")] string text = null)
        {
            // if(!ctx.Channel.IsNSFW)
            // throw new UnauthorizedAccessException("This command is restricted to NSFW channels");
            if (text is null)
                throw new ArgumentException("What text should I complete?");
            if (filter.DetectAllProfanities(text).Any() && !ctx.Channel.IsNSFW)
                throw new InvalidOperationException("I can't autocomplete that, sorry…");
            var close = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Danger, "ai_close", "close", false);
            var builder = new DiscordMessageBuilder();
            var interactivity = ctx.Client.GetInteractivity();
            DiscordButtonComponent[] buttons = { close };
            builder.AddComponents(buttons);
            var hEmbed = new HexaEmbed(ctx, "autocomplete");
            hEmbed.embed.Description = "waiting… <a:pinging:781983658646175764>";
            var message = await ctx.RespondAsync(hEmbed.Build());

            // await ctx.Channel.TriggerTypingAsync();
            var output = RequestAutoCompleteDeepAI("468f237f-d4c0-426b-b06f-7362d03daadb", text).TruncateAtWord(900) + '…';
            // var output = RequestAutoCompleteInferKit("b0444ca5-bd34-4f24-bc56-8beaaa811b69", text);
            hEmbed.embed.Description = $"```\n{((!ctx.Channel.IsNSFW) ? filter.CensorString(output) : output)}\n```";
            builder.WithEmbed(hEmbed.Build());
            try { message = await message.ModifyAsync(builder); } catch (NotFoundException) { return; }
            var timeout = TimeSpan.FromSeconds(60);
            var then = DateTime.Now;
            while (then + timeout > DateTime.Now)
            {
                var buttonResponse = await interactivity.WaitForButtonAsync(message, buttons, timeout);
                if (!buttonResponse.TimedOut)
                {
                    if (buttonResponse.Result.User == ctx.Message.Author)
                        await message.DeleteAsync();
                }
                else
                    break;
            }
            close.Disabled = true;
            try { await message.ModifyAsync(builder); } catch (NotFoundException) { }
        }
    }
}