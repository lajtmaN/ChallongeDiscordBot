using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChallongeCSharpDriver.Main;

namespace ChallongeDiscordBot
{
    public class ChallongeDiscordBot
    {
        private DiscordBot DiscordBot { get; set; }
        private ChallongeClient ChallongeClient { get; set; }

        public ChallongeDiscordBot(ChallongeDiscordBotConfig config)
        {
            this.DiscordBot = new DiscordBot(config);
            this.ChallongeClient = new ChallongeClient(config);

            AttachEvents();

            DiscordBot.StartDiscordBotThread();
            ChallongeClient.StartLoaderThread();
        }

        private void AttachEvents()
        {
            ChallongeClient.OnNewMatchStarted += OnOnNewMatchStarted;
        }


        private static async void OnOnNewMatchStarted(object sender, OnNewMatchStartedArgs args)
        {
            IParticipant team1 = await args.Match.player1;
            IParticipant team2 = await args.Match.player2;


        }
    }
}
