using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

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
        [Category("Fun")]
        public async Task SayCommand(CommandContext ctx, [RemainingText][Description("What the bot should say")] string message = null)
        {   
            if (message is null)
            {
                await ctx.RespondAsync("What should I say?");
                return;
            }
            await ctx.Channel.SendMessageAsync(message);
        }

        [Command("sayd")]
        [RequireUserPermissions(Permissions.Administrator)]
        [RequireBotPermissions(Permissions.ManageMessages)]
        [Description("Make the bot say something and delete the original message")]
        [Category("Fun")]
        [GuildOnly]
        public async Task SayDCommand(CommandContext ctx, [RemainingText][Description("What the bot should say")] string message = null)
        {
            
            await ctx.Message.DeleteAsync();
            if (message is null)
                return;
            
            await ctx.Channel.SendMessageAsync(message);
        }
    }
}