using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChallongeCSharpDriver.Caller;
using ChallongeCSharpDriver.Core.Queries;
using ChallongeCSharpDriver.Main;
using ChallongeCSharpDriver.Main.Objects;
using ChallongeDiscordBot.Entities;

namespace ChallongeDiscordBot
{
    public class ChallongeClient
    {
        private const int UPDATE_RATE_SECONDS = 30;
        private Timer LoaderTimer { get; }
        private ChallongeTournaments ChallongeTournaments { get; }
        private ChallongeMatches ChallongeMatches { get; }
        private DateTime CreatedAfterDate { get; }

        public DiscordChallongeDatabase Database { get; }

        public ChallongeClient(ChallongeDiscordBotConfig config)
            : this(config.ApiKey, config.Subdomain, config.CreatedAfter) { }
        private ChallongeClient(string apiKey, string subdomain, DateTime createdAfterDate)
        {
            var config = new ChallongeConfig(apiKey);
            var caller = new ChallongeHTTPClientAPICaller(config);
            ChallongeTournaments = new ChallongeTournaments(caller, subdomain);
            ChallongeMatches = new ChallongeMatches(caller);

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
        private async void LoadNewestData()
        {
            await LoadNewestTournaments();
            await LoadNewestParticipants();
            await LoadNewestMatches();

            await AnnounceNewTournaments();
            await AnnounceNewMatches();
        }

        private async Task LoadNewestTournaments()
        {
            var tournaments = await ChallongeTournaments.GetTournamentsCreatedAfter(CreatedAfterDate);
            foreach (ITournament tournament in tournaments)
            {
                if (await Database.Tournaments.FindAsync(tournament.TournamentID) == null)
                {
                    Database.Tournaments.Add(Tournament.CreateTournament(tournament));
                    Console.WriteLine($"New tournament added: {tournament.Name}");
                }
            }
            await Database.SaveChangesAsync();
        }

        private async Task LoadNewestParticipants()
        {
            foreach (var tourn in Database.Tournaments)
            {
                var challongeTournament = await ChallongeTournaments.getTournament(tourn.ShortName);
                var participants = await challongeTournament.GetParticipants();
                foreach (var chalPart in participants)
                {
                    if (await Database.Participants.FindAsync(chalPart.id.ID) == null)
                    {
                        Database.Participants.Add(Participant.CreateParticipant(chalPart));
                        Console.WriteLine($"New participant added: {chalPart.name}");
                    }
                }
            }
            await Database.SaveChangesAsync();
        }

        private async Task LoadNewestMatches()
        {
            foreach (var tourn in Database.Tournaments)
            {
                var challongeTournament = await ChallongeTournaments.getTournament(tourn.ShortName);
                var matches = await challongeTournament.GetAllActiveMatches();
                foreach (var chalMatch in matches)
                {
                    if (await Database.Matches.FindAsync(chalMatch.id) == null)
                    {
                        Database.Matches.Add(Match.CreateMatch(chalMatch));
                        Console.WriteLine($"New match added: {chalMatch.id} for {challongeTournament.Name}");
                    }
                }
            }
            await Database.SaveChangesAsync();
        }

        private async Task AnnounceNewMatches()
        {
            foreach (var match in Database.Matches.Where(x => !x.Announced))
            {
                var challongeTournament = await ChallongeTournaments.getTournament(match.Tournament.ShortName);
                var challongeMatch = await ChallongeMatches.GetMatch(match.Tournament.ChallongeIDWithSubdomain, match.ID);
                var args = new OnNewMatchStartedArgs(challongeMatch, challongeTournament);
                OnNewMatchStarted?.Invoke(this, args);

                match.Announced = true;
            }
            await Database.SaveChangesAsync();
        }

        private async Task AnnounceNewTournaments()
        {
            foreach (var tourn in Database.Tournaments.Where(x => !x.Announced))
            {
                var challongeTournament = await ChallongeTournaments.getTournament(tourn.ShortName);
                var args = new OnTournamentStartedEventArgs(challongeTournament);
                OnTournamentStarted?.Invoke(this, args);

                tourn.Announced = true;
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
