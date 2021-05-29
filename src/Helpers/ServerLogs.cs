using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Hexa.Helpers
{
    public class UsernameChangeLogger
    {
        public async Task OnChange(DiscordClient client, GuildMemberUpdateEventArgs args)
        {
            if (args.NicknameBefore is null && args.NicknameAfter is null)
                return;
            await client.SendMessageAsync(await client.GetChannelAsync(847657394343510056), $"```yaml\nNICKNAME CHANGE:\n{args.Guild}\nMember {args.Member.Id}; {args.Member.Username}#{args.Member.Discriminator}\n{args.NicknameBefore ?? "null"} ➜ {args.NicknameAfter ?? "null"}```");
        }
    }

    public class JoinLeaveLogger
    {
        public async Task OnChange(DiscordClient client, GuildCreateEventArgs args)
        {
            await client.SendMessageAsync(await client.GetChannelAsync(847649085237755954), $"JOINED GUILD: ``{args.Guild.ToString() ?? "null"}``");
        }
        public async Task OnChange(DiscordClient client, GuildDeleteEventArgs args)
        {
            await client.SendMessageAsync(await client.GetChannelAsync(847649085237755954), $"LEFT GUILD: ``{args.Guild.ToString() ?? "null"}``");
        }
    }

    // public class 
}