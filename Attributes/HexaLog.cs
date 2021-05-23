using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;

public class HexaLogger
{   
    private string file_name;
    public HexaLogger(string log_file_name) { file_name = log_file_name; }
    public async Task Log(CommandsNextExtension command_ext, CommandExecutionEventArgs args)
    {
        Console.WriteLine($"{args.Command} executed in {args.Context.Guild}, {args.Context.Channel} by {args.Context.Message.Author} with arguments \"{args.Context.RawArgumentString}\"");
    }
}