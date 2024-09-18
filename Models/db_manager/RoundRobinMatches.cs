using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace tournament_blazor.Models.db_manager
{
    public class RoundRobinMatches
    {
        public RoundRobinMatches(int tournamentID, int team1ID, int team2ID)
        {
            TournamentID = tournamentID;
            Team1ID = team1ID;
            Team2ID = team2ID;
        }

        [Key]
        public int MatchID { get; set; }

        [Required]
        public int TournamentID { get; set; }

        [Required]
        public int Team1ID { get; set; }

        [Required]
        public int Team2ID { get; set; }

        public int? Team1Score { get; set; }
        public int? Team2Score { get; set; }
        public DateTime? MatchDate { get; set; }

        [ForeignKey("TournamentID")]
        [JsonIgnore] // Ignore Tournament to prevent cycles
        public Tournaments? Tournament { get; set; }

        [ForeignKey("Team1ID")]
        [JsonIgnore]
        public Teams? Team1 { get; set; }

        [ForeignKey("Team2ID")]
        [JsonIgnore]
        public Teams? Team2 { get; set; }
    }
}
