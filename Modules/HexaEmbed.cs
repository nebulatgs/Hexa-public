using System;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;

namespace Hexa.Modules
{
    public class HexaEmbed
    {   
        public DiscordEmbedBuilder embed;
        public DiscordEmbed Build()
        {
            return embed.Build();
        }
        public HexaEmbed(CommandContext Context, string Title) : base()
        {
            embed = new DiscordEmbedBuilder
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = Context.Client.CurrentUser.AvatarUrl,
                    Name = Title
                },
                Color = new DiscordColor(0xd63031),
                Timestamp = DateTime.Now,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = Context.Message.Author.AvatarUrl,
                    Text = $"{Context.Message.Author.Username}#{Context.Message.Author.Discriminator}"
                }
            };
            embed.Build();
        }
    }
}