using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharp​Plus.CommandsNext.Attributes;

namespace Hexa.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AdminOnlyAttribute : CheckBaseAttribute
    {

        public AdminOnlyAttribute()
        {
        }

        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            var member = await ctx.Guild.GetMemberAsync(ctx.Message.Author.Id);
            bool isAdmin = member.PermissionsIn(ctx.Channel).HasPermission(Permissions.Administrator) || Program.DevGroupIds.Contains(ctx.Message.Author.Id) ;
            if(ctx.Channel is DiscordDmChannel || !isAdmin)
            {
                await ctx.RespondAsync("This command is for admins only!");
                return false;
            }
            return true;
        }
    }
}