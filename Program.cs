using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Hexa.Helpers;
using Hexa.Other;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Postgrest.Attributes;
using Postgrest.Models;
using Supabase;

namespace Hexa
{

    [Table("ServerSettings")]
    public class GuildSetting : SupabaseModel
    {
        [PrimaryKey("SettingId", false)]
        public int SettingId { get; set; }

        [Column("GuildId")]
        public ulong GuildId { get; set; }

        [Column("SettingType")]
        public int SettingTypeId { get; set; }

        [Column("Value")]
        public string Value { get; set; }


        public override bool Equals(object obj)
        {
            return obj is GuildSetting message &&
                    GuildId == message.GuildId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GuildId);
        }
    }

    class Program
    {
        private readonly IConfiguration _config;
        public static DateTime LaunchTime { get; private set; }
        public static string TOKEN { get; private set; }
        public static List<ulong> DevGroupIds { get; set; }
        public Program()
        {
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");
            _config = _builder.Build();
            if (Environment.GetEnvironmentVariable("PROD") is not null)
                TOKEN = Environment.GetEnvironmentVariable("BOT_TOKEN");
            else
                TOKEN = _config["Token"];
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
                MinimumLogLevel = LogLevel.Information
            });
            HexaLogger logger = new HexaLogger($"logs/{DateTime.Now.ToString("u").Replace(':', '.')}.log");
            HexaCommandHandler command_handler = new HexaCommandHandler(_config["Prefix"]);
            var services = new ServiceCollection().AddSingleton<HexaLogger>(logger).AddSingleton<Random>().BuildServiceProvider();
            var commands = await discord.UseCommandsNextAsync(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { _config["Prefix"] },
                EnableDms = true,
                UseDefaultCommandHandler = false,
                Services = services
            });

            await discord.UseInteractivityAsync(new InteractivityConfiguration
            {
                // default pagination behaviour to just ignore the reactions
                PaginationBehaviour = PaginationBehaviour.Ignore,
                PollBehaviour = PollBehaviour.DeleteEmojis,
                ResponseBehavior = InteractionResponseBehavior.Ack,
                // default timeout for other actions to 2 minutes
                Timeout = TimeSpan.FromMinutes(2)
            });
            discord.GuildMemberUpdated += new UsernameChangeLogger().OnChange;
            discord.GuildCreated += new JoinLeaveLogger().OnChange;
            discord.GuildDeleted += new JoinLeaveLogger().OnChange;
            foreach (var client in discord.ShardClients)
            {
                var slash = client.Value.UseSlashCommands();
                if (Environment.GetEnvironmentVariable("PROD") is not null)
                {
                    slash.RegisterCommands<Modules.ActivitySlash>();
                    slash.RegisterCommands<Modules.AvatarSlash>();
                    slash.RegisterCommands<Modules.ButtonSlash>();
                }
                else
                {
                    slash.RegisterCommands<Modules.ActivitySlash>(844754896358998018);
                    slash.RegisterCommands<Modules.AvatarSlash>(844754896358998018);
                    slash.RegisterCommands<Modules.ButtonSlash>(844754896358998018);
                }
            }
            var guild_levels = new GuildLevels();
            var user_levels = new UserLevels();
            foreach (var command in commands)
            {
                command.Value.RegisterCommands(Assembly.GetExecutingAssembly());
                command.Value.CommandExecuted += logger.LogCommandExecution;

                command.Value.CommandExecuted += guild_levels.CommandExecuted;
                command.Value.CommandExecuted += user_levels.CommandExecuted;

                command.Value.CommandErrored += logger.LogCommandError;
                command.Value.CommandErrored += CmdErroredHandler;
                command.Value.SetHelpFormatter<HexaHelpFormatter>();
            }

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
            }
            await Supabase.Client.InitializeAsync(url, key);

            discord.MessageDeleted += new Modules.GhostPingDetector().OnDelete;
            discord.MessageCreated += command_handler.CommandHandler;
            discord.MessageCreated += guild_levels.MessageSent;
            discord.MessageCreated += user_levels.MessageSent;


            discord.ComponentInteractionCreated += async (DiscordClient client, ComponentInteractionCreateEventArgs args) =>
            {
                await args.Interaction.CreateResponseAsync(InteractionResponseType.DefferedMessageUpdate);
                // await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                // {
                //     Content = "Wow",
                //     IsEphemeral = true
                // });
                // await args.Message.ModifyAsync("Ok");
            };
            await discord.StartAsync();
            await PeriodicTask.Run(async () =>
            {
                foreach (var client in discord.ShardClients)
                {
                    var activity = new DiscordActivity($"{_config["Prefix"]}help | {client.Value.Guilds.Sum(x => x.Value.MemberCount).ToString("N0")} users", ActivityType.Playing);
                    await client.Value.UpdateStatusAsync(activity, UserStatus.Online);
                }
                ServerTime.FetchServerTimeDifference();
            }, TimeSpan.FromMinutes(1));
        }
        private async Task CmdErroredHandler(CommandsNextExtension _, CommandErrorEventArgs e)
        {
            await e.Context.RespondAsync(e.Exception.Message);
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