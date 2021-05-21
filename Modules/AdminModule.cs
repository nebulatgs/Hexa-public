using System.Linq;
using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus;
using Hexa.Attributes;

namespace Hexa.Modules
{
    [Hidden]
    [HexaCooldown(5)]
    [GuildOnly]
    public class AdminModule : BaseCommandModule
    {
        [Command("purge")]
        [Aliases("delete")]
        [RequireUserPermissions(Permissions.Administrator)]
        [RequireBotPermissions(Permissions.ManageMessages)]
        [Description("Delete X messages")]
        public async Task PurgeCommand(CommandContext ctx, [RemainingText][Description("The number of messages to delete")] int count)
        {
            await ctx.Message.DeleteAsync();
            await ctx.Channel.DeleteMessagesAsync(await ctx.Channel.GetMessagesAsync(count), $"Requested by {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator}");
        }

        [Command("kick")]
        [Aliases("remove")]
        [RequireUserPermissions(Permissions.Administrator)]
        [RequireBotPermissions(Permissions.ManageGuild)]
        [Description("Kick a user")]        
        public async Task KickCommand(CommandContext ctx, [Description("The user to kick")] DiscordMember user)
        {
            await user.RemoveAsync($"Requested by {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator}");
            await ctx.RespondAsync($"Kicked {user.Username}#{user.Discriminator}");
        }

        [Command("ban")]
        [RequireUserPermissions(Permissions.Administrator)]
        [RequireBotPermissions(Permissions.ManageGuild)]
        [Description("Ban a user")]        
        public async Task BanCommand(CommandContext ctx, [Description("The user to ban")] DiscordMember user, [Description("The number of messages to delete")] int messages_to_delete = 0)
        {   
            await user.BanAsync(messages_to_delete, $"Requested by {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator}");
            await ctx.RespondAsync($"Banned {user.Username}#{user.Discriminator}");
        }
    }
}