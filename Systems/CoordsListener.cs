using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using sisbase;
using sisbase.Utils;
using siscode_bot.Utils;

namespace siscode_bot.Systems {
    public class CoordsListener : IClientSystem {
        public void Activate() {
            Name = "CoordsListener";
            Status = true;
            Description = "Listen for coordinates on chat";
        }

        public void Deactivate() {
            Name = null; Description = null;
            Status = false;
            SisbaseBot.Instance.Client.MessageCreated -= ClientOnMessageCreated;
        }

        public void Execute() {
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public async Task ApplyToClient(DiscordClient client) {
            client.MessageCreated += ClientOnMessageCreated;
        }

        private async Task ClientOnMessageCreated(MessageCreateEventArgs e) {
            if(e.Message.Author == SisbaseBot.Instance.Client.CurrentUser) return;
            if(!MinecraftUtils.Coords3Regex.IsMatch(e.Message.Content)) return;
            var content = e.Message.Content.ToLowerInvariant();
            if (content.Contains("portal")) {
                var data = MinecraftUtils.Coords3Regex.Match(content).ToString().Split(" ").Select(x => int.Parse(x)).ToArray();
                var coord = new MinecraftUtils.Coordinate(data[0],data[1],data[2]);
                await e.Message.RespondAsync($"Assuming Overworld -> Nether : {coord.ConvertToNether()}\nAssuming Nether -> Overworld : {coord.ConvertToOverworld()}");
            }
        }
    }
}