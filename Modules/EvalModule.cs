using System.Reflection;
using System;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus;
using Hexa.Attributes;

namespace Hexa.Modules
{
    [Hidden]
    [DevOnly]
    [HexaCooldown(5)]
    public class EvalModule : BaseCommandModule
    {
        private Microsoft.CodeAnalysis.Scripting.Script<object> state;
        public EvalModule()
        {
            state = CSharpScript.Create("using System;", globalsType: typeof(CommandContext));
        }
        [Command("evaluate")]
        [Aliases("eval", "ev")]
        [Description("Evaluate an expression")]
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
    }
}