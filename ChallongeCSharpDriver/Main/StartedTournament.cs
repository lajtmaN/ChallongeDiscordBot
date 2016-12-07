using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main {
    public interface StartedTournament {
        Task<int> remainingUncompletedMatches { get; }
        Task<OpenMatch> getNextMatch();
    }
}
