using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Hexa.Attributes;
using Hexa.Helpers;
using System;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace Hexa.Modules
{
    [HexaCooldown(60, 1)]
    public class TextCompletionModule : BaseCommandModule
    {
        private class Prompt
        {
            public string text { get; set; }
        }
        private class InferKitRequest{
            public Prompt prompt { get; set; }  
            public int length { get; set; }
        }

        private string RequestAutoCompleteInferKit(string api_key, string text)
        {
            var request = new InferKitRequest{
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
            if(!ctx.Channel.IsNSFW)
                throw new UnauthorizedAccessException("This command is restricted to NSFW channels");
            if (text is null)
                throw new ArgumentNullException("What text should I complete?");
            var close = new DiscordButtonComponent(ButtonStyle.Danger, "ai_close", "close", false);
            var builder = new DiscordMessageBuilder();
            var interactivity = ctx.Client.GetInteractivity();
            DiscordButtonComponent[] buttons = { close };
            builder.WithComponents(buttons);
            await ctx.Channel.TriggerTypingAsync();
            var output = RequestAutoCompleteDeepAI("468f237f-d4c0-426b-b06f-7362d03daadb", text);
            // var output = RequestAutoCompleteInferKit("b0444ca5-bd34-4f24-bc56-8beaaa811b69", text);

            var message = await ctx.Channel.SendMessageAsync(builder.WithContent($"```\n{output.ToString()}\n```"));

            var buttonResponse = await interactivity.WaitForButtonAsync(message, buttons, TimeSpan.FromSeconds(60));
            if(!buttonResponse.TimedOut)
                await message.DeleteAsync();
            else
            {
                close.Disabled = true;
                await message.ModifyAsync(builder);
            }
        }
    }
}