using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Hexa.Database;

namespace Hexa.Helpers
{
    public class UsernameChangeLogger
    {
        private DiscordUser lastMember;
        public async Task OnChange(DiscordClient client, GuildMemberUpdateEventArgs args)
        {
            if ((DiscordUser)args.Member == lastMember)
                return;
            lastMember = args.Member;
            if(args.NicknameBefore != args.NicknameAfter)
                return;
            using (var db = new HexaContext())
            {
                db.Add(new PastUserState(){
                    UserId = args.Member.Id,
                    Username = args.Member.Username,
                    Discriminator = int.Parse(args.Member.Discriminator),
                    Flags = (int)args.Member.Flags,
                    AvatarUrl = args.Member.AvatarUrl,
                });
                await db.SaveChangesAsync();
            }
            if (args.NicknameBefore is null && args.NicknameAfter is null)
                return;
            await client.SendMessageAsync(await client.GetChannelAsync(849083307747704860), $"```yaml\nUSER UPDATE:\n{args.Guild}\nMember {args.Member.Id}; {args.Member.Username}#{args.Member.Discriminator}\n{args.NicknameBefore ?? "null"} ➜ {args.NicknameAfter ?? "null"}```");
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