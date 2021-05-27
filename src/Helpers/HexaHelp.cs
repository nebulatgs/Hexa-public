using System;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;

namespace Hexa
{
    public class HexaHelp : DefaultHelpFormatter
    {
        public HexaHelp(CommandContext ctx) : base(ctx) { }

        public override CommandHelpMessage Build()
        {
            EmbedBuilder.Color = DiscordColor.SpringGreen;
            EmbedBuilder.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                IconUrl = Context.Client.CurrentUser.AvatarUrl,
                Name = "hexa help"
            };
            EmbedBuilder.Color = new DiscordColor(0xd63031);
            EmbedBuilder.Timestamp = DateTime.Now;
            EmbedBuilder.Footer = new DiscordEmbedBuilder.EmbedFooter
            {
                IconUrl = Context.Message.Author.AvatarUrl,
                Text = $"{Context.Message.Author.Username}#{Context.Message.Author.Discriminator}"
            };
            return base.Build();
        }
    }
}