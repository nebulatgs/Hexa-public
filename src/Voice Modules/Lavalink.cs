using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using Hexa.Attributes;
using Hexa.Helpers;

namespace Hexa.Modules
{
    [Hidden]
    public class LavalinkModule : BaseCommandModule
    {
        [Command("join")]
        [Aliases("connect", "j", "summon")]
        [Category(SettingsManager.HexaSetting.VoiceCategory)]
        public async Task ConnectCommand(CommandContext ctx, DiscordChannel channel = null)
        {
            var vstat = ctx.Member?.VoiceState;
            if (vstat?.Channel is null && channel is null)
                throw new ArgumentException("You must specify a channel or join a voice channel first");
            if (channel is null)
                channel = vstat.Channel;
            else if (channel?.Type != ChannelType.Voice)
                throw new ArgumentException("I can only connect to voice channels");

            var link = ctx.Client.GetLavalink().GetIdealNodeConnection();
            await link.ConnectAsync(channel);
            await ctx.RespondAsync($"Connected to {channel.Mention}");
        }

        [Command("play")]
        [Aliases("p")]
        [Category(SettingsManager.HexaSetting.VoiceCategory)]
        public async Task PlayCommand(CommandContext ctx, [RemainingText] string query)
        {
            var link = ctx.Client.GetLavalink().GetIdealNodeConnection();
            var conn = link.GetGuildConnection(ctx.Guild);
            if (conn is null)
            {
                var vstat = ctx.Member?.VoiceState;
                if (vstat?.Channel is null)
                    throw new ArgumentException("You must join a voice channel first");
                var channel = vstat.Channel;
                conn = await link.ConnectAsync(channel);
                // await ctx.RespondAsync($"Connected to {channel.Mention}");
            }
            var tracks = await conn.GetTracksAsync(query);
            var self = await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id);
            await self.SetDeafAsync(true);
            var hEmbed = new HexaEmbed(ctx, "hexa music");
            hEmbed.embed.Description = "fetching… <a:pinging:781983658646175764>";
            var message = await ctx.Channel.SendMessageAsync(hEmbed.Build());
            await conn.StopAsync();
            var track = tracks.Tracks.First();
            var StartTime = DateTime.Now;
            hEmbed.embed.WithTitle($"Now Playing: {track.Title} by {track.Author}").WithUrl(track.Uri).WithDescription("");
            hEmbed.embed.AddField("Duration", track.Length.ToString("c"), true);
            var pos = (DateTime.Now - StartTime);
            hEmbed.embed.AddField("Position", pos.ToString(@"hh\:mm\:ss\.ff"), true);
            // hEmbed.embed.RemoveFieldAt(1);
            // hEmbed.WithFooter(track.Author);
            await message.ModifyAsync(hEmbed.Build());
            await conn.PlayAsync(tracks.Tracks.First());
            // var playing = Task.Delay(track.Length);
            bool canceled = false;
            conn.PlaybackFinished += async (LavalinkGuildConnection _, TrackFinishEventArgs __) => {canceled = true;};
            while (!canceled)
            {
                hEmbed.embed.RemoveFieldAt(1);
                pos = (DateTime.Now - StartTime);
                hEmbed.embed.AddField("Position", pos.ToString(@"hh\:mm\:ss\.ff"), true);
                await message.ModifyAsync(hEmbed.Build());
                await Task.Delay(5000);
            }
        }

        [Command("play")]
        public async Task PlayCommand(CommandContext ctx, Uri url)
        {
            var link = ctx.Client.GetLavalink().GetIdealNodeConnection();
            var conn = link.GetGuildConnection(ctx.Guild);
            if (conn is null)
            {
                var vstat = ctx.Member?.VoiceState;
                if (vstat?.Channel is null)
                    throw new ArgumentException("You must join a voice channel first");
                var channel = vstat.Channel;
                conn = await link.ConnectAsync(channel);
                // await ctx.RespondAsync($"Connected to {channel.Mention}");
            }
            var tracks = await conn.GetTracksAsync(url);
            var self = await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id);
            await self.SetDeafAsync(true);
            var hEmbed = new HexaEmbed(ctx, "hexa music");
            hEmbed.embed.Description = "fetching… <a:pinging:781983658646175764>";
            var message = await ctx.Channel.SendMessageAsync(hEmbed.Build());
            await conn.StopAsync();
            var track = tracks.Tracks.First();
            hEmbed.embed.WithTitle($"Now Playing: {track.Title} by {track.Author}").WithUrl(track.Uri).WithDescription("");
            hEmbed.embed.AddField("Duration", track.Length.ToString("c"));
            // hEmbed.WithFooter(track.Author);
            await message.ModifyAsync(hEmbed.Build());
            await conn.PlayAsync(track);

        }

        [Command("skip")]
        [Aliases("s", "stop")]
        public async Task StopCommand(CommandContext ctx)
        {
            var link = ctx.Client.GetLavalink().GetIdealNodeConnection();
            var conn = link.GetGuildConnection(ctx.Guild);
            await conn.StopAsync();
        }

        [Command("box")]
        public async Task BoxCommand(CommandContext ctx)
        {
            var link = ctx.Client.GetLavalink().GetIdealNodeConnection();
            var conn = link.GetGuildConnection(ctx.Guild);
            if (conn is null)
            {
                var vstat = ctx.Member?.VoiceState;
                if (vstat?.Channel is null)
                    throw new ArgumentException("You must join a voice channel first");
                var channel = vstat.Channel;
                conn = await link.ConnectAsync(channel);
                // await ctx.RespondAsync($"Connected to {channel.Mention}");
            }
            var tracks = await conn.GetTracksAsync(new Uri("https://cdn.offline.codes/21/06/hip_shop.ogg"));
            await conn.PlayAsync(tracks.Tracks.First());
        }
    }
}