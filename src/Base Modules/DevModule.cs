using System;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Hexa.Attributes;
using DSharpPlus.Entities;
using Hexa.Helpers;

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
                                .AddReferences("System")
                                .AddReferences("System.Linq")
                                .AddReferences(typeof(System.Linq.Enumerable).Assembly);
                                // .AddReferences("Microsoft.CSharp")
            state = CSharpScript.Create("using System;", globalsType: typeof(CommandContext)).WithOptions(scriptOptions);
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
        
        [Command("leave")]
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

        [Command("dm")]
        [Description("DM a user")]
        [Category(SettingsManager.HexaSetting.DangerCategory)]
        public async Task DMCommand(CommandContext ctx, DiscordMember user, [RemainingText] string message)
        {
            await user.SendMessageAsync(message);
        }
    }
}