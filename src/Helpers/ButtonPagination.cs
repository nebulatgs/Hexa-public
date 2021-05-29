using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace Hexa.Helpers
{
    public static class InteractivityExtensions
    {
        public static async Task SendButtonPaginatedMessageAsync(this InteractivityExtension interactivity, DiscordChannel c, DiscordUser u, IEnumerable<Page> pages, string pagination_id, TimeSpan? timeout = null)
        {
            // Create the buttons
            var next = new DiscordButtonComponent(ButtonStyle.Primary, $"{pagination_id}_next", "next page", false);
            var previous = new DiscordButtonComponent(ButtonStyle.Primary, $"{pagination_id}_previous", "previous page", true);
            var close = new DiscordButtonComponent(ButtonStyle.Danger, $"{pagination_id}_close", "close", false);
            DiscordButtonComponent[] buttons = { previous, next, close };
            var page_list = pages.ToImmutableList();
            int current_page = 0;
            // Init the message builder
            var builder = new DiscordMessageBuilder();
            var buttonBuilder = builder.WithComponents(buttons);
            builder = buttonBuilder.WithContent(page_list[current_page].Content).WithEmbed(page_list[current_page].Embed);

            var message = await c.SendMessageAsync(builder);

            // Loop until timeout and handle the buttons
            var loop_timeout = DateTime.Now + timeout;
            while (DateTime.Now < loop_timeout)
            {
                var result = await interactivity.WaitForButtonAsync(message, buttons, timeout);
                if (result.TimedOut)
                {
                    builder.Clear();
                    builder = builder.WithContent(page_list[current_page].Content).WithEmbed(page_list[current_page].Embed);
                    await message.ModifyAsync(builder);
                    return;
                }
                if (result.Result.User != u)
                    continue;
                else if (result.Result.Id == $"{pagination_id}_next")
                {
                    current_page++;
                    previous.Disabled = false;
                    if (current_page == page_list.Count - 1)
                        next.Disabled = true;
                    builder = buttonBuilder.WithContent(page_list[current_page].Content).WithEmbed(page_list[current_page].Embed);
                    await message.ModifyAsync(builder);
                }
                else if (result.Result.Id == $"{pagination_id}_previous")
                {
                    current_page--;
                    next.Disabled = false;
                    if (current_page == 0)
                        previous.Disabled = true;
                    builder = buttonBuilder.WithContent(page_list[current_page].Content).WithEmbed(page_list[current_page].Embed);
                    await message.ModifyAsync(builder);
                }
                else if (result.Result.Id == $"{pagination_id}_close")
                {
                    await message.DeleteAsync();
                    return;
                }
            }

            // Delete buttons after timing out
            builder.Clear();
            builder = builder.WithContent(page_list[current_page].Content).WithEmbed(page_list[current_page].Embed);
            await message.ModifyAsync(builder);
        }
    }
}