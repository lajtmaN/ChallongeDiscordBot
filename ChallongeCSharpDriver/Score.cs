using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChallongeCSharpDriver {
    public class Score {
        public int player1 { get; set; }
        public int player2 { get; set; }

        public Score(int player1, int player2) {
            this.player1 = player1;
            this.player2 = player2;
        }
    }
}
