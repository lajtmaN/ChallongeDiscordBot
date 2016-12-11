
namespace ChallongeCSharpDriver.Core.Results {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TournamentResult {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string full_challonge_url { get; set; }
        public string description { get; set; }
        public string tournament_type { get; set; }
        public string state { get; set; }
        public string subdomain { get; set; }
        public DateTime? started_checking_in_at { get; set; }
    }
}
