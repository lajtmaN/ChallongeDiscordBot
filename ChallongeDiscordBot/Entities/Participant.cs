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
    /// <summary>
    /// There exist one Entities.Participant for each participant in each tournament 
    /// (same DiscordUserName could occour on multiple Particpant)
    /// </summary>
    public class Participant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public int? GroupParticipantID { get; set; }
        public int? SeatNum { get; set; }
        public string DiscordUserName { get; set; }
        public string DisplayName { get; set; }
        public bool CheckedIn { get; set; }
        public int TournamentID { get; set; }
        public virtual Tournament Tournament { get; set; }
        public virtual IList<Match> Matches { get; set; }

        public static Participant CreateParticipant(IParticipant challonge)
        {
            return new Participant
            {
                ID = challonge.id.ID,
                GroupParticipantID = challonge.id.GroupID,
                TournamentID = challonge.tournament_id,
                DisplayName = challonge.name
            };
        }
    }
}
