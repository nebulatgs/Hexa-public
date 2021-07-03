using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        public SettingsManager Manager { get; set; }
        private Command Command { get; set; }

        /// <summary>
        /// Creates a new default help formatter.
        /// </summary>
        /// <param name="ctx">Context in which this formatter is being invoked.</param>
        public HexaHelpFormatter(CommandContext ctx)
            : base(ctx)
        {
            var hEmbed = new HexaEmbed(ctx, "hexa help");
            EmbedBuilder = hEmbed.embed;
            Manager = new SettingsManager();
        }

        /// <summary>
        /// Sets the command this help message will be for.
        /// </summary>
        /// <param name="command">Command for which the help message is being produced.</param>
        /// <returns>This help formatter.</returns>
        public override BaseHelpFormatter WithCommand(Command command)
        {
            Command = command;
            var categories = command.ExecutionChecks.Where(x => x.GetType() == typeof(CategoryAttribute)).Cast<CategoryAttribute>();
            if (categories.Any())
                EmbedBuilder.AddField("Category", categories.First().Name, true);
            else
                EmbedBuilder.AddField("Category", "Uncategorized", true);
            EmbedBuilder.AddField("Description", command.Description ?? "none", true);
            bool devonly = command.Module.ModuleType.CustomAttributes.Any(x => x.AttributeType == typeof(DevOnlyAttribute));
            bool adminonly = command.Module.ModuleType.CustomAttributes.Any(x => x.AttributeType == typeof(AdminOnlyAttribute));
            if (devonly && !adminonly)
                EmbedBuilder.AddField("User Permissions", "Hexa Developer", false);
            if (adminonly)
                EmbedBuilder.AddField("User Permissions", "Server Administrator", false);
            EmbedBuilder.Title = $"help for ``{command.Name}``";

                var methods = command.Module.ModuleType.GetMethods().Where(m => m.ReturnType == typeof(System.Threading.Tasks.Task) && m.CustomAttributes.Any(x => x.AttributeType == typeof(DSharpPlus.CommandsNext.Attributes.CommandAttribute)));
                var thing = methods.Where(m => m.GetCustomAttributes(typeof(HelpHideAttribute), true).Any());
                var types = thing.Select(x => x.GetParameters().Select(y => y.ParameterType).Skip(1));
                if (command.Overloads?.Any() == true)
                {
                    var sb = new StringBuilder();

                    foreach (var ovl in command.Overloads.OrderByDescending(x => x.Priority))
                    {
                        var ovlTypes = ovl.Arguments.Select(x => x.Type);
                        var ovlSelect = ovlTypes.Select(y => y.FullName).OrderBy(x => x);
                        if(types.Any(x => !x.Select(y => y.FullName).OrderBy(x => x).Except(ovlSelect).Any() && !ovlSelect.Except(x.Select(y => y.FullName).OrderBy(x => x)).Any()))
                            continue;
                        
                        sb.Append('`').Append(command.QualifiedName);

                        foreach (var arg in ovl.Arguments)
                            sb.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name).Append(arg.IsCatchAll ? "..." : "").Append(arg.IsOptional || arg.IsCatchAll ? ']' : '>');

                        sb.Append("`\n");

                    }

                    EmbedBuilder.AddField("Usage", sb.ToString().Trim(), false);
                }

            if (command.Aliases?.Any() == true)
                EmbedBuilder.AddField("Aliases", string.Join(", ", command.Aliases.Select(Formatter.InlineCode)), false);

            return this;
        }

        /// <summary>
        /// Sets the subcommands for this command, if applicable. This method will be called with filtered data.
        /// </summary>
        /// <param name="subcommands">Subcommands for this command group.</param>
        /// <returns>This help formatter.</returns>
        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            if (Command is null)
            {
                var bot_info = new StringBuilder();
                var prefix = Environment.GetEnvironmentVariable("PROD") is not null ? Manager.GetSetting(Context.Guild, SettingsManager.HexaSetting.ServerPrefix).GetAwaiter().GetResult().Value ?? "-" : "+";
                prefix = Regex.Replace(prefix, @"[\\\*\~_>`]", (Match m) => $"{m.Value}\u200B");
                bot_info.Append($"``{prefix}``: current server prefix\n");
                bot_info.Append($"{Context.Client.CurrentUser.Mention}: mention me for help\n");
                EmbedBuilder.Title = "bot info";
                EmbedBuilder.Description = bot_info.ToString().Trim();
                EmbedBuilder.AddField("important commands", $"use ``/activity`` or ``{prefix}activity`` to start an activity in a voice channel\nuse ``/youtube`` to start Youtube Together in a voice channel", true);
                EmbedBuilder.AddField("bugs", $"report any bugs using the ``{prefix}bugreport`` command to help improve Hexa", false);
                var categories = subcommands.GroupBy(x => x.ExecutionChecks.Where(y => y.GetType() == typeof(CategoryAttribute)).Cast<CategoryAttribute>().Select(x => x.Name).FirstOrDefault());
                foreach (var category in categories)
                {
                    var command_list = new StringBuilder();
                    int i = 1;
                    foreach (var command in category)
                    {
                        command_list.Append($"``{command.Name}`` ");
                        if(i % 3 == 0)
                            command_list.Append('\n');
                        i++;
                    }
                    EmbedBuilder.AddField(category.Key is not null ? category.Key : "Uncategorized", command_list.ToString().Trim(), true);
                }
            }

            return this;
        }

        /// <summary>
        /// Construct the help message.
        /// </summary>
        /// <returns>Data for the help message.</returns>
        public override CommandHelpMessage Build() =>
            new(embed: EmbedBuilder.Build());
    }
}