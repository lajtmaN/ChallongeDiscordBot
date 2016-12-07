using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeDiscordBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationManager.GetSection("BotConfig") as ChallongeDiscordBotConfigSection;
            foreach (ChallongeDiscordBotConfig bot in config.Instances)
            {
                Console.WriteLine($"Creating bot for: {bot.Name} using ApiKey: {bot.ApiKey}");
                var loader = new ChallongeDataLoader(bot.ApiKey, bot.Subdomain, bot.CreatedAfter);
                LoadCS(loader);
            }
            Console.ReadLine();
        }

        static async void LoadCS(ChallongeDataLoader loader)
        {
            var tournament = await loader.LoadTournament("lajtman_discord");
            var matches = await tournament.GetAllOpenMatches();
            
        }
    }
}
