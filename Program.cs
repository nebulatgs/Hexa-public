using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharp​Plus.CommandsNext.Attributes;
using DSharp​Plus.CommandsNext.Exceptions;
using Hexa.Attributes;
using Microsoft.Extensions.Configuration;
using Postgrest;
using Postgrest.Attributes;
using Postgrest.Extensions;
using Postgrest.Models;
using Supabase;
using Microsoft.Extensions.Logging;

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
        // public static Dictionary<ulong, HexaSettings> settings;
        public static string TOKEN;
        public Program()
        {
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");
            _config = _builder.Build();
            TOKEN = _config["Token"];
            // settings = new Dictionary<ulong, HexaSettings>();
            // settings.Add(0, new HexaSettings
            // {
            //     toggles = new Dictionary<string, bool>(){
            //         { "Ghost Ping Detection", false }
            //     }
            // });
        }
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();

        }

        public async Task MainAsync()
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = _config["Token"],
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Information
            });
            
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { _config["Prefix"] },
                EnableDms = true,
            });
            commands.RegisterCommands(Assembly.GetExecutingAssembly());
            commands.CommandExecuted += new HexaLogger("logfile.log").Log;
            commands.SetHelpFormatter<HexaHelp>();

            var url = "https://mjtsibevgbffwhfbctsh.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoiYW5vbiIsImlhdCI6MTYyMTY1Njk5NiwiZXhwIjoxOTM3MjMyOTk2fQ.Vk50me1kdQFmVrn2e4geY1qdg0_93yq-CGcJwfwKDf0";

            await Supabase.Client.InitializeAsync(url, key);

            discord.MessageDeleted += new Modules.GhostPingDetector().OnDelete;

            await discord.ConnectAsync();
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