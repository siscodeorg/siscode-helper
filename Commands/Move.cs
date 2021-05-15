using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Webhook;
using sisbase.CommandsNext;
using sisbase.Streams;
using siscode_helper.Utils;

namespace siscode_helper.Commands {
    public class Move : ModuleBase<SisbaseCommandContext> {
        [Command("move"), RequireUserPermission(GuildPermission.Administrator)]
        public async Task MoveCommand(IMessageChannel channel, params ulong[] ids) {
            var pairs = pair(ids.ToList());
            
            if (!(channel is ITextChannel toTextChannel))  return;
            var fromMessageChannel = Context.Channel as ITextChannel;
            
            var webhook = await toTextChannel.CreateWebhookAsync("MessageBoi");
            DiscordWebhookClient webhookClient = new(webhook);
            
            foreach (var (start,end) in pairs) {
                var messages = await fromMessageChannel.StreamAllMessagesAsync()
                        .TakeWhile(m => m.Id >= start)
                        .SkipWhile(m => m.Id > end)
                        .ToListAsync();

                var blocks = messageBlock(messages).Select(a => {
                    var (user, arr) = a;
                    arr.Reverse();
                    return (user, arr);
                }).ToList();
                
                blocks.Reverse();
                
                var users = blocks.Select(x => x.Item1).Distinct();
                var guildUsers = await Task.WhenAll(users.Select(x => Context.Guild.GetUserAsync(x.Id)));
                var members = guildUsers.Select(x => new KeyValuePair<ulong, IGuildUser>(x.Id, x)).ToDictionary(x => x.Key, x => x.Value);
                
                foreach (var block in blocks) {
                    await SendBlockAsync(block, members, webhookClient);
                }
                await webhook.DeleteAsync();
                await toTextChannel.DeleteMessagesAsync(messages);
            }
        }


        async Task SendBlockAsync((IUser, List<IMessage>) block, Dictionary<ulong, IGuildUser> members, DiscordWebhookClient client) {
            var compoundMsg = "";
            var (user, messages) = block;
            foreach (var msg in messages) {
                compoundMsg += $"{msg.Content}\n";
                if (!msg.Attachments.Any()) continue;

                var attachments = await Task.WhenAll(msg.Attachments.Select(x =>
                    new WebClient().DownloadOnlineResourceAsync(x.Url, FileUtils.GetTempName(x.Filename))));
                
                if (!string.IsNullOrWhiteSpace(compoundMsg)) {
                    await client.SendMessageAsync(compoundMsg, avatarUrl: user.GetAvatarUrl(), username: members[user.Id].Nickname ?? user.Username);
                }

                foreach (var attachment in attachments) {
                    await client.SendFileAsync(attachment, "", avatarUrl: user.GetAvatarUrl(), username: members[user.Id].Nickname ?? user.Username);
                    File.Delete(attachment);
                }

                compoundMsg = "";
            }

            if (!string.IsNullOrWhiteSpace(compoundMsg)) {
                await client.SendMessageAsync(compoundMsg, avatarUrl: user.GetAvatarUrl(), username: members[user.Id].Nickname ?? user.Username);
            }
        }
        
        IEnumerable<(ulong, ulong)> pair(List<ulong> ids) {
            List<(ulong, ulong)> pairs = new();
            
            if (!ids.Any()) return new List<(ulong, ulong)>();
            
            if (ids.Count == 1) return new List<(ulong, ulong)>();
            
            var sortedIds = new List<ulong>{ids[0], ids[1]};
            sortedIds.Sort();
            
            pairs.Add((sortedIds[0],sortedIds[1]));
            
            var xs = ids.ToArray()[2..];
            pairs.AddRange(pair(xs.ToList()));
            
            return pairs;
        }
        
        List<(IUser, List<IMessage>)> messageBlock(List<IMessage> messages) {
            List<(IUser, List<IMessage>)> blocks = new();
            
            if (!messages.Any()) return new List<(IUser, List<IMessage>)>();
            
            var xs = messages.TakeWhile(x => x.Author == messages[0].Author);
            var ys = messages.SkipWhile(x => xs.Contains(x));
            
            blocks.Add((messages[0].Author,xs.ToList()));
            blocks.AddRange(messageBlock(ys.ToList()));
            return blocks;
        }
    }
}