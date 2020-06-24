using System;
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
            var _fetch = await ctx.Channel.GetMessagesAsync();
            var Messages = new List<DiscordMessage>(_fetch);
            var currentUser = 0ul;
            int count = 1;
            var (start, end) = (_fetch.getMessage(">>move start"), _fetch.getMessage(">>move end"))
            while (start == default(DiscordMessage) && _fetch.Count != 0 && count < 6) {
                _fetch = await ctx.Channel.GetMessagesBeforeAsync(_fetch.LastOrDefault().Id);
                Messages.AddRange(_fetch);
                start = _fetch.getMessage(">>move start");
                count++;
            }

            if (start == default(DiscordMessage)) {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed($"No starting message was found after searching ~{Messages.Count} messages. Exiting."));
                return;
            }
            await ctx.RespondAsync(embed: EmbedBase.OutputEmbed($"Found starting and ending messages. Transfer has begun, Please wait."));
            var AllMessages = Messages.Where(m => m.Id >= start.Id && m.Id <= end.Id).ToList();
            AllMessages.Reverse();
            var embeds = await AllMessages.ToEmbeds(ctx);
            foreach (var embed in embeds) {
                await dst.SendMessageAsync(embed: embed);
            }

            if (!soft) {
                foreach (var msg in AllMessages) {
                    if (msg.Attachments.Any()) continue;
                    await msg.DeleteAsync();
                }
            }

        }
    }
}

