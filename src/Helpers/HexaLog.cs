using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;

public class HexaLogger
{
    private string file_name;
    // private readonly DiscordChannel logChannel;
    public string LogFile { get{return file_name;} }
    public HexaLogger(string log_file_name) { file_name = log_file_name;}
    public async Task LogCommandExecution(CommandsNextExtension command_ext, CommandExecutionEventArgs args)
    {
        string logString = $"Executed {args.Command} : {args.Context.Guild}, {args.Context.Channel};\nby {args.Context.Message.Author} with arguments \"{args.Context.RawArgumentString}\"\n"
                            .Replace("   ", "\t");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\x1b[32m{logString}");
        Console.ResetColor();
        using StreamWriter file = File.AppendText(file_name);
        await file.WriteLineAsync(logString);
        var logChannel = await args.Context.Client.GetChannelAsync(849357173775007804);
        await logChannel.SendMessageAsync($"```diff\n+ {logString}```");
    }

    public async Task LogSlashCommandExecution(SlashCommandsExtension command_ext, SlashCommandExecutedEventArgs args)
    {
        string logString = $"Executed Slash Command: /{args.Context.CommandName} : {args.Context.Guild}, {args.Context.Channel};\nby {args.Context.User} with arguments \"{string.Join(", ", args.Context.Interaction.Data.Options.Select(x => x.Value.ToString()))}\"\n"
                            .Replace("   ", "\t");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\x1b[32m{logString}");
        Console.ResetColor();
        using StreamWriter file = File.AppendText(file_name);
        await file.WriteLineAsync(logString);
        var logChannel = await args.Context.Client.GetChannelAsync(849357173775007804);
        await logChannel.SendMessageAsync($"```diff\n+ {logString}```");
    }

    public async Task LogCommandError(CommandsNextExtension command_ext, CommandErrorEventArgs args)
    {

        string logString = $"Error in {args.Command} : {args.Context.Guild}, {args.Context.Channel} with exception \n\t\'{args.Exception}\'\nby {args.Context.Message.Author} with arguments \"{args.Context.RawArgumentString}\"\n"
                            .Replace("   ", "\t");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\x1b[31m{logString}");
        Console.ResetColor();
        using StreamWriter file = File.AppendText(file_name);
        await file.WriteLineAsync(logString);
        var logChannel = await args.Context.Client.GetChannelAsync(849357173775007804);
        await logChannel.SendMessageAsync($"```diff\n- {logString}```");
    }

    public async Task LogSlashCommandError(SlashCommandsExtension command_ext, SlashCommandErrorEventArgs args)
    {

        string logString = $"Error in Slash Command: /{args.Context.CommandName} : {args.Context.Guild}, {args.Context.Channel} with exception \n\t\'{args.Exception}\'\nby {args.Context.User} with arguments \"{string.Join(", ", args.Context.Interaction.Data.Options.Select(x => x.Value.ToString()))}\"\n"
                            .Replace("   ", "\t");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\x1b[31m{logString}");
        Console.ResetColor();
        using StreamWriter file = File.AppendText(file_name);
        await file.WriteLineAsync(logString);
        var logChannel = await args.Context.Client.GetChannelAsync(849357173775007804);
        await logChannel.SendMessageAsync($"```diff\n- {logString}```");
    }
    public async Task LogInfo(CommandsNextExtension command_ext, CommandExecutionEventArgs args)
    {
        string logString = $"{args.Command} executed in {args.Context.Guild}, {args.Context.Channel} by {args.Context.Message.Author} with arguments \"{args.Context.RawArgumentString}\"\n";
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\x1b[33m{logString}");
        Console.ResetColor();
        using StreamWriter file = File.AppendText(file_name);
        await file.WriteLineAsync(logString);
        var logChannel = await args.Context.Client.GetChannelAsync(849357173775007804);
        await logChannel.SendMessageAsync($"```yaml\n{logString}```");
    }

    public async Task LogDm(DiscordClient client, MessageCreateEventArgs args)
    {
        if (args.Channel.Type != ChannelType.Private || args.Author == client.CurrentUser)
            return;
        string logString = $"Received DM by {args.Message.Author} with content \"{args.Message}\"\n";
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\x1b[33m{logString}");
        Console.ResetColor();
        using StreamWriter file = File.AppendText(file_name);
        await file.WriteLineAsync(logString);
        var logChannel = await client.GetChannelAsync(849357173775007804);
        await logChannel.SendMessageAsync($"```yaml\n{logString}```");
    }
}