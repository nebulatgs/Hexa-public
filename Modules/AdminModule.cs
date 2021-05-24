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
    [AdminOnly]
    public class AdminModule : BaseCommandModule
    {
        [Command("purge")]
        [Aliases("delete")]
        [RequireBotPermissions(Permissions.ManageMessages)]
        [Description("Delete X messages")]
        public async Task PurgeCommand(CommandContext ctx, [Description("The number of messages to delete")] int count)
        {
            await ctx.Message.DeleteAsync();
            await ctx.Channel.DeleteMessagesAsync(await ctx.Channel.GetMessagesAsync(count), $"Requested by {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator}");
        }

        [Command("kick")]
        [Aliases("remove")]
        [RequireBotPermissions(Permissions.ManageGuild)]
        [Description("Kick a user")]        
        public async Task KickCommand(CommandContext ctx, [Description("The user to kick")] DiscordMember user, [RemainingText, Description("The reason for kicking the user")] string reason = null)
        {
            string kickReason = $"Requested by {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator}";
            if(reason is not null)
                kickReason += $" : For reason: {reason}";
            await user.RemoveAsync(kickReason);
            await ctx.RespondAsync($"Kicked {user.Username}#{user.Discriminator}");
        }

        [Command("ban")]
        [RequireBotPermissions(Permissions.ManageGuild)]
        [Description("Ban a user")]        
        public async Task BanCommand(CommandContext ctx, [Description("The user to ban")] DiscordMember user, [Description("The number of messages to delete")] int messages_to_delete = 0, [RemainingText, Description("The reason for banning the user")] string reason = null)
        {   
            string kickReason = $"Requested by {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator}";
            if(reason is not null)
                kickReason += $" : For reason: {reason}";
            await user.BanAsync(messages_to_delete, kickReason);
            await ctx.RespondAsync($"Banned {user.Username}#{user.Discriminator}");
        }
    }
}