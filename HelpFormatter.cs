using System.Collections.Generic;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using Hexa.Modules;

public class HexaHelpFormatter : BaseHelpFormatter
{
    protected DiscordEmbedBuilder _embed;
    // protected StringBuilder _strBuilder;

    public HexaHelpFormatter(CommandContext ctx) : base(ctx)
    {
        var hEmbed = new HexaEmbed(ctx, "hexa help");
        _embed = hEmbed.embed;
        // _strBuilder = new StringBuilder();

        // Help formatters do support dependency injection.
        // Any required services can be specified by declaring constructor parameters. 

        // Other required initialization here ...
    }

    public override BaseHelpFormatter WithCommand(Command command)
    {
        _embed.AddField(command.Name, command.Description);            
        // _strBuilder.AppendLine($"{command.Name} - {command.Description}");

        return this;
    }

    public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> cmds)
    {
        foreach (var cmd in cmds)
        {
            _embed.AddField(cmd.Name, cmd.Description);            
            // _strBuilder.AppendLine($"{cmd.Name} - {cmd.Description}");
        }

        return this;
    }

    public override CommandHelpMessage Build()
    {
        return new CommandHelpMessage(embed: _embed);
        // return new CommandHelpMessage(content: _strBuilder.ToString());
    }
}