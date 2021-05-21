using System.Reflection;
using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharp​Plus.CommandsNext.Exceptions;
using DSharp​Plus.CommandsNext.Attributes;
using Microsoft.Extensions.Configuration;
using Hexa.Attributes;

namespace Hexa
{
    class Program
    {
        private readonly IConfiguration _config;
        public static string TOKEN;
        public Program()
        {
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");
            _config = _builder.Build();
            TOKEN = _config["Token"];
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
                AutoReconnect = true
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { _config["Prefix"] },
                EnableDms = true
            });

            commands.RegisterCommands(Assembly.GetExecutingAssembly());

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