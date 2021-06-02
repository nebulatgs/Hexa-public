using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.EventArgs;
using DSharpPlus;
using Hexa.Attributes;

namespace Hexa.Modules
{
    public class ButtonModule : BaseCommandModule
    {
        [Command("buttons")]
        [Category("Random")]
        [DevOnly, Hidden]
        public async Task ButtonCommand(CommandContext ctx)
        {
            // await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(hEmbed.embed.Build()));
            var button = new DiscordButtonComponent(ButtonStyle.Primary, "hello", "test123", false);
            var a = new DiscordSelectComponent();
            a.Options = new DiscordSelectComponentOption[] { new DiscordSelectComponentOption("one", "1", "test")};
            var builder = new DiscordMessageBuilder().WithComponents(new DiscordComponent[] {button, a});
            var interactivity = ctx.Client.GetInteractivity();
            builder.WithContent("buttons test");
            DiscordButtonComponent[] buttons = {button};
            var message = await builder.SendAsync(ctx.Channel);
            var buttonResponse = await interactivity.WaitForButtonAsync(message, buttons, TimeSpan.FromSeconds(50));
            // await ctx.RespondAsync("Hi");
            await ctx.RespondAsync(buttonResponse.Result.Message.Content);
            // await ctx.RespondAsync();
        }
    }
}