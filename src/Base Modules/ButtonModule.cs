// using System;
// using System.Threading.Tasks;
// using DSharpPlus.CommandsNext;
// using DSharpPlus.CommandsNext.Attributes;
// using DSharpPlus.Entities;
// using DSharpPlus.Interactivity.Extensions;
// using DSharpPlus.EventArgs;
// using DSharpPlus;
// using Hexa.Attributes;
// using Hexa.Helpers;

// namespace Hexa.Modules
// {
//     public class ButtonModule : BaseCommandModule
//     {
//         [Command("buttons")]
//         [Category(SettingsManager.HexaSetting.RandomCategory)]
//         [DevOnly, Hidden]
//         public async Task ButtonCommand(CommandContext ctx)
//         {
//             // await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(hEmbed.embed.Build()));
//             var button = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, "hello", "test123", false);
//             // var a = new DiscordSelectComponent();
//             // a.Options = new DiscordSelectComponentOption[] { new DiscordSelectComponentOption("one", "1", "test")};
//             // var builder = new DiscordMessageBuilder().AddComponents(new DiscordComponent[] {button, a});
//             var interactivity = ctx.Client.GetInteractivity();
//             builder.WithContent("buttons test");
//             DiscordButtonComponent[] buttons = {button};
//             var message = await builder.SendAsync(ctx.Channel);
//             var buttonResponse = await interactivity.WaitForButtonAsync(message, buttons, TimeSpan.FromSeconds(50));
//             // await ctx.RespondAsync("Hi");
//             await ctx.RespondAsync(buttonResponse.Result.Message.Content);
//             // await ctx.RespondAsync();
//         }
//     }
// }