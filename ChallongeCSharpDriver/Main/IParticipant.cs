﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main {
    public interface IParticipant {
        ParticipantID id { get; }
        string name { get; }
        int tournament_id { get; }
        bool active { get; }
    }
}
