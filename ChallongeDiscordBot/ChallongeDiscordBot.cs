using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChallongeCSharpDriver.Main;
using RestSharp.Extensions;

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
            DiscordBot.OnUserCheckedIn += DiscordBotOnOnUserCheckedIn;
            ChallongeClient.OnNewMatchStarted += OnOnNewMatchStarted;
            ChallongeClient.OnTournamentStarted += ChallongeClientOnOnTournamentStarted;
            ChallongeClient.OnTournamentCheckInOpened += ChallongeClientOnOnTournamentCheckInOpened;
            ChallongeClient.OnNewParticipantRegistered += ChallongeClientOnOnNewParticipantRegistered;
        }
        
        private void DiscordBotOnOnUserCheckedIn(object sender, UserCheckedInEventArgs args)
        {
            ChallongeClient.CheckUserIn(args.TournamentID, args.TeamName, args.User.Name, args.User.Mention, args.SeatNum);
            Console.WriteLine($"{args.TeamName} just checked in from seatnum {args.SeatNum} by Discord User: {args.User.Name}");
        }

        private async void ChallongeClientOnOnTournamentStarted(object sender, OnTournamentStartedEventArgs args)
        {
            string channelName = args.Tournament.URL;
            bool channelExist = await DiscordBot.CreateChannel(channelName);
            DiscordBot.SendMessage($"I denne kanal vil der komme informationer fra {args.Tournament.Name} turneringen.{Environment.NewLine}Hold dig venligst opdateret i denne kanal hvis du deltager i turneringen.", channelName);
            Console.WriteLine($"{args.Tournament.Name} has started");
        }

        private async void OnOnNewMatchStarted(object sender, OnNewMatchStartedArgs args)
        {
            IParticipant team1 = await args.Match.player1;
            IParticipant team2 = await args.Match.player2;

            var teamNameGenerator = new Func<string, string, int?, string>((t, u, p) => "(" + t + (!string.IsNullOrWhiteSpace(u) ? $" {u}" : "") + (p.HasValue ? $" pladsnr.: {p}" : "") + ")");
            
            string message = $":gun:** {teamNameGenerator(team1.name, args.Team1DiscordName, args.Team1SeatNum)} vs {teamNameGenerator(team2.name, args.Team2DiscordName, args.Team2SeatNum)} **:gun:" +
                             $"{Environment.NewLine}Kampen er klar til at blive spillet!";
            if (!string.IsNullOrWhiteSpace(args.Match.Location))
                message += $"{Environment.NewLine}Server: {args.Match.Location}";

            DiscordBot.SendMessage(message, args.Tournament.URL);
            Console.WriteLine($"Match {team1.name} vs {team2.name} is ready");
        }

        private void ChallongeClientOnOnTournamentCheckInOpened(object sender, OnTournamentStartedEventArgs args)
        {
            string channelName = args.Tournament.URL;
            string message = $"@everyone Det er nu muligt at meddele sin ankomst til {args.Tournament.Name} turneringen.{Environment.NewLine}"
                           + $"For at checke ind, skriver du: '{DiscordBot.BOT_PREFIX}checkin', og følger de angivne instruktioner.";
            DiscordBot.SendMessage(message, channelName);
        }
        
        private void ChallongeClientOnOnNewParticipantRegistered(object sender, OnNewParticipantRegisteredEventArgs args)
        {
            string message = $"{args.Participant.name} har lige tilmeldt sig {args.Tournament.Name} turneringen";
            DiscordBot.SendMessage(message, args.Tournament.URL);
        }
    }
}
