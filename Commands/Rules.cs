using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using sisbase.CommandsNext;

namespace siscode_helper.Commands {
    [RequireUserPermission(GuildPermission.Administrator)]
    [Group("rules"),Summary("Generates siscode's legal code")]
    public class Rules : ModuleBase<SisbaseCommandContext> {
        [Command("generate"), Summary("Generates rules")]
        public async Task GenerateCommand() {
            Color mainColor = new(0x9B83EA); ;
            EmbedBuilder[] embeds = {
                new EmbedBuilder()
                    .WithAuthor("Respect everyone")
                    .WithColor(mainColor)
                    .WithDescription(
                        "➥ You will treat every other user with respect, **NO EXCEPTIONS**.\n"+ 
                        "➥ Don't be too edgy.\n"+
                        "➥ Don't ping people for no reason.\n"+
                        "➥ Don't cause drama.\n"+ 
                        "➥ Show common sense."
                    ),
                new EmbedBuilder()
                    .WithAuthor("Spamming")
                    .WithColor(mainColor)
                    .WithDescription(
                        "➥ Spamming of text, pictures, emotes or reactions in any channel is prohibited and is likely to mute you at best, ban you at worse.\n"+
                        "➥ Posting multiple pictures should be done in <#715246616419893319>, <#830134322186027038> and <#700157411650175106> depending on context."
                    )
                    .WithFooter("If you see #deleted-channel on either of those its because you don't have access to said channel. Don't ask us about that."),
                new EmbedBuilder()
                    .WithAuthor("Prohibited content")
                    .WithColor(mainColor)
                    .WithDescription(
                        "➥ 3D lewds. Just don't.\n"+
                        "➥ Do not post any content showing gore, AT ALL. No real life and fictional.\n"+
                        "➥ [Plzbro](https://www.urbandictionary.com/define.php?term=plzbro)ing or giving out plzbro solutions.\n"+
                        "➥ Memes are limited to oxe's <#232944137140305921>."
                    )
                    .WithFooter("#deleted-channel on this one could mean you aren't on OxE's discord. This is just a joke, post memes idk on #offtopic or smth.")
                ,
                new EmbedBuilder()
                    .WithAuthor("Copyrighted content")
                    .WithColor(mainColor)
                    .WithDescription(
                        "➥ We have a dedicated channel for sharing infohashes.\n"+
                        "➥ Linking streaming sites should be avoided but is tolerated when kept to a minimum. If possible, use nyaa or BakaBT.\n"+
                        "➥ Do not encourage so-called \"legal streaming\" websites. Don't advertise or imply how certain media can be legally obtained."
                    ),
                new EmbedBuilder()
                    .WithAuthor("Advertising")
                    .WithColor(mainColor)
                    .WithDescription(
                        "➥ You are allowed to mention other discord servers but don't actively recruit members for it. Invite links shouldn't be sent in chat.\n"+
                        "➥ You can share streams, videos, art etc. of your favourite content creator, including your own, unless if it's locked behind a payment e.g. Patreon and similar sites."
                    ),
                new EmbedBuilder()
                    .WithAuthor("How to ask questions")
                    .WithColor(mainColor)
                    .WithDescription(
                        "➥ Give us all the information needed to help you. Ex : \"I'm using <framework> on <language>, I'm trying to do [<x>](https://xyproblem.info) attempted [<y>](https://xyproblem.info), but its giving me this <error> here is <code>\"\n"+
                        "➥ Be careful to not end up wasting our time with an [XY Problem](https://xyproblem.info).\n"+
                        "➥ Discord had an update where if you drop a text file with a known extension it formats and inlines it nicely, removing the need for using [hatebin](https://hatebin.com). The current order of preference is : Discord Inline Feature (For >2k) , Discord Code Blocks (For <2k) ,Hatebin.\n"
                    ),
                new EmbedBuilder()
                    .WithAuthor("Assume all conversations are cooperative")
                    .WithColor(mainColor)
                    .WithDescription(
                        "➥ Due to activity (and light speed not being fast enough) the order of the messages received will sometimes make no sense as it is.. Please re-organize them mentally in the order that makes the most sense for you (which likely would be the same for anyone)."
                    )
            };

            foreach (var embed in embeds) {
                await ReplyAsync(embed: embed.Build());
            }
        }
    }
}