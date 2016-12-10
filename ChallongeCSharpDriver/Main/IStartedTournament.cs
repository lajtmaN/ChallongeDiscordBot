using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main {
    public interface IStartedTournament : ITournament
    {
        Task<int> remainingUncompletedMatches { get; }
        Task<IOpenMatch> getNextMatch();
        Task<List<IOpenMatch>> GetAllOpenMatches();
        Task<List<IOpenMatch>> GetAllActiveMatches();
    }
}
