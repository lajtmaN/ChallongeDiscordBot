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
                new ChallongeDataLoader(bot.ApiKey).LoadNewestData();
            }
            Console.ReadLine();

        }
    }
}
