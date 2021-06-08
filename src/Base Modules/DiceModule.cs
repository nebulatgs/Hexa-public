using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity.Extensions;
using Hexa.Attributes;
using Hexa.Helpers;

namespace Hexa.Modules
{
    [HexaCooldown(5)]
    public class DiceModule : BaseCommandModule
    {
        public Random rand { get; set; }

        [Command("dice")]
        [Aliases("roll")]
        [Category(SettingsManager.HexaSetting.RandomCategory)]
        [Description("Roll a die!")]
        public async Task DiceCommand(CommandContext ctx, string sides = "6")
        {
            // BigInteger.TryParse(sides, out var bigint_sides);
            if (!BigInteger.TryParse(sides, out var bigint_sides)) throw new ArgumentException("That's…physically impossible?");
            if (bigint_sides <= 0) throw new ArgumentException("That's…physically impossible?");
            if (sides.Length > 150)
                throw new ArgumentException("The die was too heavy to roll…");
            var button = new DiscordButtonComponent(ButtonStyle.Success, "roll", "roll again", false);
            var builder = new DiscordMessageBuilder();
            var interactivity = ctx.Client.GetInteractivity();
            DiscordButtonComponent[] buttons = { button };

            var hEmbed = new HexaEmbed(ctx, "dice roll");
            hEmbed.embed.WithTitle($"Rolling a {bigint_sides}-sided die");
            hEmbed.embed.WithDescription($"Rolling…");
            builder.WithEmbed(hEmbed.Build());
            var message = await builder.SendAsync(ctx.Channel);
            await Task.Delay(500);
            var roll = rand.NextBigInteger(1, bigint_sides + 1);
            hEmbed.embed.Description = $"**Rolled a** {roll}";
            builder.WithEmbed(hEmbed.Build()).AddComponents(button);

            message = await message.ModifyAsync(builder);
            var timeout = DateTime.Now.AddSeconds(30);
            while (DateTime.Now < timeout)
            {
                var buttonResponse = await interactivity.WaitForButtonAsync(message, buttons, TimeSpan.FromSeconds(30));
                hEmbed.embed.WithDescription($"**Rolling…**");
                builder.WithEmbed(hEmbed.Build());
                await message.ModifyAsync(builder);
                await Task.Delay(500);
                roll = rand.NextBigInteger(1, bigint_sides + 1);
                hEmbed.embed.Description = $"**Rolled a** {roll}";
                builder.WithEmbed(hEmbed.Build());
                await message.ModifyAsync(builder);
            }
            button.Disabled = true;
            await message.ModifyAsync(builder);
        }

        [Command("coinflip")]
        [Aliases("coin", "flip")]
        [Category(SettingsManager.HexaSetting.RandomCategory)]
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
            builder.WithEmbed(hEmbed.Build()).AddComponents(button);

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

        private string[] Responses = new[]
        {
            "It is Certain.",
            "It is decidedly so.",
            "Without a doubt.",
            "Yes definitely.",
            "You may rely on it.",
            "As I see it, yes.",
            "Most likely.",
            "Outlook good.",
            "Yes.",
            "Signs point to yes.",
            "Reply hazy, try again.",
            "Ask again later.",
            "Better not tell you now.",
            "Cannot predict now.",
            "Concentrate and ask again.",
            "Don't count on it.",
            "My reply is no.",
            "My sources say no.",
            "Outlook not so good.",
            "Very doubtful."
        };

        [Command("8ball")]
        [Aliases("8-ball")]
        [Category(SettingsManager.HexaSetting.RandomCategory)]
        [Description("Call upon the magic 8-ball to answer your questions")]
        public async Task EightBallCommand(CommandContext ctx, [RemainingText] string query)
        {
            if (query is null)
                throw new ArgumentException("What shall you ask the great 8-ball?");
            var hEmbed = new HexaEmbed(ctx, "magic 8-ball");
            hEmbed.embed.WithDescription("thinking… <a:pinging:781983658646175764>");
            var message = await ctx.RespondAsync(hEmbed.Build());
            await Task.Delay(500);
            using var algo = SHA1.Create();
            var hash = BitConverter.ToInt32(algo.ComputeHash(Encoding.UTF8.GetBytes(query)));
            var rand = new Random(hash);
            string response = Responses[rand.Next(0, 20)];
            hEmbed.embed.WithDescription($"**{response}**");
            try { await message.ModifyAsync(hEmbed.Build()); } catch (NotFoundException) { }
            // await ctx.TriggerTypingAsync();
        }
    }
}