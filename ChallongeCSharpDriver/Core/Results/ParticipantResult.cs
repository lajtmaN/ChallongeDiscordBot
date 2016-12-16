using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Core.Results {
    public class ParticipantResult {
        public int tournament_id { get; set; }
        public int id { get; set; }
        public string display_name { get; set; }
        public bool active { get; set; }
        public List<int> group_player_ids { get; set; }
    }
}
