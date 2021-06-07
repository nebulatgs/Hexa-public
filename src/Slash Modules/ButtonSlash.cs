using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

using Hexa.Helpers;

namespace Hexa.Modules
{
    // public class ButtonSlash : SlashCommandModule
    // {
    //     [SlashCommand("buttons", "I'm testing buttons idk")]
    //     public async Task ButtonCommand(InteractionContext ctx)
    //     {
    //         // await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(hEmbed.embed.Build()));
    //         var button = new DiscordButtonComponent(ButtonStyle.Primary, "hello", "test123", false);
    //         var builder = new DiscordMessageBuilder().AddComponents(button);
    //         builder.WithContent("buttons test");
    //         DiscordComponent[] buttons = {button};
    //         var interactivity = ctx.Client.GetInteractivity();
    //         await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddComponents(buttons).WithContent("button test"));
    //     }
    // }
}