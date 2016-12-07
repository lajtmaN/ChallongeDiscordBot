using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main {
    public interface OpenMatch {
        MatchState state { get; }
        Task<Participant> player1 { get; }
        Task<Participant> player2 { get; }
        void addScore(Score score);
        Task update();
        Task<ClosedMatch> close();
    }
}
