using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Core.Queries {
    using ChallongeCSharpDriver.Core.Results;
    using ChallongeCSharpDriver.Caller;
    using ChallongeCSharpDriver.Main;

    public class ParticipantQuery : ChallongeQuery<ParticipantResult> {
        public string tournamentID { get; set; }
        public ParticipantID ParticipantID { get; set; }

        public ParticipantQuery(string tournamentID, ParticipantID id) {
            this.tournamentID = tournamentID;
            this.ParticipantID = id;
        }

        private class ParticipantQueryResult {
            public ParticipantResult participant { get; set; }
        }

        private ChallongeQueryParameters getParameters() {
            return new ChallongeQueryParameters();
        }

        private string getAPIPath() {
            return "tournaments/" + tournamentID + "/participants/" + ParticipantID.ID;
        }

        private string getGroupStageApiPath()
        {
            return "tournaments/" + tournamentID + "/participants";
        }

        public async Task<ParticipantResult> call(ChallongeAPICaller caller) {
            //TODO: Implement some cache here please

            if (ParticipantID.ID > 0)
            {
                try
                {
                    ParticipantQueryResult participantResult = await caller.GET<ParticipantQueryResult>(getAPIPath(), getParameters());
                    if (participantResult?.participant != null)
                        return participantResult.participant;
                } catch(Exception) { }
            }

            //If scrub challonge only provided group_player_ids, we fetch ALL participants and find the one we want
            try
            {
                List<ParticipantQueryResult> participantGroupResult = await caller.GET<List<ParticipantQueryResult>>(getGroupStageApiPath(), getParameters());
                return participantGroupResult.FirstOrDefault(x => x.participant.group_player_ids.Contains(ParticipantID.GroupID))?.participant;
            } catch(Exception) { }
            return null;

        }
    }
}
