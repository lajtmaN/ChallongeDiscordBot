using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main {
    public interface IClosedMatch
    {
        Task<IParticipant> winner { get; }
        Task<IParticipant> loser { get; }
        IOpenMatch reopen();
    }
}
