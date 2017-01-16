using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChallongeCSharpDriver.Caller;
using ChallongeCSharpDriver.Core.Queries;
using ChallongeCSharpDriver.Main;
using ChallongeCSharpDriver.Main.Objects;
using ChallongeDiscordBot.Entities;
using RestSharp.Extensions;

namespace ChallongeDiscordBot
{
    public class ChallongeClient
    {
        private const int UPDATE_RATE_SECONDS = 30;
        private Timer LoaderTimer { get; }
        private ChallongeTournaments ChallongeTournaments { get; }
        private ChallongeMatches ChallongeMatches { get; }
        private ChallongeHTTPClientAPICaller Caller { get; }
        private DateTime CreatedAfterDate { get; }

        public DiscordChallongeDatabase Database { get; }

        public ChallongeClient(ChallongeDiscordBotConfig config)
            : this(config.ApiKey, config.Subdomain, config.CreatedAfter) { }
        private ChallongeClient(string apiKey, string subdomain, DateTime createdAfterDate)
        {
            var config = new ChallongeConfig(apiKey);
            Caller = new ChallongeHTTPClientAPICaller(config);
            ChallongeTournaments = new ChallongeTournaments(Caller, subdomain);
            ChallongeMatches = new ChallongeMatches(Caller);

            CreatedAfterDate = createdAfterDate;
            
            LoaderTimer = new Timer(e => LoadNewestData());
            Database = new DiscordChallongeDatabase();
        }

        public event OnNewParticipantRegisteredEvent OnNewParticipantRegistered;
        public event OnNewMatchStartedEvent OnNewMatchStarted;
        public event OnTournamentStartedEvent OnTournamentStarted;
        public event OnTournamentCheckInOpenedEvent OnTournamentCheckInOpened;

        public void StartLoaderThread()
        {
            LoaderTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(UPDATE_RATE_SECONDS));
            Console.WriteLine($"Starting Challonge Client. Reloading with {UPDATE_RATE_SECONDS} sec. interval.");
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
                    var participant = await Database.Participants.FindAsync(chalPart.id.ID);
                    bool exist = participant != null;
                    if (chalPart.active && !exist)
                    {
                        Database.Participants.Add(Participant.CreateParticipant(chalPart));
                        OnNewParticipantRegistered?.Invoke(this, new OnNewParticipantRegisteredEventArgs(chalPart, challongeTournament));
                        Console.WriteLine($"New participant added: {chalPart.name}");
                    }
                    else if (!chalPart.active && exist)
                    {
                        Database.Participants.Remove(participant);
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
                var matches = await challongeTournament.GetAllOpenMatches();
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
                if (!challongeMatch.MatchMarkedAsActive)
                    continue;

                var team1 = await Database.Participants.FindAsync(ParticipantIDCache.Instance.GetParticipantID(challongeMatch.player1_id.Value));
                var team2 = await Database.Participants.FindAsync(ParticipantIDCache.Instance.GetParticipantID(challongeMatch.player2_id.Value));
                var args = new OnNewMatchStartedArgs
                {
                    Match = challongeMatch,
                    Tournament = challongeTournament,
                    Team1DiscordName = team1.DiscordMentionName,
                    Team1SeatNum = team1.SeatNum,
                    Team2DiscordName = team2.DiscordMentionName,
                    Team2SeatNum = team2.SeatNum
                };
                OnNewMatchStarted?.Invoke(this, args);

                match.Announced = true;
            }
            await Database.SaveChangesAsync();
        }

        private async Task AnnounceNewTournaments()
        {
            foreach (var tourn in Database.Tournaments.Where(x => !x.Announced || !x.CheckInOpen))
            {
                var challongeTournament = await ChallongeTournaments.getTournament(tourn.ShortName);
                var args = new OnTournamentStartedEventArgs(challongeTournament);

                if (!tourn.Announced)
                {
                    OnTournamentStarted?.Invoke(this, args);
                    tourn.Announced = true;
                }
                if (challongeTournament.CheckInStartedTime.HasValue && !tourn.CheckInOpen)
                {
                    OnTournamentCheckInOpened?.Invoke(this, args);
                    tourn.CheckInOpen = true;
                }
            }
            await Database.SaveChangesAsync();
        }

        public async void CheckUserIn(int tournamentId, string teamDisplayName, string discordUserName, string discordMention, int seatNum)
        {
            Participant participant = await Database.Participants.FirstAsync(x => x.DisplayName == teamDisplayName && x.TournamentID == tournamentId);
            bool alreadyCheckedIn = participant.CheckedIn;

            participant.SeatNum = seatNum;
            participant.DiscordUserName = discordUserName;
            participant.DiscordMentionName = discordMention;
            participant.CheckedIn = true;

            if (!alreadyCheckedIn)
            {
                string fullTournamentId = (await Database.Tournaments.FindAsync(tournamentId))?.ChallongeIDWithSubdomain;
                if (fullTournamentId.HasValue())
                    await ParticipantCheckInQuery.CheckIn(fullTournamentId, participant.ID, Caller);
            }

            await Database.SaveChangesAsync();
        }
    }

    public delegate void OnNewParticipantRegisteredEvent(object sender, OnNewParticipantRegisteredEventArgs args);
    public delegate void OnNewMatchStartedEvent(object sender, OnNewMatchStartedArgs args);
    public delegate void OnTournamentStartedEvent(object sender, OnTournamentStartedEventArgs args);
    public delegate void OnTournamentCheckInOpenedEvent(object sender, OnTournamentStartedEventArgs args);

    public class OnNewParticipantRegisteredEventArgs : EventArgs
    {
        public OnNewParticipantRegisteredEventArgs(IParticipant p, ITournament t)
        {
            Participant = p;
            Tournament = t;
        }
        public IParticipant Participant { get; }
        public ITournament Tournament { get; }
    }

    public class OnTournamentStartedEventArgs : EventArgs
    {
        public OnTournamentStartedEventArgs(IStartedTournament tournament)
        {
            Tournament = tournament;
        }
        public IStartedTournament Tournament { get; }
    }

    public class OnNewMatchStartedArgs : EventArgs
    {
        public IOpenMatch Match { get; set; }
        public IStartedTournament Tournament { get; set; }
        public string Team1DiscordName { get; set; }
        public int? Team1SeatNum { get; set; }
        public string Team2DiscordName { get; set; }
        public int? Team2SeatNum { get; set; }
    }
}
