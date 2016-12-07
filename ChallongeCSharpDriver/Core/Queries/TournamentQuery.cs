using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Core.Queries {
    using ChallongeCSharpDriver.Core;
    using ChallongeCSharpDriver.Core.Results;

    public class TournamentQuery : ChallongeQuery<TournamentResult> {
        private string tournamentID { get; }
        private string subdomain { get; }

        public TournamentQuery(string tournamentID, string subdomain) {
            this.tournamentID = tournamentID;
            this.subdomain = subdomain;
        }

        private class TournamentQueryResult {
            public TournamentResult tournament { get; set; }
        }

        private ChallongeQueryParameters getParameters() {
            return new ChallongeQueryParameters();
        }

        private string getAPIPath()
        {
            string prefix = !string.IsNullOrWhiteSpace(subdomain) ? "-" + subdomain : "";
            return $"tournaments/{prefix}{tournamentID}";
        }

        public async Task<TournamentResult> call(ChallongeAPICaller caller) {
            TournamentQueryResult tournamentQueryResult = await caller.GET<TournamentQueryResult>(getAPIPath(), getParameters());
            return tournamentQueryResult.tournament;
        }
    }
}
