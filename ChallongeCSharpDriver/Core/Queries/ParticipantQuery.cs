using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Core.Queries {
    using ChallongeCSharpDriver.Core.Results;
    using ChallongeCSharpDriver.Caller;

    public class ParticipantQuery : ChallongeQuery<ParticipantResult> {
        public string tournamentID { get; set; }
        public int participantID { get; set; }

        public ParticipantQuery(string tournamentID, int participantID) {
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
            //return "tournaments/" + tournamentID + "/participants/" + participantID; //TODO Does not work in group stage tournaments
            return "tournaments/" + tournamentID + "/participants";
        }

        public async Task<ParticipantResult> call(ChallongeAPICaller caller) {
            List<ParticipantQueryResult> participantResults = await caller.GET<List<ParticipantQueryResult>>(getAPIPath(), getParameters());
            return participantResults.First(x => x.participant.id == participantID || x.participant.group_player_ids.Contains(participantID))?.participant;
        }
    }
}
