// using System.Collections.Generic;

// using DSharpPlus.CommandsNext;
// using DSharpPlus.CommandsNext.Converters;
// using DSharpPlus.CommandsNext.Entities;
// using DSharpPlus.Entities;

// using Hexa.Helpers;

// public class HexaHelpFormatter : BaseHelpFormatter
// {
//     protected DiscordEmbedBuilder _embed;
//     // protected StringBuilder _strBuilder;

//     public HexaHelpFormatter(CommandContext ctx) : base(ctx)
//     {
//         var hEmbed = new HexaEmbed(ctx, "hexa help");
//         _embed = hEmbed.embed;
//         // _strBuilder = new StringBuilder();

//         // Help formatters do support dependency injection.
//         // Any required services can be specified by declaring constructor parameters. 

//         // Other required initialization here ...
//     }

//     public override BaseHelpFormatter WithCommand(Command command)
//     {
//         _embed.AddField(command.Name, command.Description);            
//         // _strBuilder.AppendLine($"{command.Name} - {command.Description}");

//         return this;
//     }

//     public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> cmds)
//     {
//         foreach (var cmd in cmds)
//         {
//             _embed.AddField(cmd.Name, cmd.Description);            
//             // _strBuilder.AppendLine($"{cmd.Name} - {cmd.Description}");
//         }

//         return this;
//     }

//     public override CommandHelpMessage Build()
//     {
//         return new CommandHelpMessage(embed: _embed);
//         // return new CommandHelpMessage(content: _strBuilder.ToString());
//     }
// }
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;

using Hexa.Attributes;

namespace Hexa.Helpers
{
    /// <summary>
    /// Default CommandsNext help formatter.
    /// </summary>
    public class HexaHelpFormatter : BaseHelpFormatter
    {
        public DiscordEmbedBuilder EmbedBuilder { get; }
        private Command Command { get; set; }

        /// <summary>
        /// Creates a new default help formatter.
        /// </summary>
        /// <param name="ctx">Context in which this formatter is being invoked.</param>
        public HexaHelpFormatter(CommandContext ctx)
            : base(ctx)
        {
            var hEmbed = new HexaEmbed(ctx, "hexa help");
            this.EmbedBuilder = hEmbed.embed;
        }

        /// <summary>
        /// Sets the command this help message will be for.
        /// </summary>
        /// <param name="command">Command for which the help message is being produced.</param>
        /// <returns>This help formatter.</returns>
        public override BaseHelpFormatter WithCommand(Command command)
        {
            this.Command = command;
            var categories = command.CustomAttributes.Where(x => x.GetType() == typeof(CategoryAttribute)).Cast<CategoryAttribute>();
            if (categories.Count() > 0)
                this.EmbedBuilder.AddField("Category", categories.First().Description, true);
            else
                this.EmbedBuilder.AddField("Category", "Uncategorized", true);
            this.EmbedBuilder.AddField("Description", command.Description ?? "none", true);
            bool devonly = command.Module.ModuleType.CustomAttributes.Where(x => x.AttributeType == typeof(DevOnlyAttribute)).Count() > 0;
            bool adminonly = command.Module.ModuleType.CustomAttributes.Where(x => x.AttributeType == typeof(AdminOnlyAttribute)).Count() > 0;
            if (devonly && !adminonly)
                this.EmbedBuilder.AddField("User Permissions", "Hexa Developer", false);
            if (adminonly)
                this.EmbedBuilder.AddField("User Permissions", "Server Administrator", false);
            // this.EmbedBuilder.WithDescription($"Description: {command.Description ?? "No description provided."}");
            this.EmbedBuilder.Title = $"help for ``{command.Name}``";

            // if (command is CommandGroup cgroup && cgroup.IsExecutableWithoutSubcommands)
            // this.EmbedBuilder.WithDescription($"{this.EmbedBuilder.Description}\n\nThis group can be executed as a standalone command.");

            if (command.Overloads?.Any() == true)
            {
                var sb = new StringBuilder();

                foreach (var ovl in command.Overloads.OrderByDescending(x => x.Priority))
                {
                    sb.Append('`').Append(command.QualifiedName);

                    foreach (var arg in ovl.Arguments)
                        sb.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name).Append(arg.IsCatchAll ? "..." : "").Append(arg.IsOptional || arg.IsCatchAll ? ']' : '>');

                    sb.Append("`\n");

                    // foreach (var arg in ovl.Arguments)
                    // sb.Append('`').Append(arg.Name).Append(" (").Append(this.CommandsNext.GetUserFriendlyTypeName(arg.Type)).Append(")`: ").Append(arg.Description ?? "No description provided.").Append('\n');

                    // sb.Append('\n');
                }

                this.EmbedBuilder.AddField("Usage", sb.ToString().Trim(), false);
            }
            if (command.Aliases?.Any() == true)
                this.EmbedBuilder.AddField("Aliases", string.Join(", ", command.Aliases.Select(Formatter.InlineCode)), false);

            return this;
        }

        /// <summary>
        /// Sets the subcommands for this command, if applicable. This method will be called with filtered data.
        /// </summary>
        /// <param name="subcommands">Subcommands for this command group.</param>
        /// <returns>This help formatter.</returns>
        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            if (this.Command is null)
            {
                // this.EmbedBuilder.AddField("bot info", $"``{HexaSettings.GetValue(this.Context.Guild, HexaSettings.SettingType.ServerPrefix).GetAwaiter().GetResult()}``: current server prefix", false);
                var bot_info = new StringBuilder();
                var prefix = HexaSettings.GetValue(this.Context.Guild, HexaSettings.SettingType.ServerPrefix).GetAwaiter().GetResult() ?? "-";
                bot_info.Append($"``{prefix}``: current server prefix\n");
                bot_info.Append($"{this.Context.Client.CurrentUser.Mention}: mention me for help\n");
                this.EmbedBuilder.Title = "bot info";
                this.EmbedBuilder.Description = bot_info.ToString().Trim();
                this.EmbedBuilder.AddField("important commands", $"use ``/activity`` or ``{prefix}activity`` to start an activity in a voice channel\nuse ``/youtube`` to start Youtube Together in a voice channel", true);
                this.EmbedBuilder.AddField("bugs", $"report any bugs using the ``{prefix}bugreport`` command to help improve Hexa", false);
                var categories = subcommands.GroupBy(x => x.CustomAttributes.Where(y => y.GetType() == typeof(CategoryAttribute)).Cast<CategoryAttribute>().FirstOrDefault());
                foreach (var category in categories)
                {
                    // if (category.Count() == 0)
                        // continue;
                    var command_list = new StringBuilder();
                    int i = 1;
                    foreach (var command in category)
                    {
                        command_list.Append($"``{command.Name}`` ");
                        if(i % 3 == 0)
                            command_list.Append("\n");
                        i++;
                    }
                    this.EmbedBuilder.AddField(category.Key is not null ? category.Key.Description : "Uncategorized", command_list.ToString().Trim(), true);
                }
            }

            return this;
        }

        /// <summary>
        /// Construct the help message.
        /// </summary>
        /// <returns>Data for the help message.</returns>
        public override CommandHelpMessage Build()
        {
            // if (this.Command == null)
            // this.EmbedBuilder.WithDescription("Listing all top-level commands and groups. Specify a command to see more information.");

            return new CommandHelpMessage(embed: this.EmbedBuilder.Build());
        }
    }
}