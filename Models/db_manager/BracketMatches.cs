using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace tournament_blazor.Models.db_manager
{
    public class BracketMatches
    {
        public BracketMatches() { }

        public BracketMatches(int tournamentID, int round, int team1ID, int team2ID, int matchNumber)
        {
            TournamentID = tournamentID;
            Round = round;
            Team1ID = team1ID;
            Team2ID = team2ID;
            MatchNumber = matchNumber;
        }

        [Key]
        public int MatchID { get; set; }

        [Required]
        public int TournamentID { get; set; }

        [Required]
        public int Round { get; set; }

        [Required]
        public int Team1ID { get; set; }

        [Required]
        public int? Team2ID { get; set; }

        public int MatchNumber { get; set; }

        public DateTime? MatchDate { get; set; }

        public int? Team1Score { get; set; }
        public int? Team2Score { get; set; }

        [ForeignKey("TournamentID")]
        public Tournaments? Tournament { get; set; }

        [ForeignKey("Team1ID")]
        public Teams? Team1 { get; set; }

        [ForeignKey("Team2ID")]
        public Teams? Team2 { get; set; }
    }
}
