using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Hexa.Attributes;
using Hexa.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hexa.Modules
{
    [HexaCooldown(5)]
    public class WikiModule : BaseCommandModule
    {
        public ProfanityFilter.ProfanityFilter filter { get; set; }

        public List<WikiPage> GetResponse(string query)
        {
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("action", "query");
            queryString.Add("format", "json");
            queryString.Add("formatversion", "2");
            queryString.Add("generator", "search");
            queryString.Add("redirects", "1");
            queryString.Add("prop", "extracts|info|pageimages|revisions|categories");
            queryString.Add("gsrsearch", string.Join(' ', Regex.Split(query, @"\s+").Select(x => $"intitle:{x}")));
            queryString.Add("gsrlimit", "15");
            queryString.Add("exintro", "1");
            queryString.Add("explaintext", "1");
            queryString.Add("inprop", "url");
            queryString.Add("piprop", "original");
            queryString.Add("rvprop", "timestamp");
            queryString.Add("clcategories", "Category:All disambiguation page");
            var url = "https://en.wikipedia.org/w/api.php?" + queryString.ToString();
            // var url = $"https://en.wikipedia.org/w/api.php?action=query&format=json&list=search&prop=extracts&srsearch={Uri.EscapeDataString(query)}&explaintext=true";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);


            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                // var response = JsonConvert.DeserializeObject<WikiResponse>(result);
                var response = JObject.Parse(result);
                if (response?["query"] is null)
                    return null;
                    // throw new Exception($"I couldn't find any results for \"{query}\"");
                var list = (response["query"]["pages"]).
                    Select(x => new WikiPage()
                    {
                        Title = x["title"].Value<string>(),
                        Content = x["extract"].Value<string>(),
                        Url = x["fullurl"].Value<string>(),
                        // ImageUrl = x["original"] is not null ? x["original"]["source"].Value<string>() : null
                        ImageUrl = x["original"]?["source"]?.Value<string>()
                    }).
                    ToList();
                // var a = 1;
                return list;
            }
        }

        [Command("wiki")]
        [Aliases("wikipedia", "search")]
        [Category(SettingsManager.HexaSetting.FunCategory)]
        [Description("Search anything on Wikipedia")]
        public async Task WikiCommand(CommandContext ctx, [RemainingText] string query = null)
        {
            if (query is null)
                throw new ArgumentException("What should I search for?");
            if (filter.DetectAllProfanities(query).Any() && !ctx.Channel.IsNSFW)
                throw new InvalidOperationException("I can't search for that, sorry…");
            var hEmbed = new HexaEmbed(ctx, "wikipedia");
            hEmbed.embed.Description = "waiting… <a:pinging:781983658646175764>";
            var message = await ctx.Channel.SendMessageAsync(hEmbed.Build());
            // await ctx.Channel.TriggerTypingAsync();
            var response = GetResponse(query);
            if (response is null)
            {
                hEmbed.embed.WithDescription($"I couldn't find any results for **\"{query}\"**");
                await message.ModifyAsync(hEmbed.Build());
                return;
            }
            // if (!response.Any())
            var interactivity = ctx.Client.GetInteractivity();

            var pages = new List<Page>();
            int page_index = 1;
            foreach (var search in response)
            {
                hEmbed = new HexaEmbed(ctx, "wikipedia").WithFooter($"Page {page_index} of {response.Count()}");
                hEmbed.embed.WithTitle(search.Title);
                hEmbed.embed.WithUrl(search.Url);
                // hEmbed.embed.Description = Regex.Replace(HttpUtility.HtmlDecode(search.Value), "<[^>]*>", "**");
                hEmbed.embed.Description = filter.CensorString(search.Content).TruncateAtWord(1500, "…");
                if (search.ImageUrl is not null && !filter.DetectAllProfanities(search.Content).Any())
                    hEmbed.embed.WithImageUrl(search.ImageUrl);
                pages.Add(new Page("", hEmbed.embed));
                page_index++;
            }
            await interactivity.SendButtonPaginatedMessageAsync(ctx.Channel, ctx.Message.Author, pages, "wikipedia", TimeSpan.FromSeconds(60), msg: message, showPrint: false, showClose: true);
        }
    }
}