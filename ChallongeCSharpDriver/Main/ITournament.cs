using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main
{
    public interface ITournament
    {
        int TournamentID { get; }
        string TournamentSubdomainID { get; }

        TournamentState State { get; }

        string URL { get; }

        string SubDomain { get; }

        string Name { get; }

        string Description { get; }
    }
}
