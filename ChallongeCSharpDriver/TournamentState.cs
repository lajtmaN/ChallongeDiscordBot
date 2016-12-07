using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver {
    public enum TournamentState {
        All, Pending, In_Progress, Complete, Awaiting_Review
    }

    public class TournamentStateParser {
        public static string ToString(TournamentState state) {
            switch (state) {
                case TournamentState.All:
                    return "all";
                case TournamentState.Pending:
                    return "pending";
                case TournamentState.In_Progress:
                    return "in_progress";
                case TournamentState.Complete:
                    return "complete";
                case TournamentState.Awaiting_Review:
                    return "awaiting_review";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
