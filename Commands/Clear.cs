using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using sisbase.CommandsNext;
using sisbase.Streams;

namespace siscode_helper.Commands {
    public class Clear : ModuleBase<SisbaseCommandContext> {
        [Command("clear"), 
         RequireUserPermission(GuildPermission.ManageMessages), 
         RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task ClearCommand([Summary("ID of the starting message")] ulong startId,
            [Summary("ID of the ending message")] ulong endId) {
            var channel =  Context.Channel as SocketTextChannel;
            var messagesToBeDeleted = await channel.StreamAllMessagesAsync()
                .TakeWhile(x => x.Id >= startId)
                .SkipWhile(x => x.Id > endId).ToListAsync();

            await ReplyAsync($"Found {messagesToBeDeleted.Count} messages to be deleted." +
                       $" Using batch deletion. This may take a while.");

            if (channel != null) await channel.DeleteMessagesAsync(messagesToBeDeleted);

            await ReplyAsync($"Batch deletion endpoint called. {messagesToBeDeleted.Count}" +
                             $" messages will be deleted once discord decides to. \n" +
                             $"Information about the messages:");

            await ReplyAsync(embed: GetInfoEmbed(messagesToBeDeleted.Last(), "First Message"));
            await ReplyAsync(embed: GetInfoEmbed(messagesToBeDeleted.First(), "Last Message"));
        }

        internal Embed GetInfoEmbed(IMessage message, string details) {
            EmbedBuilder builder = new();
            builder
                .WithDescription($"Author : {message.Author.Mention}\n" +
                                 $"Content : {message.Content}\n" +
                                 $"Source : {message.Source}")
                .WithFooter($"Timestamp : {message.Timestamp}")
                .WithAuthor($"Message Details : {details}");

            return builder.Build();
        }
        
    }
}