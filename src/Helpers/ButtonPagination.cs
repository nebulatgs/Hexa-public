using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

namespace Hexa.Helpers
{
    // public static class InteractivityExtensions
    // {
    //     public static async Task SendButtonPaginatedMessageAsync(this InteractivityExtension interactivity, DiscordChannel c, DiscordUser u, IEnumerable<Page> pages, string pagination_id, TimeSpan? timeout = null)
    //     {
    //         // Create the buttons
    //         var next = new DiscordButtonComponent(ButtonStyle.Primary, $"{pagination_id}_next", "next page", false);
    //         var previous = new DiscordButtonComponent(ButtonStyle.Primary, $"{pagination_id}_previous", "previous page", true);
    //         var close = new DiscordButtonComponent(ButtonStyle.Danger, $"{pagination_id}_close", "close", false);
    //         DiscordButtonComponent[] buttons = { previous, next, close };
    //         var page_list = pages.ToImmutableList();
    //         int current_page = 0;
    //         // Init the message builder
    //         var builder = new DiscordMessageBuilder();
    //         var buttonBuilder = builder.WithComponents(buttons);
    //         builder = buttonBuilder.WithContent(page_list[current_page].Content).WithEmbed(page_list[current_page].Embed);

    //         var message = await c.SendMessageAsync(builder);

    //         // Loop until timeout and handle the buttons
    //         var loop_timeout = DateTime.Now + timeout;
    //         while (DateTime.Now < loop_timeout)
    //         {
    //             var result = await interactivity.WaitForButtonAsync(message, buttons, timeout);
    //             if (result.TimedOut)
    //             {
    //                 break;
    //             }
    //             if (result.Result.User != u)
    //                 continue;
    //             else if (result.Result.Id == $"{pagination_id}_next")
    //             {
    //                 current_page++;
    //                 previous.Disabled = false;
    //                 if (current_page == page_list.Count - 1)
    //                     next.Disabled = true;
    //                 builder = buttonBuilder.WithContent(page_list[current_page].Content).WithEmbed(page_list[current_page].Embed);
    //                 await message.ModifyAsync(builder);
    //             }
    //             else if (result.Result.Id == $"{pagination_id}_previous")
    //             {
    //                 current_page--;
    //                 next.Disabled = false;
    //                 if (current_page == 0)
    //                     previous.Disabled = true;
    //                 builder = buttonBuilder.WithContent(page_list[current_page].Content).WithEmbed(page_list[current_page].Embed);
    //                 await message.ModifyAsync(builder);
    //             }
    //             else if (result.Result.Id == $"{pagination_id}_close")
    //             {
    //                 await message.DeleteAsync();
    //                 return;
    //             }
    //         }
    //         next.Disabled = true;
    //         previous.Disabled = true;
    //         close.Disabled = true;
    //         await message.ModifyAsync(builder);
    //     }
    public static class InteractivityExtensions
    {
        public static async Task SendButtonPaginatedMessageAsync(this InteractivityExtension interactivity, DiscordChannel channel, DiscordUser user,
            IEnumerable<Page> pages, string pagination_id, TimeSpan? timeout = null, bool userCheck = true,
            bool showFirst = true, bool showPrevious = true, bool showNext = true, bool showLast = true, bool showPrint = true, bool showClose = false)
        {
            var page_list = pages.ToImmutableList();
            var current_page = 0;

            // Create the buttons
            var first = new DiscordButtonComponent(ButtonStyle.Secondary, $"{pagination_id}_first", "First", true);
            var previous = new DiscordButtonComponent(ButtonStyle.Secondary, $"{pagination_id}_previous", "Previous", true);
            var next = new DiscordButtonComponent(ButtonStyle.Secondary, $"{pagination_id}_next", "Next", current_page == (page_list.Count - 1));
            var last = new DiscordButtonComponent(ButtonStyle.Secondary, $"{pagination_id}_last", "Last", current_page == (page_list.Count - 1));
            var print = new DiscordButtonComponent(ButtonStyle.Success, $"{pagination_id}_print", "Print", false);
            var close = new DiscordButtonComponent(ButtonStyle.Danger, $"{pagination_id}_close", "Close", false);

            var buttons = new List<DiscordButtonComponent>();
            if (showFirst) buttons.Add(first);
            if (showPrevious) buttons.Add(previous);
            if (showNext) buttons.Add(next);
            if (showLast) buttons.Add(last);
            if (showPrint) buttons.Add(print);
            if (showClose) buttons.Add(close);

            if (buttons.Count > 5)
                throw new Exception("Cannot have more than 5 buttons.");

            // Init the message builder
            var builder = new DiscordMessageBuilder();
            var buttonBuilder = builder.WithComponents(buttons);
            builder = buttonBuilder.WithContent(page_list[current_page].Content).WithEmbed(page_list[current_page].Embed);
            var message = await channel.SendMessageAsync(builder);

            // Loop until timeout and handle the buttons
            var loop_timeout = DateTime.Now + timeout;
            while (DateTime.Now < loop_timeout)
            {
                var result = await interactivity.WaitForButtonAsync(message, buttons, timeout);
                if (result.TimedOut) break;
                if (userCheck && result.Result.User != user) continue;

                if (result.Result.Id == $"{pagination_id}_close")
                {
                    await message.DeleteAsync();
                    return;
                }

                if (result.Result.Id == $"{pagination_id}_print")
                {
                    await message.DeleteAsync();
                    page_list.ForEach(async page => await channel.SendMessageAsync(new DiscordMessageBuilder().WithContent(page.Content).WithEmbed(page.Embed)));
                    return;
                }

                if (result.Result.Id == $"{pagination_id}_first")
                    current_page = 0;
                else if (result.Result.Id == $"{pagination_id}_previous")
                    current_page--;
                else if (result.Result.Id == $"{pagination_id}_next")
                    current_page++;
                else if (result.Result.Id == $"{pagination_id}_last")
                    current_page = page_list.Count - 1;

                first.Disabled = current_page == 0;
                previous.Disabled = current_page == 0;
                next.Disabled = current_page == (page_list.Count - 1);
                last.Disabled = current_page == (page_list.Count - 1);

                builder = buttonBuilder.WithContent(page_list[current_page].Content).WithEmbed(page_list[current_page].Embed);
                await message.ModifyAsync(builder);


            }
            next.Disabled = true;
            previous.Disabled = true;
            close.Disabled = true;
            first.Disabled = true;
            last.Disabled = true;
            print.Disabled = true;
            await message.ModifyAsync(builder);
        }
    }
}