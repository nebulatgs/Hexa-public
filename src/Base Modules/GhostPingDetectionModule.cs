using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using Hexa.Helpers;

namespace Hexa.Modules
{
    public class GhostPingDetector
    {   
        public SettingsManager Manager { get; set; }
        public GhostPingDetector(){Manager = new SettingsManager();}
        public async Task OnDelete(DiscordClient client, DSharpPlus.EventArgs.MessageDeleteEventArgs args)
        {
            if (args.Message.Author == client.CurrentUser)
                return;
            var setting = (await Manager.GetSetting(args.Guild, SettingsManager.HexaSetting.GhostPing)).Value ?? "false";
            if(args.Message.MentionedUsers.Count() > 0 && bool.Parse(setting))
            {
                var channels = (await args.Message.Channel.Guild.GetChannelsAsync());
                var validChannels = channels.Where(x => (x.Topic ?? "").ToString().ToLower().Contains("ghost ping"));
                foreach(var channel in validChannels)
                {
                    await channel.SendMessageAsync($"Ghost Ping in {args.Message.Channel} by {args.Message.Author}\nOriginal message was \\{args.Message.Content}");
                }
            }
        }
    }
}