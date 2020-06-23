using DSharpPlus;
using DSharpPlus.EventArgs;
using LiteDB;
using sisbase;
using sisbase.Utils;
using siscode_bot.Data.ReactRole;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace siscode_bot.Systems {
    public class ReactRole : IClientSystem, ISchedule {
        public string Name {get;set;}
        public string Description {get;set;}
        public bool Status {get;set;}
        public LiteDatabase Database { get; set; }

        public TimeSpan Timeout { get; internal set; }

        internal ILiteCollection<Category> categoryCache;
        public void Activate() {
            Database = new LiteDatabase(@$"{Directory.GetCurrentDirectory()}/ReactRole.db");
            Name = "ReactRole";
            Status = true;
            Timeout = TimeSpan.FromMinutes(10);
        }
        public async Task ApplyToClient(DiscordClient client) {
            client.MessageReactionAdded += ReactionAdded;
            client.MessageReactionRemoved += ReactionRemoved;
        }

        private async Task ReactionRemoved(MessageReactionRemoveEventArgs e) {
            if (e.Guild == null) return;
            if (e.User.IsBot) return;
            var categories = categoryCache.Query().Where(x => x.ChannelId == e.Channel.Id).ToList();
            var category = categories.FirstOrDefault(x => x.MessageId == e.Message.Id);
            if (!categories.Any() || category == null)
                return;
            if (!category.Data.ContainsKey(e.Emoji.Id)) {
                await e.Message.DeleteReactionAsync(e.Emoji, e.User);
                return;
            }
            var member = await e.Guild.GetMemberAsync(e.User.Id);
            await member.RevokeRoleAsync(e.Guild.GetRole(category.Data[e.Emoji.Id]));
        }
        private async Task ReactionAdded(MessageReactionAddEventArgs e) {
            if (e.Guild == null) return;
            if (e.User.IsBot) return;
            var categories = categoryCache.Query().Where(x => x.ChannelId == e.Channel.Id).ToList();
            var category = categories.FirstOrDefault(x => x.MessageId == e.Message.Id);
            if (!categories.Any() || category == null)
                return;
            if (!category.Data.ContainsKey(e.Emoji.Id)) {
                await e.Message.DeleteReactionAsync(e.Emoji, e.User);
                return;
            }
            var member = await e.Guild.GetMemberAsync(e.User.Id);
            await member.GrantRoleAsync(e.Guild.GetRole(category.Data[e.Emoji.Id]));
        }

        public void Deactivate() {
            SisbaseBot.Instance.Client.MessageReactionRemoved -= ReactionRemoved;
            SisbaseBot.Instance.Client.MessageReactionAdded -= ReactionAdded;
            Database.Dispose();
            categoryCache = null;
            Name = null;
            Status = false;
            Timeout = TimeSpan.Zero;
        }
        public void Execute() {
            categoryCache = Database.GetCollection<Category>("category");
        }
        public Action RunContinuous => () => {
            Execute();
        };
    }
}
