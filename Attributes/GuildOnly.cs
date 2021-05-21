using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharp​Plus.CommandsNext.Exceptions;
using DSharp​Plus.CommandsNext.Attributes;
using DSharp​Plus.Entities;

namespace Hexa.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class GuildOnlyAttribute : CheckBaseAttribute
    {

        public GuildOnlyAttribute()
        {
        }

        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            // return Task.FromResult(CooldownExpires < DateTime.Now);
            bool isGuild = ctx.Channel is not DiscordDmChannel;
            if(!isGuild)
                await ctx.RespondAsync("This command is only available in servers");
            return isGuild;
        }
    }
}