using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChallongeCSharpDriver.Main;

namespace ChallongeDiscordBot.Entities
{
    public class Match
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }

        public bool Announced { get; set; }

        public int? Player1ID { get; set; }
        public virtual Participant Player1 { get; set; }

        public int? Player2ID { get; set; }
        public virtual Participant Player2 { get; set; }

        public int TournamentID { get; set; }
        public virtual Tournament Tournament { get; set; }

        public static Match CreateMatch(IMatch challonge)
        {
            return new Match
            {
                ID = challonge.id,
                Player1ID = challonge.player1.Result.id.ID,
                Player2ID = challonge.player2.Result.id.ID,
                TournamentID = challonge.tournament_id
            };
        }
    }
}
