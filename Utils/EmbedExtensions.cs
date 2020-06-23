using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using sisbase.Utils;
using siscode_bot.Data.ReactRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace siscode_bot.Utils {
    public static class EmbedExtensions {
        public static async Task<DiscordEmbed> ToEmbedAsync(this Category category, CommandContext ctx) {
            var dict = new Dictionary<DiscordEmoji, DiscordRole>();
            foreach(var entry in category.Data) {
                dict.Add(await ctx.Guild.GetEmojiAsync(entry.Key), ctx.Guild.GetRole(entry.Value));
            }
            var embed = EmbedBase.OutputEmbed("").Mutate(x => x.WithAuthor(category.Name)
            .WithDescription("Click on a reaction to get a role, Click again to remove it")
            .AddField("Available Roles",$"{(dict.Count == 0 ? "None" : string.Join("\n", dict.Select(x => $"{x.Key} - {x.Value.Mention}")))}")
            );
            return embed;
        }
        public static int FirstInt(this DiscordMessage m) =>
            int.Parse(m.Content.Split(" ").Where(x => int.TryParse(x, out int _)).FirstOrDefault());
    }
}
