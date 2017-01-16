using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChallongeCSharpDriver;
using ChallongeCSharpDriver.Main;

namespace ChallongeDiscordBot.Entities
{
    public class Tournament
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string ChallongeIDWithSubdomain { get; set; }
        public string FullLink { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public bool Announced { get; set; }
        public bool CheckInOpen { get; set; }
        public virtual IList<Match> Matches { get; set; }
        public virtual IList<Participant> Participants { get; set; }

        public static Tournament CreateTournament(ITournament challonge)
        {
            return new Tournament
            {
                ID = challonge.TournamentID,
                ChallongeIDWithSubdomain = challonge.TournamentSubdomainID,
                ShortName = challonge.URL,
                Name = challonge.Name,
                FullLink = challonge.FullLink
            };
        }
    }
}
