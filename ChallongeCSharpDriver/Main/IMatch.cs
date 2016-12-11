using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main
{
    public interface IMatch
    {
        int id { get; }
        int tournament_id { get; }
        MatchState state { get; }
        int? player1_id { get; }
        Task<IParticipant> player1 { get; }
        int? player2_id { get; }
        Task<IParticipant> player2 { get; }
        string Location { get; }
    }
}
