using System.Collections.Generic;
using DSharpPlus.EventArgs;
using DSharpPlus;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace Hexa.Helpers {
    public class SnipeHelper {
        private Dictionary<DiscordChannel, DiscordMessage> Snipes = new();
        public async Task MessageDeleted(DiscordClient _, MessageDeleteEventArgs args){
            if (Snipes.ContainsKey(args.Channel))
                Snipes[args.Channel] = args.Message;
            else
                Snipes.TryAdd(args.Channel, args.Message);
        }
        public DiscordMessage GetSnipe(DiscordChannel channel) {
            return Snipes.ContainsKey(channel) ? Snipes[channel] : null;
        }
    }
}