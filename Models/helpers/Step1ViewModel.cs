namespace tournament_blazor.Models.helpers
{
    public class Step1ViewModel
    {
        public Guid? SelectedDisciplineID { get; set; }
        public string NewDisciplineName { get; set; } = string.Empty;
        public string NewDisciplineDescription { get; set; } = string.Empty;
        public int NewDisciplinePlayersPerTeam { get; set; } = 0;
        public string ErrorMessage { get; set; } = string.Empty;
        public bool ShowNewDisciplineFields { get; set; } = false;
    }
}
