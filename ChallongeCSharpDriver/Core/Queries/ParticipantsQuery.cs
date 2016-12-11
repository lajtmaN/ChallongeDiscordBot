using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Core.Queries
{

    using ChallongeCSharpDriver.Core.Results;
    using ChallongeCSharpDriver.Caller;

    public class ParticipantsQuery : ChallongeQuery<List<ParticipantResult>>
    {
        public string tournamentID { get; }

        public ParticipantsQuery(string tournamentID)
        {
            this.tournamentID = tournamentID;
        }

        private class ParticipantsQueryResult
        {
            public ParticipantResult participant { get; set; }
        }

        private ChallongeQueryParameters getParameters()
        {
            return new ChallongeQueryParameters();
        }

        private string getAPIPath()
        {
            return "tournaments/" + tournamentID + "/participants";
        }

        public async Task<List<ParticipantResult>> call(ChallongeAPICaller caller)
        {
            List<ParticipantsQueryResult> participantResults =
                await caller.GET<List<ParticipantsQueryResult>>(getAPIPath(), getParameters());
            return participantResults.Select(x => x.participant).ToList();
        }
    }
}
