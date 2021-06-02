using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Hexa.Helpers;
using Hexa.Attributes;

namespace Hexa.Modules{
    [HexaCooldown(60)]
    public class BugModule : BaseCommandModule
    {
        [Command("bugreport")]
        [Description("Report bugs")]
        [Aliases("bug", "report", "plshelpmethebotisbroken")]
        public async Task ReportCommand(CommandContext ctx, [RemainingText]string report)
        {
            if (report is null)
                throw new ArgumentException("Please provide a report");
            var reportChannel = await ctx.Client.GetChannelAsync(849330358700998669);
            var hEmbed = new HexaEmbed(ctx, "bug report");
            hEmbed.embed.WithTitle($"Bug Report by {ctx.Message.Author.Id} in {ctx.Guild.Id} ({ctx.Guild.Name})");
            hEmbed.embed.WithDescription(report);
            await reportChannel.SendMessageAsync(hEmbed.Build());

            hEmbed = new HexaEmbed(ctx, "bug report");
            hEmbed.embed.WithTitle($"Bug report filed");
            hEmbed.embed.WithDescription("Thank you for helping us improve Hexa!");
            await ctx.RespondAsync(hEmbed.Build());
        }
    }
}