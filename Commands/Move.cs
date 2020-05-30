using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using sisbase.Attributes;
using sisbase.Utils;
using siscode_bot.Utils;

namespace siscode_bot.Commands {
    public class Move : BaseCommandModule {
        [Command("move"), Prefix(">>")]
        public async Task MoveCommand(CommandContext ctx, DiscordChannel dst, bool soft = false) {
            var messages = await ctx.Channel.GetMessagesAsync();
            var MessageIds = new List<ulong>();
            var currentUser = 0ul;
            int count = 1;
            var (start, end) = (messages.getMessage(">>move start"), messages.getMessage(">>move end"));
         
            while (start == default(DiscordMessage) && messages.Count != 0 && count < 6) {
                messages = await ctx.Channel.GetMessagesBeforeAsync(messages.LastOrDefault().Id);
                MessageIds.InsertRange(0,messages.Select(x => x.Id));
                end = messages.getMessage(">>move start");
                count++;
            }

            if (end == default(DiscordMessage)) {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed($"No starting message was found after searching ~{MessageIds.Count} messages. Exiting."));
                return;
            }
            await ctx.RespondAsync(embed: EmbedBase.OutputEmbed($"Found starting and ending messages. Transfer has begun, Please wait."));
            var MessageCount = MessageIds.Where(m => m >= start.Id && m <= end.Id).Count();
            var AllMessages = new List<DiscordMessage>();
            AllMessages.Add(start);
            if (MessageCount > 100) {
                AllMessages.AddRange(await ctx.Channel.GetMessagesAfterAsync(start.Id));
                MessageCount -= 100;
                while (MessageCount > 100) {
                    AllMessages.AddRange((await ctx.Channel.GetMessagesAfterAsync(AllMessages.Last().Id)).Reverse());
                    MessageCount -= 100;
                }
                AllMessages.AddRange((await ctx.Channel.GetMessagesAfterAsync(AllMessages.Last().Id,MessageCount-1)).Reverse());

            }
            else {
                AllMessages.AddRange((await ctx.Channel.GetMessagesAfterAsync(start.Id, MessageCount-1)).Reverse());
            }
            var embeds = await AllMessages.ToEmbeds(ctx);
            foreach (var embed in embeds) {
                await dst.SendMessageAsync(embed: embed);
            }

            if (!soft) {
                foreach (var msg in AllMessages) {
                    await msg.DeleteAsync();
                }
            }
            
        }
    }
    
}
    
