using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Hexa.Attributes;
using Hexa.Helpers;
using Newtonsoft.Json;

namespace Hexa.Modules
{
    [HexaCooldown(5)]
    public class WikiModule : BaseCommandModule
    {
        public WikiResponse GetResponse(string query)
        {
            var url = $"https://en.wikipedia.org/w/api.php?action=query&format=json&list=search&prop=extracts&srsearch={Uri.EscapeDataString(query)}&explaintext=true";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);


            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var response = JsonConvert.DeserializeObject<WikiResponse>(result);
                return response;
            }
        }

        [Command("wiki")]
        [Aliases("wikipedia", "search")]
        [Category("Fun")]
        [Description("Search anything on Wikipedia")]
        public async Task WikiCommand(CommandContext ctx, [RemainingText] string query = null)
        {
            if (query is null)
                throw new Exception("What should I search for?");

            var response = GetResponse(query);
            if (response.Query.Search.Count() == 0)
                throw new Exception($"I couldn't find any results for \"{query}\"");
                
            var interactivity = ctx.Client.GetInteractivity();
            var pages = new List<Page>();
            int page_index = 1;
            foreach (var search in response.Query.Search)
            {
                var hEmbed = new HexaEmbed(ctx, "wikipedia").WithFooter($"Page {page_index} of {response.Query.Search.Count()}");
                hEmbed.embed.Title = search.Title;
                hEmbed.embed.Description = Regex.Replace(HttpUtility.HtmlDecode(search.Snippet), "<[^>]*>", "**");
                pages.Add(new Page("", hEmbed.embed));
                page_index++;
            }
            await interactivity.SendButtonPaginatedMessageAsync(ctx.Channel, ctx.Message.Author, pages, "wikipedia", TimeSpan.FromSeconds(1));
        }
    }
}