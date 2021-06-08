using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Hexa.Attributes;
using Hexa.Helpers;

namespace Hexa.Modules
{
    [HexaCooldown(5)]
    public class MathModule : BaseCommandModule
    {
        private string GetResponse(string query)
        {
            var url = "https://miscreant-force-production.up.railway.app/";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/json";

            var data = $"{{\"query\": \"{Uri.EscapeDataString(query)}\"}}";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                if (result == "")
                    throw new Exception("Blank response");
                return result;
            }
        }
        [Command("math")]
        [Aliases("expr", "calc", "calculate")]
        [Description("Evaluate a mathamatical expression or equation")]
        [Category(SettingsManager.HexaSetting.FunCategory)]
        public async Task MathCommand(CommandContext ctx, [RemainingText, Description("The expression or equation to evaluate")] string query)
        {
            if (query is null)
                throw new ArgumentException("Please provide an expression or equation to evaluate.");
            // await ctx.TriggerTypingAsync();
            var hEmbed = new HexaEmbed(ctx, "hexa math");
            hEmbed.embed.WithDescription("waiting… <a:pinging:781983658646175764>");
            var message = await ctx.RespondAsync(hEmbed.Build());
            try
            {
                hEmbed.embed.WithImageUrl(GetResponse(query)).WithDescription($"Evaluation for **{query}**");
                try { await message.ModifyAsync(hEmbed.Build()); } catch (NotFoundException) { }
            }
            catch (BadRequestException)
            {
                hEmbed.embed.ImageUrl = null;
                if (query.Contains('='))
                    hEmbed.embed.WithDescription("**Unable to evaluate expression**");
                else
                    hEmbed.embed.WithDescription("**Unable to evaluate equation**");
                try { await message.ModifyAsync(hEmbed.Build()); } catch (NotFoundException) { }
            }
            catch
            {
                if (query.Contains('='))
                    hEmbed.embed.WithDescription("**Invalid equation**");
                else
                    hEmbed.embed.WithDescription("**Invalid expression**");
                try { await message.ModifyAsync(hEmbed.Build()); } catch (NotFoundException) { }
            }
            // try
            // {
            //     await ctx.RespondAsync(embed: hEmbed.Build());
            // }
            // catch
            // {
            //     throw new Exception("Response too large");
            // }
        }
    }
}