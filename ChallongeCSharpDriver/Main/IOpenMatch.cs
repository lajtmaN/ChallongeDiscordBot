using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main {
    public interface IOpenMatch : IMatch
    {
        void addScore(Score score);
        Task update();
        DateTime StartedAt { get; }
        Task<IClosedMatch> close();
    }
}
