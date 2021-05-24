using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharp​Plus.CommandsNext.Exceptions;
using DSharp​Plus.CommandsNext.Attributes;
using System.Linq;
using DSharp​Plus.Entities;

namespace Hexa.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class HexaCooldownAttribute : CheckBaseAttribute
    {
        private Dictionary<ulong, DateTime> CooldownIds = new Dictionary<ulong, DateTime>();
        private List<ulong> MessageUserIds = new List<ulong>();
        private int CooldownSeconds;
        private int Repeats;
        private async Task CooldownMessage(CommandContext context)
        {
            var msg = await context.Channel.SendMessageAsync($"You're using commands too fast! Try again in {CooldownSeconds} seconds");
            await Task.Delay(TimeSpan.FromSeconds(CooldownSeconds));
            await msg.DeleteAsync();
        }

        public HexaCooldownAttribute(int seconds, int repeats = 4)   
        {
            CooldownSeconds = seconds;
            Repeats = repeats;
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            if(Program.DevGroupIds.Contains(ctx.Message.Author.Id))
                return Task.FromResult(true);
            MessageUserIds.Add(ctx.Message.Author.Id);
            if (CooldownIds.ContainsKey(ctx.Message.Author.Id))
                if (CooldownIds[ctx.Message.Author.Id].CompareTo(DateTime.Now) < 0)
                {
                    CooldownIds.Remove(ctx.Message.Author.Id);
                    MessageUserIds.RemoveAll(x => x == ctx.Message.Author.Id);
                    return Task.FromResult(true);
                }
                else
                    return Task.FromResult(false);
            if (MessageUserIds.Contains(ctx.Message.Author.Id))
                if (MessageUserIds.Where(x => x == ctx.Message.Author.Id).Count() > Repeats)
                {
                    CooldownIds.Add(ctx.Message.Author.Id, DateTime.Now.AddSeconds(CooldownSeconds));
                    CooldownMessage(ctx);
                    Console.WriteLine("Cooldown");
                    return Task.FromResult(false);
                }
            return Task.FromResult(true);
        }
    }
}