using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main
{
    public interface IMatch
    {
        MatchState state { get; }
        Task<IParticipant> player1 { get; }
        Task<IParticipant> player2 { get; }
        string Location { get; }
    }
}
