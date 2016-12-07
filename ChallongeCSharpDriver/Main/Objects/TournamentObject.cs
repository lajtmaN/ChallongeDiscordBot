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

    public class TournamentObject : IStartedTournament, IPendingTournament {
        private ChallongeAPICaller caller;
        private TournamentResult result;
        public Task<int> remainingUncompletedMatches => getNumberOfUncompletedMatches();

        public TournamentObject(TournamentResult result, ChallongeAPICaller caller) {
            this.result = result;
            this.caller = caller;
        }

        public override string ToString() {
            return "Tournament #" + result.id + ", \"" + result.name + "\" at https://challonge.com/" + result.url + " (" + result.description + ")";
        }

        public async Task AddParticipant(String participant) {
            await new AddParticipantQuery(result.id, new ParticipantEntry(participant)).call(caller);
        }

        public async Task<IStartedTournament> StartTournament() {
            await new StartTournamentQuery(result.id).call(caller);
            return this;
        }

        public async Task<OpenMatch> getNextMatch() {
            List<MatchResult> matches = await new MatchesQuery(result.id) { matchState = MatchState.Open }.call(caller);
            if (matches.Count >= 0) {
                return new MatchObject(matches[0], caller);
            } else {
                throw new NoNextMatchAvailable();
            }
        }

        private async Task<int> getNumberOfUncompletedMatches() {
            List<MatchResult> matches = await new MatchesQuery(result.id).call(caller);
            return matches.Select(match => match.state != "completed").Count();
        }

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

        public string URL => result.url;
        public string Name => result.name;
        public string Description => result.description;
    }
}
