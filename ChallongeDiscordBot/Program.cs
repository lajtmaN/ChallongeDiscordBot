using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ChallongeCSharpDriver.Main;
using Discord;

namespace ChallongeDiscordBot
{
    class Program
    {
        static void Main(string[] args)
        {
            ChallongeDiscordBotConfig config = LoadConfig();
            new ChallongeDiscordBot(config);

            string TurnOffCommand = "DIE";
            string readline;
            while ((readline = Console.ReadLine()) != TurnOffCommand);
        }

        private static void LoadFromChallonge(ChallongeDiscordBotConfig bot)
        {
            var loader = new ChallongeDataLoader(bot.ApiKey, bot.Subdomain, bot.CreatedAfter);
            LoadCS(loader);
        }

        static async void LoadCS(ChallongeDataLoader loader)
        {
            var tournament = await loader.LoadTournament("lajtman_discord");
            var matches = await tournament.GetAllOpenMatches();
            
        }

        private static ChallongeDiscordBotConfig LoadConfig()
        {
            var config = ConfigurationManager.GetSection("BotConfig") as ChallongeDiscordBotConfigSection;
            foreach (ChallongeDiscordBotConfig bot in config?.Instances)
            {
                Console.WriteLine($"Config loaded for: {bot.Name}");
                return bot;
            }
            throw new ApplicationException("No config could be loaded");
        }
    }
}
