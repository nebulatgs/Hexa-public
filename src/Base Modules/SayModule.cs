using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Hexa.Attributes;
using Hexa.Helpers;

namespace Hexa.Modules
{
    [Hidden]
    [HexaCooldown(5)]
    [AdminOnly]
    public class SayModule : BaseCommandModule
    {
        [Command("say")]
        [Description("Make the bot say something")]
        [Category(SettingsManager.HexaSetting.FunCategory)]
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
        [RequireBotPermissions(Permissions.ManageMessages)]
        [Description("Make the bot say something and delete the original message")]
        [Category(SettingsManager.HexaSetting.FunCategory)]
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