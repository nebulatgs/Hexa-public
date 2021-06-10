using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;

namespace Hexa.Helpers
{
    public static class MessageExtensions
    {
        public static async Task<DiscordMessage> SafeModifyAsync(this DiscordMessage message, Optional<DiscordEmbed> embed = default)
        {
            try
            {
                var editedMsg = await message.ModifyAsync(embed);
                return editedMsg;
            }
            catch (NotFoundException) 
            {
                await Program.Logger.LogException("SafeModifyAsync", "Attemped to modify a deleted message");
                return null;
            }
        }
    }
}