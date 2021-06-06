using System;
using System.Linq;
using System.Text;
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
        private DateTime lastTime;
        public async Task OnChange(DiscordClient client, GuildMemberUpdateEventArgs args)
        {
            if ((DiscordUser)args.Member == lastMember && lastTime.AddSeconds(1) > DateTime.Now)
                return;
            lastMember = args.Member;
            lastTime = DateTime.Now;
            if (args.NicknameBefore != args.NicknameAfter)
                return;
            using (var db = new HexaContext())
            {
                var state = new PastUserState()
                {
                    UserId = args.Member.Id,
                    Username = args.Member.Username,
                    Discriminator = int.Parse(args.Member.Discriminator),
                    Flags = (int)(args.Member.Flags ?? 0),
                    AvatarUrl = args.Member.AvatarUrl,//.Split('/')[5].Split('?')[0].Split('.')[0],
                    IsBot = args.Member.IsBot
                };
                if (db.PastUserStates.OrderBy(x => x.PastUserStateId).LastOrDefault().GetIdentifier() == state.GetIdentifier())
                    return;
                StringBuilder logString = new StringBuilder($"```yaml\nUSER UPDATE: ");
                if (db.PastUserStates.Where(x => x.UserId == args.Member.Id).OrderBy(x => x.PastUserStateId).Count() > 0)
                {
                    if (db.PastUserStates.Where(x => x.UserId == args.Member.Id).OrderBy(x => x.PastUserStateId).LastOrDefault().Username == args.Member.Username)
                    {
                        logString.Append("PROFILE CHANGE\n");
                        logString.AppendLine($"{args.Guild}\nMember {args.Member.Id}; {args.Member.Username}#{args.Member.Discriminator}```");
                    }
                    else
                    {
                        logString.Append("NAME CHANGE\n");
                        logString.AppendLine($"{args.Guild}\nMember {args.Member.Id}; {args.Member.Username}#{args.Member.Discriminator}");
                        logString.AppendLine($"\n{db.PastUserStates.Where(x => x.UserId == args.Member.Id).OrderBy(x => x.PastUserStateId).LastOrDefault().Username ?? "null"} ➜ {args.Member.Username ?? "null"}```");
                    }
                    await client.SendMessageAsync(await client.GetChannelAsync(849083307747704860), logString.ToString());
                }
                db.Add(state);
                await db.SaveChangesAsync();
            }
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
}