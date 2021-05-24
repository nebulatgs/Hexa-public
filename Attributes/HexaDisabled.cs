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
    public class DisabledAttribute : CheckBaseAttribute
    {

        public DisabledAttribute()
        {
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return Task.FromResult(false);
        }
    }
}