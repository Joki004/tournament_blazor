using System.ComponentModel.DataAnnotations;

namespace tournament_blazor.Models.db_manager
{
    public class Disciplines
    {
        [Key]
        public Guid DisciplineID { get; set; }

        [Required]
        [MaxLength(100)]
        public string DisciplineName { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(1, 100)]
        public int PlayersPerTeam
        {
            get; set;
        }
    }
}
