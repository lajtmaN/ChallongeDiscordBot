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

        private string getAPIPath(int? id)
        {
            if (id.HasValue)
                return "tournaments/" + tournamentID + "/participants/" + id.Value;
            return "tournaments/" + tournamentID + "/participants";
        }

        public async Task<ParticipantResult> call(ChallongeAPICaller caller) {
            int? id = ParticipantIDCache.Instance.GetParticipantID(ParticipantID);
            if (id.HasValue)
            {
                ParticipantQueryResult participantResult = await caller.GET<ParticipantQueryResult>(getAPIPath(id), getParameters());
                if (participantResult?.participant != null)
                    return participantResult.participant;
            }

            List<ParticipantQueryResult> participantGroupResult = await caller.GET<List<ParticipantQueryResult>>(getAPIPath(null), getParameters());
            ParticipantIDCache.Instance.PopulateCache(participantGroupResult.Select(x => x.participant).ToArray());
            return participantGroupResult.FirstOrDefault(x => x.participant.id == ParticipantID.ID || x.participant.group_player_ids.Contains(ParticipantID.GroupID))?.participant;
            
        }
    }
}
