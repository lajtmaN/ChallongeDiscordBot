using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Core.Objects {
    public class ParticipantEntry {
        public string name { get; set; }
        public ParticipantEntry(string name) {
            this.name = name;
        }
    }
}
