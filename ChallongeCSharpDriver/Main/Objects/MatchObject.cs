using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main.Objects {
    using ChallongeCSharpDriver.Core;
    using ChallongeCSharpDriver.Core.Queries;
    using ChallongeCSharpDriver.Core.Results;

    public class MatchObject : OpenMatch, ClosedMatch {
        private ChallongeAPICaller caller;
        private MatchResult result;
        private UpdateMatchQuery updateMatchQuery;
        private MatchState matchState;
        public Task<Participant> player1 {
            get {
                return getPlayer(result.player1_id);
            }
        }
        public Task<Participant> player2 {
            get {
                return getPlayer(result.player2_id);
            }
        }
        public Task<Participant> winner {
            get {
                return getPlayer(result.winner_id);
            }
        }
        public Task<Participant> loser {
            get {
                return getPlayer(result.loser_id);
            }
        }
        public MatchState state {
            get {
                return matchState;
            }
        }

        public MatchObject(MatchResult result, ChallongeAPICaller caller) {
            this.result = result;
            this.caller = caller;
            switch (result.state) {
                case "open":
                    matchState = MatchState.Open;
                    break;
                case "pending":
                    matchState = MatchState.Pending;
                    break;
                case "completed":
                    matchState = MatchState.Complete;
                    break;
                default:
                    throw new InvalidMatchState();
            }
            updateMatchQuery = new UpdateMatchQuery(result);
        }

        private async Task<Participant> getPlayer(Nullable<int> playerID) {
            if (playerID.HasValue) {
                ParticipantResult participantResult = await new ParticipantQuery(result.tournament_id, playerID.Value).call(caller);
                return new ParticipantObject(participantResult);
            } else {
                throw new ParticipantNotAssigned();
            }
        }

        public void addScore(Score score) {
            updateMatchQuery.scores.Add(score);
        }

        public async Task update() {
            this.result = await updateMatchQuery.call(caller);
        }

        public async Task<ClosedMatch> close() {
            await update();
            return this;
        }

        public OpenMatch reopen() {
            return this;
        }

        public override string ToString() {
            return "Match #" + result.id + ", " + result.state;
        }
    }
}
