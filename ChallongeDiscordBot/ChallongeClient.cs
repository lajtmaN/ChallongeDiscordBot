using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChallongeCSharpDriver.Caller;
using ChallongeCSharpDriver.Main;
using ChallongeCSharpDriver.Main.Objects;
using ChallongeDiscordBot.Entities;

namespace ChallongeDiscordBot
{
    public class ChallongeClient
    {
        private const int UPDATE_RATE_SECONDS = 10000;
        private Timer LoaderTimer { get; }
        private Tournaments Tournaments { get; }
        private DateTime CreatedAfterDate { get; }

        public DiscordChallongeDatabase Database { get; }

        public ChallongeClient(ChallongeDiscordBotConfig config)
            : this(config.ApiKey, config.Subdomain, config.CreatedAfter) { }
        private ChallongeClient(string apiKey, string subdomain, DateTime createdAfterDate)
        {
            var config = new ChallongeConfig(apiKey);
            var caller = new ChallongeHTTPClientAPICaller(config);
            Tournaments = new Tournaments(caller, subdomain);
            CreatedAfterDate = createdAfterDate;
            
            LoaderTimer = new Timer(e => LoadNewestData());
            Database = new DiscordChallongeDatabase();
        }

        public event OnNewMatchStartedEvent OnNewMatchStarted;
        public event OnTournamentStartedEvent OnTournamentStarted;

        public void StartLoaderThread()
        {
            LoaderTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(UPDATE_RATE_SECONDS));
        }


        /// <summary>
        /// Update DiscordChallengeDatabase with newest data
        /// </summary>
        public async void LoadNewestData()
        {
            await LoadNewestTournaments();
            await LoadNewestParticipants();
            await LoadNewestMatches();
        }
        
        private async Task LoadNewestTournaments()
        {
            var tournaments = await Tournaments.GetTournamentsCreatedAfter(CreatedAfterDate);
            foreach (ITournament tournament in tournaments)
            {
                if (await Database.Tournaments.FindAsync(tournament.TournamentID) == null)
                {
                    Database.Tournaments.Add(Tournament.CreateTournament(tournament));
                }
            }
            await Database.SaveChangesAsync();
        }

        private async Task LoadNewestParticipants()
        {
            foreach (var tourn in Database.Tournaments)
            {
                var challongeTournament = await Tournaments.getTournament(tourn.ShortName);
                var participants = await challongeTournament.GetParticipants();
                foreach (var chalPart in participants)
                {
                    if (await Database.Participants.FindAsync(chalPart.id.ID) == null)
                    {
                        Database.Participants.Add(Participant.CreateParticipant(chalPart));
                    }
                }
            }
            await Database.SaveChangesAsync();
        }

        private async Task LoadNewestMatches()
        {
            foreach (var tourn in Database.Tournaments)
            {
                var challongeTournament = await Tournaments.getTournament(tourn.ShortName);
                var matches = await challongeTournament.GetAllActiveMatches();
                foreach (var chalMatch in matches)
                {
                    if (await Database.Matches.FindAsync(chalMatch.id) == null)
                    {
                        Database.Matches.Add(Match.CreateMatch(chalMatch));
                    }
                }
            }
            await Database.SaveChangesAsync();
        }
    }

    public delegate void OnNewMatchStartedEvent(object sender, OnNewMatchStartedArgs args);
    public delegate void OnTournamentStartedEvent(object sender, OnTournamentStartedEventArgs args);

    public class OnTournamentStartedEventArgs
    {
        public OnTournamentStartedEventArgs(IStartedTournament tournament)
        {
            Tournament = tournament;
        }
        public IStartedTournament Tournament { get; }
    }

    public class OnNewMatchStartedArgs : EventArgs
    {
        public OnNewMatchStartedArgs(IOpenMatch match, IStartedTournament tournament)
        {
            Match = match;
            Tournament = tournament;
        }
        public IOpenMatch Match { get; }
        public IStartedTournament Tournament { get; }
    }
}
