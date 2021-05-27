using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

using Hexa.Helpers;

namespace Hexa.Modules
{
    public class AvatarSlash : SlashCommandModule
    {
        [SlashCommand("avatar", "Get someone's avatar")]
        public async Task AvatarCommand(InteractionContext ctx, [Option("user", "The user to get it for")] DiscordUser user = null)
        {
            user ??= ctx.Member;
            var hEmbed = new HexaEmbed(ctx, $"{user.Username}#{user.Discriminator}'s avatar");
            hEmbed.embed.WithImageUrl(user.AvatarUrl);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AddEmbed(hEmbed.embed.Build()));
        }
    }
}