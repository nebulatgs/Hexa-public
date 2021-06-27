using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DiscordBotsList.Api;
using DiscordBotsList.Api.Objects;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Hexa.Helpers;
using Hexa.Other;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hexa
{

    // [Table("ServerSettings")]
    // public class GuildSetting : SupabaseModel
    // {
    //     [PrimaryKey("SettingID", false)]
    //     public int SettingID { get; set; }

    //     [Column("GuildId")]
    //     public ulong GuildId { get; set; }

    //     [Column("SettingType")]
    //     public int SettingType { get; set; }

    //     [Column("Value")]
    //     public string Value { get; set; }


    //     public override bool Equals(object obj)
    //     {
    //         return obj is GuildSetting message &&
    //                 GuildId == message.GuildId;
    //     }

    //     public override int GetHashCode()
    //     {
    //         return HashCode.Combine(GuildId);
    //     }
    // }

    class Program
    {
        private readonly IConfiguration _config;
        public static DateTime LaunchTime { get; private set; }
        public static string TOKEN { get; private set; }
        public static List<ulong> DevGroupIds { get; set; }
        public static string DBSTRING { get; private set; }
        public static string DBOTS_TOKEN { get; private set; }
        public static string DBL_TOKEN { get; private set; }
        public static string DSKEY { get; private set; }
        public static string LAVALINK_PW { get; set; }
        public static HexaLogger Logger { get; set; }
        public Program()
        {
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");
            _config = _builder.Build();
            if (Environment.GetEnvironmentVariable("PROD") is not null)
            {
                DBSTRING = Environment.GetEnvironmentVariable("DBSTRING");
                TOKEN = Environment.GetEnvironmentVariable("BOT_TOKEN");
                DSKEY = Environment.GetEnvironmentVariable("DSKEY");
                LAVALINK_PW = Environment.GetEnvironmentVariable("LAVALINK");
                DBOTS_TOKEN = Environment.GetEnvironmentVariable("DBOTS_TOKEN");
                DBL_TOKEN = Environment.GetEnvironmentVariable("DBL_TOKEN");
                Environment.SetEnvironmentVariable("DBSTRING", null);
                Environment.SetEnvironmentVariable("BOT_TOKEN", null);
                Environment.SetEnvironmentVariable("DSKEY", null);
                Environment.SetEnvironmentVariable("LAVALINK", null);
                Environment.SetEnvironmentVariable("DBOTS_TOKEN", null);
                Environment.SetEnvironmentVariable("DBL_TOKEN", null);
            }
            else
            {
                DBSTRING = _config["Dbstring"];
                TOKEN = _config["Token"];
                DSKEY = _config["DarkSky-key"];
                LAVALINK_PW = _config["Lavalink-PW"];
                DBOTS_TOKEN = _config["Dbots_Token"];
                DBL_TOKEN = _config["Dbl_Token"];
            }
            DevGroupIds = new List<ulong>();
            var devs = _config["Devs"].Split(',');
            foreach (var dev in devs)
                DevGroupIds.Add(ulong.Parse(dev.Trim()));
            LaunchTime = DateTime.Now;
        }
        static void Main(string[] args)
        {
            new Program()
                .MainAsync()
                .GetAwaiter()
                .GetResult();
        }

        public async Task MainAsync()
        {
            var discord = new DiscordShardedClient(new DiscordConfiguration()
            {
                Token = TOKEN,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                ShardCount = 3
            });

            string url, key;
            if (Environment.GetEnvironmentVariable("PROD") is null)
            {
                url = _config["Supabase_Url"];
                key = _config["Supabase_Token"];
            }
            else
            {
                url = Environment.GetEnvironmentVariable("SUPABASE_URL");
                key = Environment.GetEnvironmentVariable("SUPABASE_TOKEN");

                Environment.SetEnvironmentVariable("SUPABASE_URL", null);
                Environment.SetEnvironmentVariable("SUPABASE_TOKEN", null);
            }
            await Supabase.Client.InitializeAsync(url, key);
            await discord.UseInteractivityAsync(new()
            {
                // default pagination behaviour to just ignore the reactions
                PaginationBehaviour = PaginationBehaviour.Ignore,
                PollBehaviour = PollBehaviour.DeleteEmojis,
                ResponseBehavior = InteractionResponseBehavior.Ack,
                // default timeout for other actions to 2 minutes
                Timeout = TimeSpan.FromMinutes(2)
            });

            HexaLogger logger = new HexaLogger($"logs/{DateTime.Now.ToString("u").Replace(':', '.')}.log") { _client = discord.ShardClients.First().Value };
            Logger = logger;
            HexaCommandHandler command_handler = new(_config["Prefix"]);
            ProfanityFilter.ProfanityFilter filter = new();
            SnipeHelper snipeHelper = new();
            AllowList.AddAllowed(filter);
            var services = new ServiceCollection().
                AddSingleton<HexaLogger>(logger).
                AddSingleton<Random>().
                AddSingleton<SettingsManager>().
                AddSingleton<ProfanityFilter.ProfanityFilter>(filter).
                AddSingleton<SnipeHelper>(snipeHelper).
                AddSingleton<DiscordShardedClient>(discord).
                BuildServiceProvider();
            var commands = await discord.UseCommandsNextAsync(new()
            {
                StringPrefixes = new[] { _config["Prefix"] },
                EnableDms = true,
                UseDefaultCommandHandler = false,
                Services = services
            });

            // await discord.UseVoiceNextAsync(new VoiceNextConfiguration(){
            //     AudioFormat = new(
            //         sampleRate: 48000,
            //         channelCount: 2,
            //         voiceApplication: VoiceApplication.Voice
            //     )
            // });

            discord.GuildMemberUpdated += new UsernameChangeLogger().OnChange;
            discord.GuildCreated += new JoinLeaveLogger().OnChange;
            discord.GuildDeleted += new JoinLeaveLogger().OnChange;
            var guild_levels = new GuildLevels();
            var user_levels = new UserLevels();
            var lavaconfig = new LavalinkConfiguration
            {
                RestEndpoint = new() { Hostname = "138.197.231.194", Port = 8080 },
                SocketEndpoint = new() { Hostname = "138.197.231.194", Port = 8080 },
                Password = LAVALINK_PW
            };
            foreach (var client in discord.ShardClients)
            {
                var slash = client.Value.UseSlashCommands();
                if (Environment.GetEnvironmentVariable("PROD") is not null)
                {
                    slash.RegisterCommands<Modules.ActivitySlash>();
                    slash.RegisterCommands<Modules.AvatarSlash>();
                    // slash.RegisterCommands<Modules.ButtonSlash>();
                }
                else
                {
                    slash.RegisterCommands<Modules.ActivitySlash>(844754896358998018);
                    slash.RegisterCommands<Modules.AvatarSlash>(844754896358998018);
                    slash.RegisterCommands<Modules.ButtonSlash>(847891805185245217);
                }
                slash.SlashCommandExecuted += logger.LogSlashCommandExecution;
                slash.SlashCommandErrored += logger.LogSlashCommandError;
            }
            foreach (var command in commands)
            {
                command.Value.RegisterCommands(Assembly.GetExecutingAssembly());
                command.Value.CommandExecuted += logger.LogCommandExecution;

                command.Value.CommandExecuted += guild_levels.CommandExecuted;
                command.Value.CommandExecuted += user_levels.CommandExecuted;

                command.Value.CommandErrored += logger.LogCommandError;
                command.Value.CommandErrored += CmdErroredHandler;
                command.Value.SetHelpFormatter<HexaHelpFormatter>();
                command.Value.RegisterConverter(new Converters.BoolConverter());
                command.Value.RegisterConverter(new Converters.HexaSettingConverter());
            }



            discord.MessageDeleted += snipeHelper.MessageDeleted;
            discord.MessageDeleted += new Modules.GhostPingDetector().OnDelete;
            discord.MessageCreated += command_handler.CommandHandler;
            discord.MessageCreated += guild_levels.MessageSent;
            discord.MessageCreated += user_levels.MessageSent;


            discord.ComponentInteractionCreated += async (DiscordClient client, ComponentInteractionCreateEventArgs args) =>
            {
                await args.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
            };
            await discord.StartAsync();
            foreach (var client in discord.ShardClients)
            {
                var lavalink = client.Value.UseLavalink();
                await lavalink.ConnectAsync(lavaconfig);
            }
            IDblSelfBot dbl_me = null;
            if (Environment.GetEnvironmentVariable("PROD") is not null)
            {
                AuthDiscordBotListApi DblApi = new(discord.CurrentUser.Id, DBL_TOKEN);
                dbl_me = await DblApi.GetMeAsync();
            }
            await PeriodicTask.Run(async () =>
            {
                int guildCount = discord.ShardClients.Sum(client => client.Value.Guilds.Sum(x => x.Value.MemberCount));
                if (Environment.GetEnvironmentVariable("PROD") is not null)
                    await dbl_me.UpdateStatsAsync(0, discord.ShardClients.Count, discord.ShardClients.Select(client => client.Value.Guilds.Count).ToArray());
                foreach (var client in discord.ShardClients)
                {
                    var activity = new DiscordActivity($"{_config["Prefix"]}help | {guildCount.ToString("N0")} users", ActivityType.Playing);
                    await client.Value.UpdateStatusAsync(activity, UserStatus.Online);
                }
                ServerTime.FetchServerTimeDifference();
                // PostDBotsStats(discord.CurrentUser.Id, guildCount);
            }, TimeSpan.FromMinutes(1));
        }
        private async Task CmdErroredHandler(CommandsNextExtension _, CommandErrorEventArgs e)
        {
            if (e.Exception is ChecksFailedException) return;
            await e.Context.RespondAsync(e.Exception.Message);
        }
        private void PostDBotsStats(ulong clientSnowflake, int guildCount)
        {
            var url = $"https://discord.bots.gg/api/v1/bots/{clientSnowflake}/stats";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.Headers["Authorization"] = Program.DBOTS_TOKEN;
            httpRequest.ContentType = "application/json";

            var data = "{\"guildCount\": " + guildCount + "}";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine("Posted stats to bots.gg");
            }
            else
            {
                Console.WriteLine("Failed to post stats to bots.gg");
            }
        }
    }

    public class PeriodicTask
    {
        public static async Task Run(Action action, TimeSpan period, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(period, cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                    action();
            }
        }

        public static Task Run(Action action, TimeSpan period)
        {
            return Run(action, period, CancellationToken.None);
        }
    }

}