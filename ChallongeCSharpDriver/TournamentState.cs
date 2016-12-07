using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver {
    public enum TournamentState {
        All, Pending, InProgress, Ended
    }

    public class TournamentStateParser {
        public static string ToString(TournamentState state) {
            switch (state) {
                case TournamentState.All:
                    return "all";
                case TournamentState.Pending:
                    return "pending";
                case TournamentState.InProgress:
                    return "in_progress";
                case TournamentState.Ended:
                    return "ended";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
