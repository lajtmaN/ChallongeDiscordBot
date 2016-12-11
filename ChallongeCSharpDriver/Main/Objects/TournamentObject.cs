using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChallongeCSharpDriver.Core;
using ChallongeCSharpDriver.Core.Objects;
using ChallongeCSharpDriver.Core.Queries;
using ChallongeCSharpDriver.Core.Results;


namespace ChallongeCSharpDriver.Main.Objects {

    public class TournamentObject : IStartedTournament, IPendingTournament
    {
        private ChallongeAPICaller caller;
        private TournamentResult result;
        public Task<int> remainingUncompletedMatches => getNumberOfUncompletedMatches();

        public TournamentObject(TournamentResult result, ChallongeAPICaller caller) {
            this.result = result;
            this.caller = caller;
        }

        public override string ToString() => $"Tournament #{TournamentID}, \"{Name}\"";

        public async Task AddParticipant(string participant) {
            await new AddParticipantQuery(result.id, new ParticipantEntry(participant)).call(caller);
        }

        public async Task<IStartedTournament> StartTournament() {
            await new StartTournamentQuery(TournamentSubdomainID).call(caller);
            return this;
        }

        public async Task<IOpenMatch> getNextMatch() {
            List<MatchResult> matches = await new MatchesQuery(TournamentSubdomainID) { matchState = MatchState.Open }.call(caller);
            if (matches.Count > 0) {
                return new MatchObject(matches[0], caller);
            } else {
                throw new NoNextMatchAvailable();
            }
        }

        public async Task<List<IOpenMatch>> GetAllOpenMatches()
        {
            var list = await new MatchesQuery(TournamentSubdomainID) {matchState = MatchState.Open }.call(caller);
            return list.Select(r => new MatchObject(r, caller)).Cast<IOpenMatch>().ToList();
        }

        public async Task<List<IOpenMatch>> GetAllActiveMatches()
        {
            var openMatches = await GetAllOpenMatches();
            return openMatches.Where(x => x.MatchMarkedAsActive).ToList();
        }
        
        private async Task<int> getNumberOfUncompletedMatches() {
            List<MatchResult> matches = await new MatchesQuery(TournamentSubdomainID).call(caller);
            return matches.Select(match => match.state != "completed").Count();
        }

        public string TournamentSubdomainID => string.IsNullOrWhiteSpace(SubDomain) ? TournamentID.ToString() : $"{SubDomain}-{URL}";

        public TournamentState State
        {
            get
            {
                TournamentState res;
                if (Enum.TryParse(result.state, true, out res))
                {
                    return res;
                }
                throw new NotSupportedException(result.state + " was not recognized");
            }
        }

        public int TournamentID => result.id;
        public string URL => result.url;
        public string FullLink => result.full_challonge_url;
        public string SubDomain => result.subdomain;
        public string Name => result.name;
        public string Description => result.description;
        public DateTime? CheckInStartedTime => result.started_checking_in_at;

        public async Task<IList<IParticipant>> GetParticipants()
        {
            var participantResults = await new ParticipantsQuery(TournamentSubdomainID).call(caller);
            return participantResults.Select(r => new ParticipantObject(r)).Cast<IParticipant>().ToList();
        }
    }
}
