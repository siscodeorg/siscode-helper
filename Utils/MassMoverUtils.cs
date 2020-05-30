using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using sisbase.Utils;

namespace siscode_bot.Utils {
    public static class MassMoverUtils {
        public static DiscordMessage getMessage(this IEnumerable<DiscordMessage> data, string match) {
          data.ToList().Sort((x, y) => DateTime.Compare(x.Timestamp.DateTime, y.Timestamp.DateTime));
          return data.FirstOrDefault(x => x.Content.ToLowerInvariant().Contains(match));
        }
        static Regex movCh = new Regex(@">>move \<#\d+\>");

        internal static async Task<List<DiscordEmbed>> ToEmbeds(this IEnumerable<DiscordMessage> messages, CommandContext ctx) {
            if (messages == null) return default;
            var ret = new List<DiscordEmbed>();
            var conversations = new List<KeyValuePair<DiscordMember, List<DiscordMessage>>>();
            foreach (var message in messages) {
                if (conversations.Any() && conversations.Last().Key == message.Author) {
                    conversations.Last().Value.Add(message);
                    continue;
                }

                conversations.Add(
                    new KeyValuePair<DiscordMember, List<DiscordMessage>>(
                        await ctx.Guild.GetMemberAsync(message.Author.Id), new List<DiscordMessage>{
                            message
                        }));
            }
            foreach (var convo in conversations) {
                ret.AddRange(GenerateMessage(convo.Value,convo.Key));
            }
            return ret;
        }

        internal static List<DiscordEmbed> GenerateMessage(List<DiscordMessage> messages, DiscordMember member) {
            if (messages == null) return default;
            var ret = new List<DiscordEmbed>();
            var currEmbed = new DiscordEmbedBuilder().WithAuthor($"{(member.Nickname ?? member.Username)}",null, member.AvatarUrl).Build();
            if (member.IsBot) {
                foreach (var message in messages) {
                    currEmbed = message.Embeds[0].Mutate(x =>
                        x.WithAuthor(
                            $"{(member.Nickname ?? member.Username)}", 
                            null, member.AvatarUrl));
                    ret.Add(currEmbed);
                }

                return ret;
            }

            var data = messages.Select(m => SanitizeMessage(m.Content)).Where(x => x != string.Empty);
            currEmbed = currEmbed.Mutate(x => x.WithDescription(string.Join("\n", data)));
            if (messages.Any(x => x.Attachments.Any())) {
                currEmbed = currEmbed.Mutate(x => x.WithColor(DiscordColor.Violet).AddField("Attachments", string.Join("\n",
                    messages.SelectMany(m => m.Attachments.Select(xa => $"[{xa.FileName}]({xa.Url})")))));
            }
            ret.Add(currEmbed);
            return ret;
        }

        internal static string SanitizeMessage(string message) {
            if (string.IsNullOrWhiteSpace(message)) return string.Empty;
            if (message.Contains(">>move start"))
                message = message.Replace(">>move start", "").Trim();
            if (message.ToLowerInvariant().Trim() == ">>move end")
                message = string.Empty;
            if(movCh.IsMatch(message.ToLowerInvariant()))
                message = string.Empty;
            return message;
        }
    }
}