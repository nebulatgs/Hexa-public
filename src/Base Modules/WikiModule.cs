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
using Hexa.Helpers;
using Newtonsoft.Json;

namespace Hexa.Modules
{
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
        public async Task WikiCommand(CommandContext ctx, [RemainingText] string query = null)
        {
            if (query is null)
                throw new Exception("What should I search for?");

            var response = GetResponse(query);
            if (response.Query.Search.Count() == 0)
                throw new Exception($"I couldn't find any results for \"{query}\"");
                
            var interactivity = ctx.Client.GetInteractivity();
            var pages = new List<Page>();
            // var pagination_id = "wikipedia";

            // var next = new DiscordButtonComponent(ButtonStyle.Primary, $"{pagination_id}_next", "next page", false);
            // var previous = new DiscordButtonComponent(ButtonStyle.Primary, $"{pagination_id}_previous", "previous page", true);
            // var close = new DiscordButtonComponent(ButtonStyle.Danger, $"{pagination_id}_close", "close", false);
            // DiscordButtonComponent[] buttons = { previous, next, close };
            // var builder = new DiscordMessageBuilder();
            int page_index = 1;
            foreach (var search in response.Query.Search)
            {
                var hEmbed = new HexaEmbed(ctx, "wikipedia").WithFooter($"Page {page_index} of {response.Query.Search.Count()}");
                hEmbed.embed.Title = search.Title;
                hEmbed.embed.Description = Regex.Replace(HttpUtility.HtmlDecode(search.Snippet), "<[^>]*>", "**");
                pages.Add(new Page("", hEmbed.embed));
                page_index++;
            }
            await interactivity.SendButtonPaginatedMessageAsync(ctx.Channel, ctx.Message.Author, pages, "wikipedia", TimeSpan.FromSeconds(60));
            // int current_page = 0;
            // var buttonBuilder = builder.WithComponents(buttons);
            // builder = buttonBuilder.WithEmbed(pages[current_page]);
            // var message = await ctx.Channel.SendMessageAsync(builder);
            // var timeout = DateTime.Now.AddSeconds(60);
            // while (DateTime.Now < timeout)
            // {
            //     var result = await interactivity.WaitForButtonAsync(message, buttons, TimeSpan.FromSeconds(60));
            //     if (result.TimedOut)
            //     {
            //         builder.Clear();
            //         builder = builder.WithEmbed(pages[current_page]);
            //         await message.ModifyAsync(builder);
            //         return;
            //     }
            //     switch (result.Result.Id)
            //     {
            //         case $"{pagination_id}_next":
            //             current_page++;
            //             previous.Disabled = false;
            //             if (current_page == pages.Count - 1)
            //                 next.Disabled = true;
            //             builder = buttonBuilder.WithEmbed(pages[current_page]);
            //             await message.ModifyAsync(builder);
            //             break;
            //         case $"{pagination_id}_previous":
            //             current_page--;
            //             next.Disabled = false;
            //             if (current_page == 0)
            //                 previous.Disabled = true;
            //             builder = buttonBuilder.WithEmbed(pages[current_page]);
            //             await message.ModifyAsync(builder);
            //             break;
            //         case $"{pagination_id}_close":
            //             await message.DeleteAsync();
            //             return;
            //     }
            // }
            // builder.Clear();
            // builder = builder.WithEmbed(pages[current_page]);
            // await message.ModifyAsync(builder);
        }
    }
}