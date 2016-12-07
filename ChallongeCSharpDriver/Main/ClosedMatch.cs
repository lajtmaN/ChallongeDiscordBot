using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main {
    public interface ClosedMatch {
        MatchState state { get; }
        Task<Participant> player1 { get; }
        Task<Participant> player2 { get; }
        Task<Participant> winner { get; }
        Task<Participant> loser { get; }
        OpenMatch reopen();
    }
}
