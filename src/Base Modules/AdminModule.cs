using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using Hexa.Attributes;
using Hexa.Helpers;

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
        [Category(SettingsManager.HexaSetting.AdminCategory)]
        public async Task PurgeCommand(CommandContext ctx, [Description("The number of messages to delete")] int count)
        {
            await ctx.Message.DeleteAsync();
            await ctx.Channel.DeleteMessagesAsync(await ctx.Channel.GetMessagesAsync(count), $"Requested by {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator}");
        }

        [Command("kick")]
        [Aliases("remove")]
        [RequireBotPermissions(Permissions.KickMembers)]
        [Description("Kick a user")]
        [Category(SettingsManager.HexaSetting.AdminCategory)]
        public async Task KickCommand(CommandContext ctx, [Description("The user to kick")] DiscordMember user, [RemainingText, Description("The reason for kicking the user")] string reason = null)
        {
            string kickReason = $"Requested by {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator}";
            if (reason is not null)
                kickReason += $" : For reason: {reason}";
            try{ await user.RemoveAsync(kickReason); } catch(UnauthorizedException) { throw new UnauthorizedAccessException("I don't have permission to kick that user…");}
            await ctx.RespondAsync($"Kicked {user.Username}#{user.Discriminator}");
        }

        [Command("ban")]
        [RequireBotPermissions(Permissions.BanMembers)]
        [Description("Ban a user")]
        [Category(SettingsManager.HexaSetting.AdminCategory)]
        public async Task BanCommand(CommandContext ctx, [Description("The user to ban")] DiscordMember user, [Description("The number of messages to delete")] int messages_to_delete = 0, [RemainingText, Description("The reason for banning the user")] string reason = null)
        {
            string kickReason = $"Requested by {ctx.Message.Author.Username}#{ctx.Message.Author.Discriminator}";
            if (reason is not null)
                kickReason += $" : For reason: {reason}";
            try{ await user.BanAsync(messages_to_delete, kickReason); } catch(UnauthorizedException) { throw new UnauthorizedAccessException("I don't have permission to ban that user…");}
            await ctx.RespondAsync($"Banned {user.Username}#{user.Discriminator}");
        }

        // [Command("slash")]
        // [RequireBotPermissions(Permissions.ManageGuild)]
        // [Description("Ban a user")]
        // public async Task SlashCommand(CommandContext ctx)
        // {
        //     var options = new List<DiscordApplicationCommandOption>();
        //     options.Add(new DiscordApplicationCommandOption(
        //         "One",
        //         "one",
        //         ApplicationCommandOptionType.String,
        //         false
        //         )
        //     );
        //     await ctx.Client.CreateGuildApplicationCommandAsync(
        //         ctx.Guild.Id,
        //         new DiscordApplicationCommand(
        //             "test1",
        //             "test",
        //             options.AsEnumerable()
        //         )
        //     );
        // }
    }
}