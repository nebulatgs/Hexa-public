using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Hexa.Attributes;
using Hexa.Helpers;

namespace Hexa.Modules
{
    public class TicTacToeAlgorithm
    {
        public List<int> board;
        public int PossWin(List<int> grid = null, int player = 0)
        {
            if (grid is null) grid = board;
            Func<int, int, int, int> win = (p1, p2, p3) =>
                grid[p1] * grid[p2] * grid[p3] == player * player * 2 ? p1 != 2 ? p1 : p2 != 2 ? p2 : p3 != 2 ? p3 : -1 : -1;
            var selected = -1;
            // Rows
            if (win(0, 1, 2) != -1)
                selected = win(0, 1, 2);
            else if (win(3, 4, 5) != -1)
                selected = win(3, 4, 5);
            else if (win(6, 7, 8) != -1)
                selected = win(6, 7, 8);
            // Columns
            else if (win(0, 3, 6) != -1)
                selected = win(0, 3, 6);
            else if (win(1, 4, 7) != -1)
                selected = win(1, 4, 7);
            else if (win(2, 5, 8) != -1)
                selected = win(2, 5, 8);
            // Diagonals
            else if (win(0, 4, 8) != -1)
                selected = win(0, 4, 8);
            else if (win(2, 4, 6) != -1)
                selected = win(2, 4, 6);
            if (selected != -1)
                if (grid[selected] != 2)
                    selected = -1;
            return selected;
        }
        public Tuple<bool, int, int, int> check(List<int> grid = null, int player = 0)
        {
            if (grid is null) grid = board;
            Func<int, int, int, bool> row = (p1, p2, p3) => grid[p1] == player && grid[p2] == player && grid[p3] == player;

            // return (
            // Rows
            if (row(0, 1, 2))
                return Tuple.Create(true, 0, 1, 2);
            else if (row(3, 4, 5))
                return Tuple.Create(true, 3, 4, 5);
            else if (row(6, 7, 8))
                return Tuple.Create(true, 6, 7, 8);
            // Columns
            else if (row(0, 3, 6))
                return Tuple.Create(true, 0, 3, 6);
            else if (row(1, 4, 7))
                return Tuple.Create(true, 1, 4, 7);
            else if (row(2, 5, 8))
                return Tuple.Create(true, 2, 5, 8);
            // Diagonals
            else if (row(0, 4, 8))
                return Tuple.Create(true, 0, 4, 8);
            else if (row(2, 4, 6))
                return Tuple.Create(true, 2, 4, 6);
            else
                return Tuple.Create(false, 0, 0, 0);
            // );
        }
    }
    [HexaCooldown(5)]
    public class TicTacToeModule : BaseCommandModule
    {
        public Random Rand { get; set; }

        [Command("tictactoe")]
        public async Task TicTacToeCommand(CommandContext ctx, DiscordMember opponent)
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
                values.Add(2);
                values.Add(2);
                values.Add(2);
            }
            var board = new TicTacToeAlgorithm();
            board.board = values;
            buttons = button_row1.Concat(button_row2).Concat(button_row3).ToList();

            // Init the message builder
            var builder = new DiscordMessageBuilder();
            var buttonBuilder = builder.WithComponents(button_row1).WithComponents(button_row2).WithComponents(button_row3);
            builder = buttonBuilder.WithContent($"Play Tic Tac Toe against {opponent.DisplayName}!\n{(await ctx.Guild.GetMemberAsync(ctx.Message.Author.Id)).DisplayName}'s turn");
            // builder.Content
            var message = await ctx.Channel.SendMessageAsync(builder);

            var interactivity = ctx.Client.GetInteractivity();
            var timeout = TimeSpan.FromSeconds(60);
            var loop_timeout = DateTime.Now + timeout;
            var turn = 1;
            while (DateTime.Now < loop_timeout)
            {
                var result = await interactivity.WaitForButtonAsync(message, buttons, timeout);
                if (result.TimedOut)
                    continue;
                if (!(result.Result.User == ctx.Message.Author || result.Result.User == opponent))
                    continue;
                if (turn % 2 == 0)
                {
                    if (result.Result.User == ctx.Message.Author)
                    {
                        continue;
                    }
                    builder.Content = $"Play Tic Tac Toe against {opponent.DisplayName}!\n{(await ctx.Guild.GetMemberAsync(ctx.Message.Author.Id)).DisplayName}'s turn";
                }
                else
                {
                    if (result.Result.User == opponent)
                    {
                        continue;
                    }
                    builder.Content = $"Play Tic Tac Toe against {opponent.DisplayName}!\n{opponent.DisplayName}'s turn";
                }
                var buttonInd = buttons.FindIndex(x => x.CustomId == result.Result.Id);
                var button = buttons.Where(x => x.CustomId == result.Result.Id).First();
                var player = result.Result.User == ctx.Message.Author ? 5 : 4;
                button.Disabled = true;
                button.Style = ButtonStyle.Primary;
                button.Label = player == 5 ? "\u200A✕\u200A" : "\u200A◯\u200A";
                values[buttonInd] = player;
                turn++;
                await message.ModifyAsync(builder);
                var cells_X = board.check(player: 5);
                if (cells_X.Item1)
                {
                    builder.Content = $"Play Tic Tac Toe against {opponent.DisplayName}!\n{(await ctx.Guild.GetMemberAsync(ctx.Message.Author.Id)).DisplayName} wins!";
                    buttons.ElementAt(cells_X.Item2).Style = ButtonStyle.Success;
                    buttons.ElementAt(cells_X.Item3).Style = ButtonStyle.Success;
                    buttons.ElementAt(cells_X.Item4).Style = ButtonStyle.Success;
                    var losingSide = values.Select((val, index) => (val, index)).Where(x => x.val == 4);
                    foreach (var elem in losingSide)
                    {
                        buttons.ElementAt(elem.index).Style = ButtonStyle.Danger;
                    }
                    break;
                }

                var cells_O = board.check(player: 4);
                if (cells_O.Item1)
                {
                    builder.Content = $"Play Tic Tac Toe against {opponent.DisplayName}!\n{opponent.DisplayName} wins!";
                    buttons.ElementAt(cells_O.Item2).Style = ButtonStyle.Success;
                    buttons.ElementAt(cells_O.Item3).Style = ButtonStyle.Success;
                    buttons.ElementAt(cells_O.Item4).Style = ButtonStyle.Success;
                    var losingSide = values.Select((val, index) => (val, index)).Where(x => x.val == 4);
                    foreach (var elem in losingSide)
                    {
                        buttons.ElementAt(elem.index).Style = ButtonStyle.Danger;
                    }
                    break;
                }

                if (values.Where(x => x == 2).Count() == 0)
                {
                    builder.Content = $"Play Tic Tac Toe against {opponent.DisplayName}!\nIt's a tie!";
                    await message.ModifyAsync(builder);
                    break;
                }

                if (buttons.Where(x => x.Label == "\u200B ").Count() <= 1)
                {
                    await message.ModifyAsync(builder);
                    continue;
                }
                await message.ModifyAsync(builder);
            }
            foreach (var button in buttons)
            {
                button.Disabled = true;
            }
            try {await message.ModifyAsync(builder);}
            catch (DSharpPlus.Exceptions.NotFoundException) {}
        }

        [Command("tictactoe")]
        [Category(SettingsManager.HexaSetting.GamesCategory)]
        [Description("Play Tic Tac Toe against the bot or a human")]
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
                values.Add(2);
                values.Add(2);
                values.Add(2);
            }
            var board = new TicTacToeAlgorithm();
            board.board = values;
            buttons = button_row1.Concat(button_row2).Concat(button_row3).ToList();

            // Init the message builder
            var builder = new DiscordMessageBuilder();
            var buttonBuilder = builder.WithComponents(button_row1).WithComponents(button_row2).WithComponents(button_row3);
            builder = buttonBuilder.WithContent("Play Tic Tac Toe!");
            var message = await ctx.Channel.SendMessageAsync(builder);

            var interactivity = ctx.Client.GetInteractivity();
            var timeout = TimeSpan.FromSeconds(60);
            var loop_timeout = DateTime.Now + timeout;
            var turn = 1;
            Func<int> Make2 = () =>
            {
                if (values[4] == 2)
                    return 4;
                else
                {
                    var selected = 1;
                    var loop = 1;
                    while (values[selected] != 2 || selected != 1 || selected != 3 || selected != 5 || selected != 7)
                    {
                        selected = Rand.Next(0, values.Count() - 1);
                        loop++;
                        if (loop >= 9)
                            break;
                    }
                    return selected;
                }
            };
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
                values[buttonInd] = 5;

                var cells_X = board.check(player: 5);
                if (cells_X.Item1)
                {
                    buttons.ElementAt(cells_X.Item2).Style = ButtonStyle.Success;
                    buttons.ElementAt(cells_X.Item3).Style = ButtonStyle.Success;
                    buttons.ElementAt(cells_X.Item4).Style = ButtonStyle.Success;
                    var losingSide = values.Select((val, index) => (val, index)).Where(x => x.val == 4);
                    foreach (var elem in losingSide)
                    {
                        buttons.ElementAt(elem.index).Style = ButtonStyle.Danger;
                    }
                    break;
                }

                if (buttons.Where(x => x.Label == "\u200B ").Count() <= 1)
                {
                    await message.ModifyAsync(builder);
                    continue;
                }

                var checkBoard = new TicTacToeAlgorithm();
                checkBoard.board = new List<int>(values);

                var aiButton = 0;
                if (turn == 1)
                    if (values[4] == 2)
                        while (values.ElementAt(aiButton) != 2)
                            aiButton = Rand.Next(0, values.Count() - 1);
                    else
                        aiButton = 0;
                if (turn == 2)
                    if (board.PossWin(player: 5) != -1)
                        aiButton = board.PossWin(player: 5);
                    else
                        aiButton = Make2();
                if (turn == 3)
                    if (board.PossWin(player: 4) != -1)
                        aiButton = board.PossWin(player: 4);
                    else if (board.PossWin(player: 5) != -1)
                        aiButton = board.PossWin(player: 5);
                    else
                        while (values.ElementAt(aiButton) != 2)
                            aiButton = Make2();
                if (turn == 4)
                    if (board.PossWin(player: 4) != -1)
                        aiButton = board.PossWin(player: 4);
                    else if (board.PossWin(player: 5) != -1)
                        aiButton = board.PossWin(player: 5);
                    else
                        while (values.ElementAt(aiButton) != 2)
                            aiButton = Rand.Next(0, values.Count() - 1);

                buttons.ElementAt(aiButton).Disabled = true;
                buttons.ElementAt(aiButton).Label = "\u200A◯\u200A";
                values[aiButton] = 4;
                board.board = values;

                var cells_O = board.check(player: 4);
                if (cells_O.Item1)
                {
                    buttons.ElementAt(cells_O.Item2).Style = ButtonStyle.Success;
                    buttons.ElementAt(cells_O.Item3).Style = ButtonStyle.Success;
                    buttons.ElementAt(cells_O.Item4).Style = ButtonStyle.Success;
                    var losingSide = values.Select((val, index) => (val, index)).Where(x => x.val == 5);
                    foreach (var elem in losingSide)
                    {
                        buttons.ElementAt(elem.index).Style = ButtonStyle.Danger;
                    }
                    break;
                }
                await message.ModifyAsync(builder);
                turn++;
            }
            foreach (var button in buttons)
            {
                button.Disabled = true;
            }
            await message.ModifyAsync(builder);
        }
    }
}