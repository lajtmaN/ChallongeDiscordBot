using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Core.Queries {
    using ChallongeCSharpDriver.Core.Results;

    public class UpdateMatchQuery : ChallongeQuery<MatchResult> {
        private MatchResult result;
        public List<Score> scores { get; set; }
        public Nullable<int> player1_votes { get; set; }
        public Nullable<int> player2_votes { get; set; }

        private class MatchQueryResult {
            public MatchResult match { get; set; }
        }

        public UpdateMatchQuery(MatchResult result) {
            this.result = result;
            this.scores = new List<Score>();
        }

        private ChallongeQueryParameters getParameters() {
            ChallongeQueryParameters parameters = new ChallongeQueryParameters();
            if (scores.Count > 0) {
                List<string> formattedScoreList = new List<string>();
                int player1TotalScore = 0;
                int player2TotalScore = 0;
                foreach (Score score in scores) {
                    formattedScoreList.Add(scoreToString(score));
                    if (score.player1 > score.player2) {
                        player1TotalScore += 1;
                    } else if (score.player2 > score.player1) {
                        player2TotalScore += 1;
                    }
                }
                parameters.Add("match[scores_csv]", String.Join(",", formattedScoreList));
                if (player1TotalScore > player2TotalScore) {
                    parameters.Add("match[winner_id]", result.player1_id.ToString());
                } else if (player2TotalScore > player1TotalScore) {
                    parameters.Add("match[winner_id]", result.player2_id.ToString());
                } else {
                    parameters.Add("match[winner_id]", "tie");
                }
            }
            return parameters;
        }

        private string scoreToString(Score score) {
            return score.player1 + "-" + score.player2;
        }

        private string getAPIPath() {
            return "tournaments/" + result.tournament_id + "/matches/" + result.id;
        }

        public async Task<MatchResult> call(ChallongeAPICaller caller) {
            MatchQueryResult matchQueryResult = await caller.PUT<MatchQueryResult>(getAPIPath(), getParameters());
            return matchQueryResult.match;
        }

    }
}
