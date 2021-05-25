using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Hexa.Attributes;

namespace Hexa.Modules
{
    public class InviteModule : BaseCommandModule
    {
        [Command("invite")]
        [Aliases("inv")]
        [Description("Invite Hexa or another bot to your server")]
        public async Task InviteCommand(CommandContext ctx, [Description("The snowflake of the bot you want to invite")] ulong? snowflake = null, [Description("The permissions integer")] int permissions = 0)
        {
            var hEmbed = new HexaEmbed(ctx, "bot invite");
            DiscordUser bot;
            if (snowflake is null)
            {
                hEmbed.embed.Description = $"[Invite me to your server!](https://discord.com/oauth2/authorize?client_id={ctx.Client.CurrentUser.Id}&permissions={permissions}&scope=bot)";
            }
            else
            {
                try { bot = await ctx.Client.GetUserAsync(snowflake.Value); }
                catch { await ctx.RespondAsync("The provided snowflake was invalid!"); return; }
                if (!bot.IsBot)
                {
                    await ctx.RespondAsync("I can only create invites for bots!");
                    return;
                }
                hEmbed.embed.Description = $"[Invite {bot.Username} to your server!](https://discord.com/oauth2/authorize?client_id={snowflake}&permissions={permissions}&scope=bot)";
            }
            await ctx.RespondAsync(embed: hEmbed.Build());
        }

        [GuildOnly]
        [Command("invite")]
        public async Task InviteCommand(CommandContext ctx, [Description("The bot you want to invite")] DiscordMember user = null, [Description("The permissions integer")] int permissions = 0)
        {
            var hEmbed = new HexaEmbed(ctx, "bot invite");
            if (user is null)
            {
                hEmbed.embed.Description = $"[Invite me to your server!](https://discord.com/oauth2/authorize?client_id={ctx.Client.CurrentUser.Id}&permissions={permissions}&scope=bot)";
            }
            else
            {
                if (!user.IsBot)
                {
                    await ctx.RespondAsync("I can only create invites for bots!");
                    return;
                }
                hEmbed.embed.Description = $"[Invite {user.Username} to your server!](https://discord.com/oauth2/authorize?client_id={user.Id}&permissions={permissions}&scope=bot)";
            }
            await ctx.RespondAsync(embed: hEmbed.Build());
        }
    }
}