using System.Threading.Tasks;
using System.Linq;
using DSharpPlus;

namespace Hexa.Modules
{
    public class GhostPingDetector
    {   
        public async Task OnDelete(DiscordClient client, DSharpPlus.EventArgs.MessageDeleteEventArgs args)
        {
            var setting = await HexaSettings.GetToggle(args.Guild, HexaSettings.SettingType.GhostPing);
            if(args.Message.MentionedUsers.Count() > 0 && bool.Parse(setting))
            {
                var validChannels = args.Message.Channel.Guild.GetChannelsAsync().Result.Where(x => x.Topic.ToString().ToLower().Contains("ghost ping"));
                foreach(var channel in validChannels)
                {
                    await channel.SendMessageAsync($"Ghost Ping in {args.Message.Channel} by {args.Message.Author}\nOriginal message was \\{args.Message.Content}");
                }
            }
        }
    }
}