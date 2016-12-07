using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Core.Queries {
    using ChallongeCSharpDriver.Caller;
    using ChallongeCSharpDriver.Core.Results;
    using ChallongeCSharpDriver.Main;
    using System.Net.Http;

    public class TournamentsQuery : ChallongeQuery<List<TournamentResult>> {
        public TournamentType? type { get; set; }
        public TournamentState? state { get; set; }
        public DateTime? created_after { get; set; }
        public string Subdomain { get; set; }

        private class TournamentsQueryResult {
            public TournamentResult tournament { get; set; }
        }

        private ChallongeQueryParameters getParameters() {
            ChallongeQueryParameters parameters = new ChallongeQueryParameters();
            if (type.HasValue) {
                parameters.Add("type", TournamentTypeParser.ToIndexString(type.Value));
            }
            if (state.HasValue) {
                parameters.Add("state", TournamentStateParser.ToString(state.Value));
            }
            if (created_after.HasValue)
            {
                parameters.Add("created_after", created_after.Value.ToString("yyyy-MM-dd"));
            }
            if (!string.IsNullOrWhiteSpace(Subdomain))
            {
                parameters.Add("subdomain", Subdomain);
            }
            return parameters;
        }

        private string getAPIPath() {
            return "tournaments";
        }

        public async Task<List<TournamentResult>> call(ChallongeAPICaller caller) {
            List<TournamentsQueryResult> tournamentsQueryResult = await caller.GET<List<TournamentsQueryResult>>(getAPIPath(), getParameters());
            List<TournamentResult> tournaments = new List<TournamentResult>();
            foreach (TournamentsQueryResult queryResult in tournamentsQueryResult) {
                tournaments.Add(queryResult.tournament);
            }
            return tournaments;
        }
    }
}
