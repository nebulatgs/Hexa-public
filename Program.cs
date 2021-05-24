using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Postgrest.Attributes;
using Postgrest.Models;

namespace Hexa
{

    [Table("ServerSettings")]
    public class GuildSetting : BaseModel
    {
        // [PrimaryKey("GuildSettingsId")]
        // public int Id { get; set; }
        [PrimaryKey("SettingId")]
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
            var services = new ServiceCollection().AddSingleton<HexaLogger>(logger).BuildServiceProvider();
            var commands = await discord.UseCommandsNextAsync(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { _config["Prefix"] },
                EnableDms = true,
                Services = services
            });

            await discord.UseInteractivityAsync(new InteractivityConfiguration
            {
                // default pagination behaviour to just ignore the reactions
                PaginationBehaviour = PaginationBehaviour.Ignore,
                PollBehaviour = PollBehaviour.DeleteEmojis,

                // default timeout for other actions to 2 minutes
                Timeout = TimeSpan.FromMinutes(2)
            });

            foreach (var command in commands)
            {
                command.Value.RegisterCommands(Assembly.GetExecutingAssembly());
                command.Value.CommandExecuted += logger.LogCommandExecution;
                command.Value.CommandErrored += logger.LogCommandError;
                command.Value.SetHelpFormatter<HexaHelp>();
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

            await discord.StartAsync();
            await Task.Delay(100);
            foreach (var client in discord.ShardClients)
            {
                var activity = new DiscordActivity($"{_config["Prefix"]}help | Shard {client.Key}", ActivityType.Playing);
                await client.Value.UpdateStatusAsync(activity, UserStatus.Online);
            }
            await Task.Delay(-1);
            // commands.CommandErrored += CmdErroredHandler;

        }

        // private async Task CmdErroredHandler(CommandsNextExtension _, CommandErrorEventArgs e)
        // {
        //     Console.WriteLine("hi");
        //     var failedChecks = ((ChecksFailedException)e.Exception).FailedChecks;
        //     foreach (var failedCheck in failedChecks)
        //     {
        //         if (failedCheck is GuildOnlyAttribute)
        //         {
        //             var cooldownAttribute = (GuildOnlyAttribute)failedCheck;
        //             // await e.Context.RespondAsync($"Only usable during year {cooldownAttribute.}.");
        //             await e.Context.RespondAsync($"You are using commands too fast! Try again in {5} seconds");
        //         }
        //     }
        // }
    }
}