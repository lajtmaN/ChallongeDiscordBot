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
        public int? player1_id => result.player1_id;
        public Task<IParticipant> player1 => GetPlayer(player1_id);
        public int? player2_id => result.player2_id;
        public Task<IParticipant> player2 => GetPlayer(player2_id);
        public string Location => result.location;
        public Task<IParticipant> winner => GetPlayer(result.winner_id);
        public Task<IParticipant> loser => GetPlayer(result.loser_id);
        public int id => result.id;
        public int tournament_id => result.tournament_id;
        public MatchState state => matchState;
        public DateTime StartedAt => result.started_at;
        public DateTime? MarkedAsActive => result.underway_at;
        public bool MatchMarkedAsActive => MarkedAsActive.HasValue;


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
                case "complete":
                case "completed":
                    matchState = MatchState.Complete;
                    break;
                default:
                    throw new InvalidMatchState();
            }
            updateMatchQuery = new UpdateMatchQuery(result);
        }

        private async Task<IParticipant> GetPlayer(int? playerID) {
            if (playerID.HasValue) {
                ParticipantResult participantResult = await new ParticipantQuery(result.tournament_subdomain_id, new ParticipantID(playerID.Value, playerID.Value)).call(caller);
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
