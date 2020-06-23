using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using sisbase.Attributes;
using sisbase.Utils;
using siscode_bot.Data.ReactRole;
using siscode_bot.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace siscode_bot.Commands {
    [Group("rr"),Description("React Role"),Emoji(":ok_hand:"),RequireSystem(typeof(Systems.ReactRole)),Imouto]
    public class ReactRole : BaseCommandModule {
        public Systems.ReactRole System = (Systems.ReactRole)SMC.RegisteredSystems[typeof(Systems.ReactRole)];
        [GroupCommand]
        public async Task GroupCommand(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.GroupHelpEmbed(ctx.Command));
        [Command("init"), Description("Starts a new category on the specified channel")]
        public async Task InitFail(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        [Command("init")]
        public async Task InitCommand(CommandContext ctx,
            [Description("The discord channel")] DiscordChannel channel,
            [RemainingText,Description("Name of the category")] string categoryName) {
            var Category = new Category {
                ChannelId = channel.Id,
                Name = categoryName.Trim()
            };
            var categories = System.categoryCache.Query().Where(x => x.ChannelId == Category.ChannelId && x.Name == Category.Name).ToList();
            if (categories.Any()) {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("This category already exists on said channel."));
                return;
            }
            var msg = await channel.SendMessageAsync(embed: await Category.ToEmbedAsync(ctx));
            Category.MessageId = msg.Id;
            System.categoryCache.Insert(Category);
            await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Category initialized."));
            System.Execute();
        }
        [Command("add"),Description("Adds a reaction-role pair to an category")]
        public async Task AddFail(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        [Command("add")]
        public async Task AddCommand(CommandContext ctx,
            [Description("The emoji")] DiscordEmoji emoji,
            [Description("The role (Id/Mention)")] DiscordRole role) {
            var categories = System.categoryCache.Query().Where(_ => true).ToList();
            var msg = await ctx.RespondAsync(embed: EmbedBase.OrderedListEmbed(categories.Select(x => $"<#{x.ChannelId}> - {x.Name}").ToList(),"Categories").Mutate(x => x.WithTitle("Select the category [#]")));
            var response = await ctx.Message.GetNextMessageAsync();
            if (response.TimedOut) {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Command canceled."));
                return;
            }
            int index = Math.Clamp(response.Result.FirstInt(),0,categories.Count - 1);
            var category = categories[index];
            var ch = ctx.Guild.GetChannel(category.ChannelId);
            var catmsg = await ch.GetMessageAsync(category.MessageId);
            if (category.Data.ContainsKey(emoji.Id)) {
                await msg.ModifyAsync(embed: EmbedBase.OutputEmbed($"Pair with emoji {emoji} already exists on {category.Name} @ <#{category.ChannelId}>"));
                return;
            }
            category.Data.TryAdd(emoji.Id, role.Id);
            System.categoryCache.Update(category);
            System.Execute();
            await catmsg.ModifyAsync(embed: await category.ToEmbedAsync(ctx));
            await catmsg.CreateReactionAsync(emoji);
            await msg.ModifyAsync(embed: EmbedBase.OutputEmbed($"Pair : {emoji} - {role.Mention} added to category {category.Name} @ <#{category.ChannelId}>"));
        }
        [Command("remove"),Description("Removes a reaction-role pair from an category")]
        public async Task RemoveFail(CommandContext ctx) => await ctx.RespondAsync(embed: EmbedBase.CommandHelpEmbed(ctx.Command));
        [Command("remove")]
        public async Task RemoveCommand(CommandContext ctx, [Description("The emoji")] DiscordEmoji emoji) {
            var categories = System.categoryCache.Query().Where(_ => true).ToList();
            var msg = await ctx.RespondAsync(embed: EmbedBase.OrderedListEmbed(categories.Select(x => $"<#{x.ChannelId}> - {x.Name}").ToList(), "Categories").Mutate(x => x.WithTitle("Select the category [#]")));
            var response = await ctx.Message.GetNextMessageAsync();
            if (response.TimedOut) {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Command canceled."));
                return;
            }
            int index = Math.Clamp(response.Result.FirstInt(), 0, categories.Count - 1);
            var category = categories[index];
            var ch = ctx.Guild.GetChannel(category.ChannelId);
            var catmsg = await ch.GetMessageAsync(category.MessageId);
            if (!category.Data.ContainsKey(emoji.Id)) {
                await msg.ModifyAsync(embed: EmbedBase.OutputEmbed($"Pair with emoji {emoji} doesn't exist on {category.Name} @ <#{category.ChannelId}>"));
                return;
            }
            category.Data.Remove(emoji.Id);
            System.categoryCache.Update(category);
            System.Execute();
            await catmsg.ModifyAsync(embed: await category.ToEmbedAsync(ctx));
            await catmsg.DeleteAllReactionsAsync(emoji);
            await msg.ModifyAsync(embed: EmbedBase.OutputEmbed($"Emoji : {emoji} removed from category {category.Name} @ <#{category.ChannelId}>"));
        }
        [Command("del"),Description("Deletes a **category**")]
        public async Task DelCommand(CommandContext ctx) {
            var categories = System.categoryCache.Query().Where(_ => true).ToList();
            var msg = await ctx.RespondAsync(embed: EmbedBase.OrderedListEmbed(categories.Select(x => $"<#{x.ChannelId}> - {x.Name}").ToList(), "Categories").Mutate(x => x.WithTitle("Select the category [#]")));
            var response = await ctx.Message.GetNextMessageAsync();
            if (response.TimedOut) {
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("Command canceled."));
                return;
            }
            int index = Math.Clamp(response.Result.FirstInt(), 0, categories.Count - 1);
            var category = categories[index];
            var ch = ctx.Guild.GetChannel(category.ChannelId);
            var catmsg = await ch.GetMessageAsync(category.MessageId);
            await catmsg.DeleteAsync();
            System.categoryCache.Delete(category.Id);
            System.Execute();
            await msg.ModifyAsync(embed: EmbedBase.OutputEmbed($"Category : {category.Name} @ <#{category.ChannelId}> deleted."));
        }
    }
}
