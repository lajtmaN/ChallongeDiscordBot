﻿
using System.Linq;

namespace ChallongeCSharpDriver.Main {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ChallongeCSharpDriver.Core;
    using ChallongeCSharpDriver.Core.Results;
    using ChallongeCSharpDriver.Core.Queries;
    using System.Threading.Tasks;
    using ChallongeCSharpDriver.Main.Objects;

    public class Tournaments {
        ChallongeAPICaller caller;
        private string TournamentSubdomain { get; }

        public Tournaments(ChallongeAPICaller caller, string subdomain = "") {
            this.caller = caller;
            this.TournamentSubdomain = subdomain;
        }

        public async Task<List<IStartedTournament>> getStartedTournaments() {
            List<TournamentResult> tournamentResultList = await new TournamentsQuery() {
                Subdomain = TournamentSubdomain,
                state = TournamentState.In_Progress
            }
            .call(caller);
            List<IStartedTournament> tournamentList = new List<IStartedTournament>();
            foreach (TournamentResult result in tournamentResultList) {
                tournamentList.Add(new TournamentObject(result, caller));
            }
            return tournamentList;
        }

        public async Task<List<IStartedTournament>> GetTournamentsCreatedAfter(DateTime date)
        {
            List<TournamentResult> tournamentResultList = await new TournamentsQuery
            {
                Subdomain = TournamentSubdomain,
                created_after = date
            }.call(caller);

            return tournamentResultList.Select(result => new TournamentObject(result, caller)).Cast<IStartedTournament>().ToList();
        }

        public async Task<TournamentObject> getTournament(string tournamentID) {
            TournamentResult tournamentResult = await new TournamentQuery(tournamentID, TournamentSubdomain).call(caller);
            return new TournamentObject(tournamentResult, caller);
        }
    }
}
