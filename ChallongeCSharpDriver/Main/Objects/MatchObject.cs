using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeCSharpDriver.Main.Objects {
    using ChallongeCSharpDriver.Core;
    using ChallongeCSharpDriver.Core.Queries;
    using ChallongeCSharpDriver.Core.Results;

    public class MatchObject : IOpenMatch, IClosedMatch {
        private ChallongeAPICaller caller;
        private MatchResult result;
        private UpdateMatchQuery updateMatchQuery;
        private MatchState matchState;
        public Task<IParticipant> player1 => getPlayer(result.player1_id);
        public Task<IParticipant> player2 => getPlayer(result.player2_id);
        public string Location => result.location;
        public Task<IParticipant> winner => getPlayer(result.winner_id);
        public Task<IParticipant> loser => getPlayer(result.loser_id);
        public MatchState state => matchState;
        public DateTime StartedAt => result.started_at;


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

        private async Task<IParticipant> getPlayer(int? playerID) {
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


        public async Task<IClosedMatch> close() {
            await update();
            return this;
        }

        public IOpenMatch reopen() {
            return this;
        }

        public override string ToString() {
            return "Match #" + result.id + ", " + result.state;
        }
    }
}
