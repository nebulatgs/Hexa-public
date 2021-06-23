using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Net;
using Hexa.Attributes;
using Hexa.Helpers;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace Hexa.Modules
{
    [Hidden]
    [DevOnly]
    [HexaCooldown(5)]
    public class DevModule : BaseCommandModule
    {
        private Microsoft.CodeAnalysis.Scripting.Script<object> state;
        public DevModule()
        {
            var scriptOptions = Microsoft.CodeAnalysis.Scripting.ScriptOptions.Default
                               .AddReferences(Assembly.GetExecutingAssembly());
                            //    .AddReferences("System")
                            //    .AddReferences("System.Linq")
                            //    .AddReferences(typeof(System.Linq.Enumerable).Assembly)
                            //    .AddReferences(typeof(DSharpPlus.DiscordClient).Assembly)
                            //    .AddReferences(typeof(DSharpPlus.Lavalink.DiscordClientExtensions).Assembly)
                            //    .AddReferences(typeof(DSharpPlus.CommandsNext.BaseCommandModule).Assembly)
                            //    .AddReferences(typeof(Hexa.Helpers.HexaEmbed).Assembly);
            // .AddReferences(Assembly.GetExecutingAssembly());
            // .AddReferences("Microsoft.CSharp")
            state = CSharpScript.Create(
              @"using System;
                using System.Threading.Tasks;

                using DSharpPlus;
                using DSharpPlus.CommandsNext;
                using DSharpPlus.CommandsNext.Attributes;
                using DSharpPlus.Entities;
                using DSharpPlus.Lavalink;

                using Hexa.Attributes;
                using Hexa.Helpers;
                using System.Linq;
                using DSharpPlus.Net;
                ",
                globalsType: typeof(CommandContext)
            )
            .WithOptions(scriptOptions);
            // state.AddReference(typeof(System.Linq.Enumerable).Assembly.Location);
        }
        [Command("evaluate")]
        [Aliases("eval", "ev")]
        [Description("Evaluate an expression")]
        [Category(SettingsManager.HexaSetting.DangerCategory)]
        public async Task EvaluateCommand(CommandContext ctx, [RemainingText] string code)
        {
            await ctx.Channel.TriggerTypingAsync();
            try
            {
                var newState = state.ContinueWith(code);
                var response = await newState.RunAsync(ctx);
                await ctx.RespondAsync(response.ReturnValue.ToString());
            }
            catch (Exception e)
            {
                await ctx.RespondAsync($"Error evaluating expression: {e.Message}");
            }
        }

        [Command("shard")]
        [Category(SettingsManager.HexaSetting.DangerCategory)]
        public async Task ShardCommand(CommandContext ctx)
        {
           await ctx.RespondAsync(ctx.Client.ShardId.ToString("N0"));
        }
        
        [Command("sleave")]
        [Description("Leave a server")]
        [Category(SettingsManager.HexaSetting.DangerCategory)]
        public async Task LeaveCommand(CommandContext ctx, ulong? server)
        {
            if (server is null)
                throw new ArgumentNullException("What server should I leave?");
            var guild = await ctx.Client.GetGuildAsync(server.Value);
            await guild.LeaveAsync();
            await ctx.RespondAsync($"Left {guild}…");
        }

        // [Command("dm")]
        // public async Task DMCommand(CommandContext ctx, DiscordMember user, [RemainingText] string message)
        // {
        //     // await user.SendMessageAsync(message);

        // }

        [Command("dm")]
        [Description("DM a user")]
        [Category(SettingsManager.HexaSetting.DangerCategory)]
        public async Task DMCommand(CommandContext ctx, ulong user, [RemainingText] string message)
        {
            var apiClient = (DiscordApiClient)typeof(DiscordClient).GetProperty("ApiClient", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(ctx.Client)!;
            var channel = await (Task<DiscordDmChannel>)typeof(DiscordApiClient).GetMethod("CreateDmAsync", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(apiClient, new object[] { user })!;
            // int a = 1;
            // var guild = ctx.Client.Guilds.First(x => x.Value.GetMemberAsync(user).GetAwaiter().GetResult() is not null);
            // var member = await guild.Value.GetMemberAsync(user);
            // await member.SendMessageAsync(message);
            await channel.SendMessageAsync(message);
        }
    }
}