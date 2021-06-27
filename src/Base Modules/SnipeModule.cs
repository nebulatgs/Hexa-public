using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Hexa.Attributes;
using Hexa.Helpers;

namespace Hexa.Modules
{
    public class SnipeModule : BaseCommandModule
    {
        public SnipeHelper snipeHelper { get; set; }
        [Command("snipe")]
        [Category(SettingsManager.HexaSetting.DumbCategory)]
        public async Task SnipeCommand(CommandContext ctx)
        {
            var message = snipeHelper.GetSnipe(ctx.Channel);
            if (message is null) throw new InvalidOperationException("There's nothing to snipe!");
            var hEmbed = new HexaEmbed(ctx, "snipe");
            hEmbed.embed.Description = message.Content ?? message.Embeds.FirstOrDefault()?.Description ?? "no content";
            // hEmbed.embed.Author = message.Author;
            hEmbed.embed.Footer.IconUrl = message.Author.AvatarUrl;
            hEmbed.embed.Footer.Text = $"{message.Author.Username}#{message.Author.Discriminator}";
            hEmbed.embed.Timestamp = message.Timestamp;
            await ctx.RespondAsync(hEmbed.Build());
        }
    }
}