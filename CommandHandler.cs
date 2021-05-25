using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;

namespace Hexa
{
    public class HexaCommandHandler
    {
        private string defaultPrefix { get; }
        public HexaCommandHandler(string default_prefix)
        {
            defaultPrefix = default_prefix;
        }
        public async Task CommandHandler(DiscordClient client, MessageCreateEventArgs e)
        {
            var cnext = client.GetCommandsNext();
            var msg = e.Message;
            string setPrefix = "";
            setPrefix = await HexaSettings.GetValue(e.Guild, HexaSettings.SettingType.ServerPrefix);
            // if (setPrefix == "")
            // setPrefix = defaultPrefix;
            // var cmdStart = msg.GetStringPrefixLength(setPrefix);
            string cmdString, args, prefix = "";
            Command command;
            if (msg.MentionedUsers.Contains(client.CurrentUser))
            {
                // cmdString = msg.Content.Replace($"<@!{client.CurrentUser.Id}>", "");
                cmdString = msg.Content.Remove(msg.Content.IndexOf($"<@!{client.CurrentUser.Id}>"), $"<@!{client.CurrentUser.Id}>".Length);
                prefix = client.CurrentUser.Mention;
                command = cnext.FindCommand(cmdString, out args);
                if (command == null) command = cnext.FindCommand("help", out args);
            }
            else
            {
                var cmdStart = msg.GetStringPrefixLength(setPrefix);
                if (cmdStart == -1) return;
                cmdString = msg.Content.Substring(cmdStart);
                prefix = setPrefix;
                command = cnext.FindCommand(cmdString, out args);
                if (command == null) return;
            }

            var ctx = cnext.CreateContext(msg, setPrefix, command, args);
            var help = cnext.CreateContext(msg, setPrefix, cnext.FindCommand($"help {command.Name}", out args), args);
            cnext.ExecuteCommandAsync(ctx);
            // Task.Run(async () =>
            // {
            //     try
            //     {
            //     }
            //     catch (Exception ex)
            //     {
            //         // switch (ex)
            //         // {
            //                 await cnext.ExecuteCommandAsync(help);
            //             // case ArgumentException:
            //                 // break;
            //         // }
            //     }
            // });

            return;
        }
    }
}