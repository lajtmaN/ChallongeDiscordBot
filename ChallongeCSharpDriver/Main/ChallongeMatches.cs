using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChallongeCSharpDriver.Core;
using ChallongeCSharpDriver.Core.Queries;
using ChallongeCSharpDriver.Main.Objects;

namespace ChallongeCSharpDriver.Main
{  
    public class ChallongeMatches
    {
        private ChallongeAPICaller Caller { get; }

        public ChallongeMatches(ChallongeAPICaller caller)
        {
            Caller = caller;
        }

        public async Task<MatchObject> GetMatch(string tournamentID, int matchID)
        {
            var res = await new MatchQuery(tournamentID, matchID).call(Caller);
            return new MatchObject(res, Caller);
        }
    }
}
