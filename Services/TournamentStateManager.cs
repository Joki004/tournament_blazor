using tournament_blazor.Models.db_manager;
using System.Collections.Generic;

public class TournamentStateManager
{
    public Tournaments? CurrentTournament { get; set; }
    public List<RoundRobinMatches>? RoundRobinMatches { get; set; }
    public List<BracketMatches>? BracketMatches { get; set; }

    public void SetCurrentTournament(Tournaments tournament)
    {
        CurrentTournament = tournament;
    }

    public void SetMatches(List<RoundRobinMatches> roundRobinMatches, List<BracketMatches> bracketMatches)
    {
        RoundRobinMatches = roundRobinMatches;
        BracketMatches = bracketMatches;
    }

    public void ClearState()
    {
        CurrentTournament = null;
        RoundRobinMatches = null;
        BracketMatches = null;
    }
}
