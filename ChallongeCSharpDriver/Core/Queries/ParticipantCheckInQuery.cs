using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Core.Queries
{
    public class ParticipantCheckInQuery : ChallongeQuery<bool>
    {
        private string tournamentID { get; }
        private int participantID { get; }

        public ParticipantCheckInQuery(string tournId, int partId)
        {
            tournamentID = tournId;
            participantID = partId;
        }

        private string API => $"tournaments/{tournamentID}/participants/{participantID}/check_in";

        public async Task<bool> call(ChallongeAPICaller caller)
        {
            return await caller.POST(API, new ChallongeQueryParameters());
        }

        public static async Task<bool> CheckIn(string tournId, int partId, ChallongeAPICaller caller)
        {
            return await new ParticipantCheckInQuery(tournId, partId).call(caller);
        }
    }
}
