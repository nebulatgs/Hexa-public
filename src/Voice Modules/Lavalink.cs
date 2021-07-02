using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using DSharpPlus.Net;
using Hexa.Attributes;
using Hexa.Helpers;
using Humanizer;
using Humanizer.Localisation;
using SpotifyAPI.Web;

namespace Hexa.Modules
{
    [GuildOnly]
    public class LavalinkModule : BaseCommandModule
    {
        private Dictionary<ulong, List<LavalinkTrack>> queue = new();
        private Dictionary<ulong, bool> locked = new();
        public SpotifyClient spotify { get; set; }
        private async Task Register(CommandContext ctx, LavalinkGuildConnection conn)
        {
            try { await ctx.Guild.CurrentMember.SetDeafAsync(true); } catch { }
            conn.PlaybackFinished += FinishedEventHandler;
        }
        private async Task FinishedEventHandler(LavalinkGuildConnection conn, TrackFinishEventArgs args)
        {
            if (!queue[conn.Guild.Id].Any())
                return;
            queue[conn.Guild.Id].Remove(queue[conn.Guild.Id].First());
            if (!queue[conn.Guild.Id].Any())
                return;
            await conn.PlayAsync(queue[conn.Guild.Id].First());
        }
        private async Task<bool> PlayQueue(CommandContext ctx, LavalinkGuildConnection conn)
        {
            if (locked.ContainsKey(ctx.Guild.Id) && queue[ctx.Guild.Id].Count > 1)
                return false;
            // locked[ctx.Guild.Id] = true;
            locked.TryAdd(ctx.Guild.Id, true);
            await conn.PlayAsync(queue[ctx.Guild.Id].First());
            return true;
            // queue[ctx.Guild.Id].Remove(queue[ctx.Guild.Id].First());
            // foreach(var track in queue[ctx.Guild.Id])
            // {
            // await conn.PlayAsync(track);
            // conn.
            // }
        }
        // private async Task<FullTrack> SearchSpotify(string query)
        // {
        //     var track = await spotify.Search;
        //     track.Name
        // }
        [Command("join")]
        [Aliases("connect", "j", "summon")]
        [Description("Join a voice channel")]
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

            await Register(ctx, await link.ConnectAsync(channel));
            await ctx.RespondAsync($"Connected to {channel.Mention}");
        }

        [Command("play")]
        [Aliases("p")]
        [Description("Play music in a voice channel")]
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
                await Register(ctx, conn);
            }

            var hEmbed = new HexaEmbed(ctx, "hexa music");
            hEmbed.embed.Description = "fetching… <a:pinging:781983658646175764>";
            var message = await ctx.Channel.SendMessageAsync(hEmbed.Build());

            var tracks = await conn.GetTracksAsync(query);
            if (!tracks.Tracks.Any())
            {
                hEmbed.embed.WithDescription("Unable to find any results for your query!");
                await message.SafeModifyAsync(hEmbed.Build());
                return;
            }

            var track = tracks.Tracks.First();
            queue.TryAdd(ctx.Guild.Id, new());
            queue[ctx.Guild.Id].Add(tracks.Tracks.First());

            bool first = await PlayQueue(ctx, conn);
            if (first)
                hEmbed.embed.WithTitle($"Now Playing: {track.Title} by {track.Author}").WithUrl(track.Uri).WithDescription("");
            else
                hEmbed.embed.WithTitle($"Enqueued: {track.Title} by {track.Author}").WithUrl(track.Uri).WithDescription("");
            hEmbed.embed.WithThumbnail($"https://img.youtube.com/vi/{track.Identifier}/0.jpg");
            hEmbed.embed.AddField("Duration", track.Length.Humanize(3, maxUnit: TimeUnit.Hour, minUnit: TimeUnit.Second));

            await message.SafeModifyAsync(hEmbed.Build());
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
                await Register(ctx, conn);
            }

            var hEmbed = new HexaEmbed(ctx, "hexa music");
            hEmbed.embed.Description = "fetching… <a:pinging:781983658646175764>";
            var message = await ctx.Channel.SendMessageAsync(hEmbed.Build());
            var tracks = await conn.GetTracksAsync(url);
            if (!tracks.Tracks.Any())
            {
                hEmbed.embed.WithDescription("Unable to find any results for your query!");
                await message.SafeModifyAsync(hEmbed.Build());
                return;
            }

            var track = tracks.Tracks.First();
            queue.TryAdd(ctx.Guild.Id, new());
            queue[ctx.Guild.Id].Add(tracks.Tracks.First());

            bool first = await PlayQueue(ctx, conn);
            if (first)
                hEmbed.embed.WithTitle($"Now Playing: {track.Title} by {track.Author}").WithUrl(track.Uri).WithDescription("");
            else
                hEmbed.embed.WithTitle($"Enqueued: {track.Title} by {track.Author}").WithUrl(track.Uri).WithDescription("");
            hEmbed.embed.WithThumbnail($"https://img.youtube.com/vi/{track.Identifier}/0.jpg");

            hEmbed.embed.AddField("Duration", track.Length.Humanize(3, maxUnit: TimeUnit.Hour, minUnit: TimeUnit.Second));
            await message.SafeModifyAsync(hEmbed.Build());
        }

        [Command("skip")]
        [Aliases("s", "stop")]
        [Description("Skip tracks in the music queue")]
        [Category(SettingsManager.HexaSetting.VoiceCategory)]
        public async Task StopCommand(CommandContext ctx, int count = 1)
        {
            if (queue.ContainsKey(ctx.Guild.Id))
            {
                if (!queue[ctx.Guild.Id].Any())
                    throw new InvalidOperationException("There's nothing in the queue to be skipped!");
            }
            else
            {
                throw new InvalidOperationException("There's nothing in the queue to be skipped!");
            }
            var hEmbed = new HexaEmbed(ctx, "hexa music");
            hEmbed.embed.Description = "fetching… <a:pinging:781983658646175764>";
            var message = await ctx.Channel.SendMessageAsync(hEmbed.Build());

            var link = ctx.Client.GetLavalink().GetIdealNodeConnection();
            var conn = link.GetGuildConnection(ctx.Guild);
            var track = queue[ctx.Guild.Id].First();
            var newTrack = queue[ctx.Guild.Id].Count() > 1 ? queue[ctx.Guild.Id][1] : null;

            await conn.StopAsync();
            if (queue[ctx.Guild.Id].Count() > 1)
                hEmbed.embed.WithTitle($"Skipped: {track.Title} by {track.Author}").WithUrl(track.Uri).WithDescription($"[Now Playing: {newTrack.Title} by {newTrack.Author}]({newTrack.Uri})");
            else
                hEmbed.embed.WithTitle($"Skipped: {track.Title} by {track.Author}").WithUrl(track.Uri).WithDescription("The queue is now empty");
            await message.SafeModifyAsync(hEmbed.Build());
        }

        [Command("leave")]
        [Aliases("l")]
        [Description("Leave the current voice channel")]
        [Category(SettingsManager.HexaSetting.VoiceCategory)]
        public async Task LeaveCommand(CommandContext ctx)
        {
            var conn = ctx.Client.
                GetLavalink().
                GetIdealNodeConnection().
                GetGuildConnection(ctx.Guild);
            if (conn is null)
                throw new InvalidOperationException("I'm not connected to a voice channel!");
            await conn.DisconnectAsync();
            await ctx.RespondAsync($"Left {conn.Channel.Mention}");
        }

        [Command("queue")]
        [Aliases("q")]
        [Description("View all tracks in the queue")]
        [Category(SettingsManager.HexaSetting.VoiceCategory)]
        public async Task QueueCommand(CommandContext ctx)
        {
            var hEmbed = new HexaEmbed(ctx, "hexa music");
            hEmbed.embed.WithDescription("fetching… <a:pinging:781983658646175764>");
            var message = await ctx.Channel.SendMessageAsync(hEmbed.Build());
            hEmbed.embed.WithTitle("Current Queue");
            if (!queue.ContainsKey(ctx.Guild.Id) || (!queue?[ctx.Guild.Id]?.Any() ?? false))
            {
                hEmbed.embed.WithDescription("The queue is empty");
                await message.SafeModifyAsync(hEmbed.Build());
                return;
            }
            int i = 0;
            var pages = new List<Page>();
            int page_index = 1;
            var chunks = queue[ctx.Guild.Id].Chunk(10);
            foreach (var chunk in chunks)
            {
                hEmbed = new HexaEmbed(ctx, "hexa music").WithFooter($"Page {page_index} of {Math.Ceiling(queue[ctx.Guild.Id].Count / 10.0)}");
                hEmbed.embed.WithTitle($"Current Queue ({TimeSpan.FromSeconds(queue[ctx.Guild.Id].Sum(x => x.Length.TotalSeconds)).Humanize(3)})");
                foreach (var track in chunk)
                {
                    i++;
                    hEmbed.embed.Description += $"\n**Track {i}:** [{track.Title}]({track.Uri}) by {track.Author} ({track.Length.Humanize(3, maxUnit: TimeUnit.Hour, minUnit: TimeUnit.Second)})";
                }
                pages.Add(new(embed: hEmbed.embed));
                page_index++;
            }
            var interactivity = ctx.Client.GetInteractivity();
            await interactivity.SendButtonPaginatedMessageAsync(ctx.Channel, ctx.Message.Author, pages, "queue", timeout: TimeSpan.FromSeconds(60), msg: message, showPrint: false);
        }

        [Command("clearqueue")]
        [Aliases("cq")]
        [Description("Clear the queue")]
        [Category(SettingsManager.HexaSetting.VoiceCategory)]
        public async Task ClearQueueCommand(CommandContext ctx)
        {
            var hEmbed = new HexaEmbed(ctx, "hexa music");
            if (!queue.ContainsKey(ctx.Guild.Id) || (!queue?[ctx.Guild.Id]?.Any() ?? false))
                throw new InvalidOperationException("The queue is already empty!");
            queue.Remove(ctx.Guild.Id);
            var link = ctx.Client.GetLavalink().GetIdealNodeConnection();
            var conn = link.GetGuildConnection(ctx.Guild);
            await conn.StopAsync();
            hEmbed.embed.WithDescription("The queue is now empty");
            await ctx.RespondAsync(hEmbed.Build());
            return;
        }

        [Command("box")]
        [Category(SettingsManager.HexaSetting.RandomCategory)]
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
                await Register(ctx, conn);
                // await ctx.RespondAsync($"Connected to {channel.Mention}");
            }
            var hEmbed = new HexaEmbed(ctx, "hexa music");
            hEmbed.embed.Description = "fetching… <a:pinging:781983658646175764>";
            var message = await ctx.Channel.SendMessageAsync(hEmbed.Build());
            // await conn.StopAsync();
            var tracks = await conn.GetTracksAsync(new Uri("https://www.youtube.com/watch?v=LeEBVtsqej0"));
            queue.TryAdd(ctx.Guild.Id, new());
            queue[ctx.Guild.Id].Add(tracks.Tracks.First());
            await PlayQueue(ctx, conn);
            // await conn.PlayAsync(tracks.Tracks.First());
            hEmbed.embed.WithImageUrl("https://cdn.offline.codes/21/06/1159-iTX5fx2s2y.gif")
                        .WithDescription("")
                        .WithTitle("Box")
                        .WithUrl("https://box.cubedhuang.com");
            await message.SafeModifyAsync(hEmbed.Build());
        }
    }
}