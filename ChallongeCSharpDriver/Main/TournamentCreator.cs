
namespace ChallongeCSharpDriver.Main {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ChallongeCSharpDriver.Core.Queries;
    using ChallongeCSharpDriver.Core.Results;
    using ChallongeCSharpDriver.Core;
    using ChallongeCSharpDriver.Main.Objects;

    public class TournamentCreator {
        public ChallongeAPICaller caller { get; set; }
        public TournamentCreator(ChallongeAPICaller caller) {
            this.caller = caller;
        }

        public async Task<PendingTournament> create(string name, TournamentType type, string url) {
            TournamentResult result = await new CreateTournamentQuery(name, type, url).call(caller);
            return new TournamentObject(result, caller);
        }
    }
}
