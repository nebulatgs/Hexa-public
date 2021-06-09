using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
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
            bool isDm = ctx.Channel is DiscordDmChannel;
            bool isGuild = ctx.Guild is not null;
            if(isDm && !help)
                await ctx.RespondAsync("This command is only available in servers");
            else if(!isGuild && !help)
                await ctx.RespondAsync("I don't have access to that information in this server");
                // Console.WriteLine("I don't have access to that information in guild");
            return isGuild || help;
        }
    }
}