using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChallongeCSharpDriver.Core.Results;

namespace ChallongeCSharpDriver.Core.Queries
{
    public class MatchQuery : ChallongeQuery<MatchResult>
    {
        private string TournamentID { get; }
        private int MatchID { get; }

        public MatchQuery(string tournamentID, int matchID)
        {
            TournamentID = tournamentID;
            MatchID = matchID;
        }
        private class MatchQueryResult
        {
            public MatchResult match { get; set; }
        }

        private ChallongeQueryParameters Parameters => new ChallongeQueryParameters();

        private string APIPath => $"tournaments/{TournamentID}/matches/{MatchID}";

        public async Task<MatchResult> call(ChallongeAPICaller caller)
        {
            MatchQueryResult result = await caller.GET<MatchQueryResult>(APIPath, Parameters);
            result.match.tournament_subdomain_id = TournamentID; //hack to add subdomain to matches
            return result.match;
        }
    }
}
