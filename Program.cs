using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord.WebSocket;
using sisbase;
using sisbase.Configuration;

namespace siscode_helper
{
    class Program {

        private static Assembly ASSEMBLY = typeof(Program).Assembly; 
        static async Task Main(string[] args) {
            DiscordSocketClient client = new();
            SisbaseBot bot = new(client, new FileInfo($"{Directory.GetCurrentDirectory()}/config.json"));
            SystemConfig systemConfig = new();
            
            systemConfig.Create(new FileInfo($"{Directory.GetCurrentDirectory()}/systems.json"));
           
            bot.UseSystemsApi(systemConfig);
            
            await bot.InstallCommandsAsync(ASSEMBLY);
            await bot.InstallSystemsAsync(ASSEMBLY);

            await bot.StartAsync();
        }
    }
}
