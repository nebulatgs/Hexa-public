using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Hexa.Attributes;

namespace Hexa.Modules
{
    public class TicTacToeAlgorithm
    {
        public List<int> board;
        public bool check(List<int> grid = null, int player = 0)
        {
            if (grid is null) grid = board;
            Func<int, int, int, bool> row = (p1, p2, p3) => grid[p1] == player && grid[p2] == player && grid[p3] == player;

            return (
                // Rows
                row(0, 1, 2) ||
                row(3, 4, 5) ||
                row(6, 7, 8) ||
                // Columns
                row(0, 3, 6) ||
                row(1, 4, 7) ||
                row(2, 5, 8) ||
                // Diagonals
                row(0, 4, 8) ||
                row(2, 4, 6)
            );
        }
    }
    public class TicTacToeModule : BaseCommandModule
    {
        [Command("tictactoe")]
        [Category("Games")]
        public async Task TicTacToeCommand(CommandContext ctx)
        {
            DiscordButtonComponent[] button_row1 = new DiscordButtonComponent[3];
            DiscordButtonComponent[] button_row2 = new DiscordButtonComponent[3];
            DiscordButtonComponent[] button_row3 = new DiscordButtonComponent[3];
            List<DiscordButtonComponent> buttons = new List<DiscordButtonComponent>();
            List<int> values = new List<int>();
            for (var i = 0; i < 3; i++)
            {
                button_row1[i] = (new DiscordButtonComponent(ButtonStyle.Secondary, $"tictactoe_{i}", "\u200B ", false));
                button_row2[i] = (new DiscordButtonComponent(ButtonStyle.Secondary, $"tictactoe_{i + 3}", "\u200B ", false));
                button_row3[i] = (new DiscordButtonComponent(ButtonStyle.Secondary, $"tictactoe_{i + 6}", "\u200B ", false));
                values.Add(0);
                values.Add(0);
                values.Add(0);
            }
            var board = new TicTacToeAlgorithm();
            board.board = values;
            buttons = button_row1.Concat(button_row2).Concat(button_row3).ToList();

            // Init the message builder
            var builder = new DiscordMessageBuilder();
            var buttonBuilder = builder.WithComponents(button_row1).WithComponents(button_row2).WithComponents(button_row3);
            builder = buttonBuilder.WithContent("Play TicTacToe!");
            var message = await ctx.Channel.SendMessageAsync(builder);

            var interactivity = ctx.Client.GetInteractivity();
            var timeout = TimeSpan.FromSeconds(60);
            var loop_timeout = DateTime.Now + timeout;
            while (DateTime.Now < loop_timeout)
            {
                var result = await interactivity.WaitForButtonAsync(message, buttons, timeout);
                if (result.TimedOut)
                    continue;
                if (result.Result.User != ctx.Message.Author)
                    continue;

                var buttonInd = buttons.FindIndex(x => x.CustomId == result.Result.Id);
                var button = buttons.Where(x => x.CustomId == result.Result.Id).First();
                
                button.Disabled = true;
                button.Style = ButtonStyle.Primary;
                button.Label = "\u200A✕\u200A";
                values[buttonInd] = 1;
                
                if (buttons.Where(x => x.Label == "\u200B ").Count() <= 1)
                {
                    await message.ModifyAsync(builder);
                    continue;
                }

                var aiButton = new Random().Next(0, values.Count() - 1);
                while (values[aiButton] != 0)
                    aiButton = new Random().Next(0, values.Count() - 1);

                if (board.check(player: 1))
                    break;

                buttons.ElementAt(aiButton).Disabled = true;
                buttons.ElementAt(aiButton).Style = ButtonStyle.Danger;
                buttons.ElementAt(aiButton).Label = "\u200A◯\u200A";
                values[aiButton] = 2;
                await message.ModifyAsync(builder);
            }
            foreach (var button in buttons)
            {
                button.Disabled = true;
                // button.Style = ButtonStyle.Success;
            }
            await message.ModifyAsync(builder);
        }
    }
}