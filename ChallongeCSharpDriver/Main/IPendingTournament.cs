using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main {
    public interface IPendingTournament : ITournament
    {
        Task AddParticipant(String participant);
        Task<IStartedTournament> StartTournament();
    }
}
