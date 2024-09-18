using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using tournament_blazor.Data;
using System.Text.Json.Serialization;

namespace tournament_blazor.Models.db_manager
{
    public class Tournaments
    {
        public Tournaments()
        {
            BracketMatches = new HashSet<BracketMatches>();
            RoundRobinMatches = new HashSet<RoundRobinMatches>();
            Teams = new HashSet<Teams>();
        }

        [Key]
        public int TournamentID { get; set; }

        [Required]
        public bool IsPublic { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public string UserID { get; set; }

        [Required]
        [MaxLength(100)]
        public string TournamentName { get; set; } = string.Empty;

        [Required]
        public Guid DisciplineID { get; set; }

        [Required]
        public int TournamentTypeID { get; set; }

        [Range(2, int.MaxValue)]
        public int NumberOfTeams { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserID")]
        public Users? User { get; set; }

        [ForeignKey("DisciplineID")]
        public Disciplines? Discipline { get; set; }

        [ForeignKey("TournamentTypeID")]
        public TournamentTypes? TournamentType { get; set; }

        [JsonIgnore] // Ignore Teams to prevent cycles
        public ICollection<Teams> Teams { get; set; }

        public ICollection<BracketMatches> BracketMatches { get; set; }

        public ICollection<RoundRobinMatches> RoundRobinMatches { get; set; }
    }
}
