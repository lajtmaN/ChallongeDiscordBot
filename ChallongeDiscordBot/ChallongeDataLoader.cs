using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChallongeCSharpDriver.Caller;
using ChallongeCSharpDriver.Main;
using ChallongeCSharpDriver.Main.Objects;

namespace ChallongeDiscordBot
{
    public class ChallongeDataLoader
    {
        private Tournaments Tournaments { get; }
        private DateTime CreatedAfterDate { get; }

        public ChallongeDataLoader(string apiKey, string subdomain, DateTime createdAfterDate)
        {
            var config = new ChallongeConfig(apiKey);
            var caller = new ChallongeHTTPClientAPICaller(config);
            Tournaments = new Tournaments(caller, subdomain);
            CreatedAfterDate = createdAfterDate;
        }

        public async void LoadNewestData()
        {
            
        }

        public async Task<IEnumerable<IStartedTournament>> GetStartedTournaments()
        {
            return await Tournaments.getStartedTournaments(CreatedAfterDate);
        }

        public async Task<TournamentObject> LoadTournament(string link)
        {
            return await Tournaments.getTournament(link);
        }
    }
}
