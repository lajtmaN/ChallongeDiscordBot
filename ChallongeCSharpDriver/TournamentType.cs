using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver {
    public enum TournamentType {
        Single_Elimination, Double_Elimination, Round_Robin, Swiss
    }

    public class TournamentTypeParser {
        public static string ToIndexString(TournamentType type) {
            switch (type) {
                case TournamentType.Single_Elimination:
                    return "single_elimination";
                case TournamentType.Double_Elimination:
                    return "double_elimination";
                case TournamentType.Round_Robin:
                    return "round_robin";
                case TournamentType.Swiss:
                    return "swiss";
                default:
                    throw new NotSupportedException();
            }
        }

        public static string ToCreateString(TournamentType type) {
            switch (type) {
                case TournamentType.Single_Elimination:
                    return "Single elimination";
                case TournamentType.Double_Elimination:
                    return "double elimination";
                case TournamentType.Round_Robin:
                    return "round robin";
                case TournamentType.Swiss:
                    return "swiss";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
