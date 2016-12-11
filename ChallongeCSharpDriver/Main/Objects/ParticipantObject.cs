using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main.Objects {
    using ChallongeCSharpDriver.Core.Results;

    public class ParticipantObject : IParticipant {
        private readonly ParticipantResult result;
        
        public ParticipantID id => new ParticipantID(result.id, result.group_player_ids.FirstOrDefault());
        public string name => result.display_name;

        public int tournament_id => result.tournament_id;

        public ParticipantObject(ParticipantResult result) {
            this.result = result;
        }

        public override string ToString() {
            return "Participant #" + result.id + ", " + name;
        }
    }
}
