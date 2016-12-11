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
        private DiscordBot DiscordBot { get; }
        private ChallongeClient ChallongeClient { get; }

        public ChallongeDiscordBot(ChallongeDiscordBotConfig config)
        {
            this.DiscordBot = new DiscordBot(config);
            this.ChallongeClient = new ChallongeClient(config);

            AttachEvents();

            DiscordBot.StartDiscordBotThread();
        }

        private void AttachEvents()
        {
            DiscordBot.OnDiscordBotReady += (s, a) => ChallongeClient.StartLoaderThread();
            ChallongeClient.OnNewMatchStarted += OnOnNewMatchStarted;
            ChallongeClient.OnTournamentStarted += ChallongeClientOnOnTournamentStarted;
        }
        
        private async void ChallongeClientOnOnTournamentStarted(object sender, OnTournamentStartedEventArgs args)
        {
            string channelName = args.Tournament.URL;
            bool channelExist = await DiscordBot.CreateChannel(channelName);
            DiscordBot.SendMessage($"{args.Tournament.Name} konkurrencen er nu startet...{Environment.NewLine}Hold dig venligst opdateret i denne kanal hvis du deltager i turneringen.", channelName);
            Console.WriteLine($"{args.Tournament.Name} has started");
        }

        private async void OnOnNewMatchStarted(object sender, OnNewMatchStartedArgs args)
        {
            IParticipant team1 = await args.Match.player1;
            IParticipant team2 = await args.Match.player2;

            string message = $":gun: {team1.name} vs {team2.name} :gun:{Environment.NewLine}Kampen er klar til at blive spillet!";
            if (!string.IsNullOrWhiteSpace(args.Match.Location))
                message += $"{Environment.NewLine}Server: {args.Match.Location}";

            DiscordBot.SendMessage(message, args.Tournament.URL);
            Console.WriteLine($"Match {team1.name} vs {team2.name} is ready");
        }
    }
}
