using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Core.Queries {
    using ChallongeCSharpDriver.Core;
    using ChallongeCSharpDriver.Core.Results;

    public class StartTournamentQuery : ChallongeQuery<bool> {
        public string tournamentID { get; set; }

        public StartTournamentQuery(string tournamentID) {
            this.tournamentID = tournamentID;
        }

        private ChallongeQueryParameters getParameters() {
            return new ChallongeQueryParameters();
        }

        private string getAPIPath() {
            return "tournaments/" + tournamentID + "/start";
        }

        public async Task<bool> call(ChallongeAPICaller caller) {
            return await caller.POST(getAPIPath(), getParameters());
        }
    }
}
