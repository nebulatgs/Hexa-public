using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharp​Plus.CommandsNext.Attributes;

namespace Hexa.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class DevOnlyAttribute : CheckBaseAttribute
    {

        public DevOnlyAttribute()
        {
        }

        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return Program.DevGroupIds.Contains(ctx.Message.Author.Id);
        }
    }
}