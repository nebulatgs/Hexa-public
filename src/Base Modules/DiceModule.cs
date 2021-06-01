using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Hexa.Attributes;
using Hexa.Helpers;

namespace Hexa.Modules
{
    public class DiceModule : BaseCommandModule
    {
        public Random rand { get; set; }

        [Command("dice")]
        [Aliases("roll")]
        [Category("Random")]
        [Description("Roll a die!")]
        public async Task DiceCommand(CommandContext ctx, string sides = "6")
        {
            if(!int.TryParse(sides, out int int_sides)) throw new ArgumentException("That's…physically impossible?");
            if (int_sides <= 0) throw new ArgumentException("That's…physically impossible?");
            var button = new DiscordButtonComponent(ButtonStyle.Success, "roll", "roll again", false);
            var builder = new DiscordMessageBuilder();
            var interactivity = ctx.Client.GetInteractivity();
            DiscordButtonComponent[] buttons = { button };

            var hEmbed = new HexaEmbed(ctx, "dice roll");
            hEmbed.embed.WithTitle($"Rolling a {int_sides}-sided die");
            hEmbed.embed.WithDescription($"Rolling…");
            builder.WithEmbed(hEmbed.Build());
            var message = await builder.SendAsync(ctx.Channel);
            await Task.Delay(500);
            var roll = rand.Next(1, int_sides + 1);
            hEmbed.embed.Description = $"Rolled a {roll}";
            builder.WithEmbed(hEmbed.Build()).WithComponents(button);

            message = await message.ModifyAsync(builder);
            var timeout = DateTime.Now.AddSeconds(30);
            while (DateTime.Now < timeout)
            {
                var buttonResponse = await interactivity.WaitForButtonAsync(message, buttons, TimeSpan.FromSeconds(30));
                hEmbed.embed.WithDescription($"Rolling…");
                builder.WithEmbed(hEmbed.Build());
                await message.ModifyAsync(builder);
                await Task.Delay(500);
                roll = rand.Next(1, int_sides + 1);
                hEmbed.embed.Description = $"Rolled a {roll}";
                builder.WithEmbed(hEmbed.Build());
                await message.ModifyAsync(builder);
            }
            button.Disabled = true;
            await message.ModifyAsync(builder);
        }

        [Command("coinflip")]
        [Aliases("coin", "flip")]
        [Category("Random")]
        [Description("Flip a coin!")]
        public async Task FlipCommand(CommandContext ctx)
        {
            var button = new DiscordButtonComponent(ButtonStyle.Success, "flip", "flip again", false);
            var builder = new DiscordMessageBuilder();
            var interactivity = ctx.Client.GetInteractivity();
            DiscordButtonComponent[] buttons = { button };

            var hEmbed = new HexaEmbed(ctx, "coin flip");
            hEmbed.embed.WithTitle($"Flipping a coin…");
            builder.WithEmbed(hEmbed.Build());
            var message = await builder.SendAsync(ctx.Channel);
            await Task.Delay(500);
            var flip = rand.Next(0, 2) == 0 ? "heads" : "tails";
            hEmbed.embed.Title = $"It's {flip}!";
            builder.WithEmbed(hEmbed.Build()).WithComponents(button);

            message = await message.ModifyAsync(builder);
            var timeout = DateTime.Now.AddSeconds(30);
            while (DateTime.Now < timeout)
            {
                var buttonResponse = await interactivity.WaitForButtonAsync(message, buttons, TimeSpan.FromSeconds(30));
                hEmbed.embed.WithTitle($"Flipping a coin…");
                builder.WithEmbed(hEmbed.Build());
                await message.ModifyAsync(builder);
                await Task.Delay(500);
                flip = rand.Next(0, 2) == 0 ? "heads" : "tails";
                hEmbed.embed.Title = $"It's {flip}!";
                builder.WithEmbed(hEmbed.Build());
                await message.ModifyAsync(builder);
            }
            button.Disabled = true;
            await message.ModifyAsync(builder);
        }
    }
}