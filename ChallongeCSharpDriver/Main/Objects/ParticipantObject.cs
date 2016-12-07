using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main.Objects {
    using ChallongeCSharpDriver.Core.Results;

    public class ParticipantObject : Participant {
        private ParticipantResult result;
        public string name {
            get {
                return result.name;
            }
        }

        public ParticipantObject(ParticipantResult result) {
            this.result = result;
        }

        public override string ToString() {
            return "Participant #" + result.id + ", " + name;
        }
    }
}
