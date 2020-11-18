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
            if(dst == ctx.Channel) {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("what are you? a FOOL? you really called me to move things to the same place?"));
                return;
            }

            var (startMsg,endMsg) = (await GetMessage(ctx, ">>move start"), await GetMessage(ctx,">>move end"));
            var err = GetErrorMessage(startMsg, endMsg);

            if(!string.IsNullOrWhiteSpace(err)) {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed($"The following messages could not be found: \n{err}"));
                return;
            }

            await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Found both starting and ending messages."));

            var messagesBetween = await GetMessagesBetween(ctx, startMsg.Id, endMsg.Id);

            await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Got all messages. Moving will begin shortly."));

            var allMessages = new List<DiscordMessage>();
            allMessages.Add(startMsg);
            allMessages.AddRange(messagesBetween);
            allMessages.Add(endMsg);
            var participants = allMessages.Select(x => $"- {x.Author.Username}").Distinct().ToList();

            var embeds = await allMessages.ToEmbeds(ctx);
            foreach (var embed in embeds) {
                await dst.SendMessageAsync(embed: embed);
                await Task.Delay(1000);
            }

            var stats = EmbedBase.OutputEmbed("Finished moving messages")
                .Mutate(x => x
                .AddField("Total Message Count:", $"{allMessages.Count()} messages", true)
                .AddField("Participants", $"{string.Join("\n", participants)}", true)
            );

            if (!soft) {
                var msgs = allMessages.Where(x => !x.Attachments.Any());
                await ctx.Channel.DeleteMessagesAsync(msgs);
                stats = stats.Mutate(x => x.AddField("Deleted", $"{msgs.Count()} messages",true));
            }
            await ctx.RespondAsync(embed: stats);
        }

        internal async Task<List<DiscordMessage>> GetMessagesBetween (CommandContext ctx, ulong start, ulong end) {
            var init = (await ctx.Channel.GetMessagesBeforeAsync(end, 100)).ToList();

            while(init.FirstOrDefault(x => x.Id == start) == null) {
                init.AddRange(await ctx.Channel.GetMessagesBeforeAsync(init.Last().Id, 100));
            }

            return init.Where(x => x.Id > start).Reverse().ToList();
        }

#nullable enable
        internal async Task<DiscordMessage?> GetMessage(CommandContext ctx, string query,ulong? fromId = null) {
            IReadOnlyList<DiscordMessage> messages;

            if (fromId != null) messages = await ctx.Channel.GetMessagesBeforeAsync(fromId.Value, 100);
            else messages = await ctx.Channel.GetMessagesAsync(100);

            if (!messages.Any()) return null;

            if (messages.Any(x => x.Content.ToLowerInvariant().Contains(query.ToLowerInvariant())))
                return messages.First(x => x.Content.ToLowerInvariant().Contains(query.ToLowerInvariant()));
            else return await GetMessage(ctx, query, messages.Last().Id);
        }
        internal string GetErrorMessage(DiscordMessage? start, DiscordMessage? end) {
            var str = "";
            if(start == null) str += "STARTING\n";
            if (end == null) str += "ENDING\n";
            return str;
        }
    }
}

