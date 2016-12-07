﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChallongeCSharpDriver.Caller;
using ChallongeCSharpDriver.Main;

namespace ChallongeDiscordBot
{
    public class ChallongeDataLoader
    {
        private Tournaments tournaments { get; }
        public ChallongeDataLoader(string apiKey, string subdomain, DateTime createdAfterDate)
        {
            var config = new ChallongeConfig(apiKey);
            var caller = new ChallongeHTTPClientAPICaller(config);
            tournaments = new Tournaments(caller, subdomain);

            var res = tournaments.GetTournamentsCreatedAfter(createdAfterDate);
            
        }

        public async void LoadNewestData()
        {
            
            await tournaments.getTournament("CSGO");
        }
    }
}