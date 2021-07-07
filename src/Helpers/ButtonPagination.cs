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
        public static async Task SendButtonPaginatedMessageAsync(this InteractivityExtension interactivity, DiscordChannel channel, DiscordUser user,
            IEnumerable<Page> pages, string pagination_id, TimeSpan? timeout = null, DiscordMessage msg = null,
            bool userCheck = true, bool showFirst = true, bool showPrevious = true, bool showNext = true, bool showLast = true, bool showPrint = true, bool showClose = false)
        {
            var page_list = pages.ToImmutableList();
            var current_page = 0;

            // Create the buttons
            var first = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Secondary, $"{pagination_id}_first", "First", true);
            var previous = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Secondary, $"{pagination_id}_previous", "Previous", true);
            var next = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Secondary, $"{pagination_id}_next", "Next", current_page == (page_list.Count - 1));
            var last = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Secondary, $"{pagination_id}_last", "Last", current_page == (page_list.Count - 1));
            var print = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Success, $"{pagination_id}_print", "Print", false);
            var close = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Danger, $"{pagination_id}_close", "Close", false);

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
            var buttonBuilder = builder.AddComponents(buttons);
            builder = buttonBuilder.WithContent(page_list[current_page].Content).WithEmbed(page_list[current_page].Embed);
            var message = msg is null ? await channel.SendMessageAsync(builder) : await msg.ModifyAsync(builder);
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
            try {await message.ModifyAsync(builder);} catch{}
        }
    }
}
