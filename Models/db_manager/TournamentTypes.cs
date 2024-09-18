using System.ComponentModel.DataAnnotations;

namespace tournament_blazor.Models.db_manager
{
    public class TournamentTypes
    {


        [Key]
        public int TournamentTypeID { get; set; }

        [Required]
        [MaxLength(100)]
        public string TournamentTypeName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
