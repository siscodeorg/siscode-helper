using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace siscode_bot.Commands {
    public class FakeDND : BaseCommandModule {
        [Command("fakednd")]
        public async Task FakeDNDCommand(CommandContext ctx) {
            var role = ctx.Guild?.Roles.FirstOrDefault(x => x.Value.Name.ToLowerInvariant().Contains("fakednd")).Value;
            if (role == null){
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("No FakeDND role was found."));
                return;
            }
            if (ctx.Member.Roles.Contains(role)) {
                await ctx.Member.RevokeRoleAsync(role);
                await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("FakeDND toggled off.")); 
                return;
            }
            await ctx.Member.GrantRoleAsync(role);
            await ctx.RespondAsync(embed: EmbedBase.OutputEmbed("FakeDND toggled on."));
        }
    }
}
