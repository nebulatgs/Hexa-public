using System;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;

public class HexaLogger
{
    private string file_name;
    public string LogFile { get{return file_name;} }
    public HexaLogger(string log_file_name) { file_name = log_file_name; }
    public async Task LogCommandExecution(CommandsNextExtension command_ext, CommandExecutionEventArgs args)
    {
        string logString = $"Executed {args.Command} : {args.Context.Guild}, {args.Context.Channel};\nby {args.Context.Message.Author} with arguments \"{args.Context.RawArgumentString}\"\n"
                            .Replace("   ", "\t");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\x1b[32m{logString}");
        Console.ResetColor();
        using StreamWriter file = File.AppendText(file_name);
        await file.WriteLineAsync(logString);
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
    }

    public async Task LogInfo(CommandsNextExtension command_ext, CommandExecutionEventArgs args)
    {
        string logString = $"{args.Command} executed in {args.Context.Guild}, {args.Context.Channel} by {args.Context.Message.Author} with arguments \"{args.Context.RawArgumentString}\"\n";
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\x1b[33m{logString}");
        Console.ResetColor();
        using StreamWriter file = File.AppendText(file_name);
        await file.WriteLineAsync(logString);
    }
}