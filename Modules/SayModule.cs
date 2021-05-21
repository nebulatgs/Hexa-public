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
    public class SayModule : BaseCommandModule
    {
        [Command("say")]
        [RequireUserPermissions(Permissions.Administrator)]
        [Description("Make the bot say something")]
        public async Task SayCommand(CommandContext ctx, [RemainingText][Description("What the bot should say")] string message)
        {   
            await ctx.Channel.SendMessageAsync(message);
        }

        [Command("sayd")]
        [RequireUserPermissions(Permissions.Administrator)]
        [RequireBotPermissions(Permissions.ManageMessages)]
        [Description("Make the bot say something and delete the original message")]
        [GuildOnly]
        public async Task SayDCommand(CommandContext ctx, [RemainingText][Description("What the bot should say")] string message)
        {
            await ctx.Message.DeleteAsync();
            await ctx.Channel.SendMessageAsync(message);
        }
    }
}