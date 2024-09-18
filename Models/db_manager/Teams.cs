using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace tournament_blazor.Models.db_manager
{
    public class Teams
    {
        public Teams()
        {
            BracketMatchesAsTeam1 = new HashSet<BracketMatches>();
            BracketMatchesAsTeam2 = new HashSet<BracketMatches>();
            RoundRobinMatchesAsTeam1 = new HashSet<RoundRobinMatches>();
            RoundRobinMatchesAsTeam2 = new HashSet<RoundRobinMatches>();
            Players = new HashSet<Players>();  
        }

        [Key]
        public int TeamID { get; set; }

        [Required]
        public int TournamentID { get; set; }

        [Required]
        [MaxLength(100)]
        public string TeamName { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int MatchesPlayed { get; set; }

        [ForeignKey("TournamentID")]
        [JsonIgnore]  // This will prevent circular reference during JSON serialization
        public Tournaments? Tournament { get; set; }

        public ICollection<Players> Players { get; set; }

        public ICollection<BracketMatches> BracketMatchesAsTeam1 { get; set; }

        public ICollection<BracketMatches> BracketMatchesAsTeam2 { get; set; }

        public ICollection<RoundRobinMatches> RoundRobinMatchesAsTeam1 { get; set; }

        public ICollection<RoundRobinMatches> RoundRobinMatchesAsTeam2 { get; set; }
    }
}
