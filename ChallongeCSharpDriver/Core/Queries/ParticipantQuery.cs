using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Core.Queries {
    using ChallongeCSharpDriver.Core.Results;
    using ChallongeCSharpDriver.Caller;

    public class ParticipantQuery : ChallongeQuery<ParticipantResult> {
        public int tournamentID { get; set; }
        public int participantID { get; set; }

        public ParticipantQuery(int tournamentID, int participantID) {
            this.tournamentID = tournamentID;
            this.participantID = participantID;
        }

        private class ParticipantQueryResult {
            public ParticipantResult participant { get; set; }
        }

        private ChallongeQueryParameters getParameters() {
            return new ChallongeQueryParameters();
        }

        private string getAPIPath() {
            return "tournaments/" + tournamentID + "/participants/" + participantID;
        }

        public async Task<ParticipantResult> call(ChallongeAPICaller caller) {
            ParticipantQueryResult participantResult = await caller.GET<ParticipantQueryResult>(getAPIPath(), getParameters());
            return participantResult.participant;
        }
    }
}
