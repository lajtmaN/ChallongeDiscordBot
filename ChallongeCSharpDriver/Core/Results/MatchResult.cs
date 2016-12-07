
namespace ChallongeCSharpDriver.Core.Results {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class MatchResult {
        public int id { get; set; }
        public int tournament_id { get; set; }
        public string identifier { get; set; }
        public Nullable<int> player1_id { get; set; }
        public Nullable<int> player2_id { get; set; }
        public Nullable<int> winner_id { get; set; }
        public Nullable<int> loser_id { get; set; }
        public string state;
    }
}
