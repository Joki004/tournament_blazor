using Microsoft.EntityFrameworkCore;
using tournament_blazor.Models.db_manager;
using tournament_blazor.Data;

public class TournamentService
{
    private readonly IDbContextFactory<tournamentDbContext> _dbFactory;

    public TournamentService(IDbContextFactory<tournamentDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    // Main method to update the tournament and its matches (both round-robin and bracket)
    public async Task<bool> UpdateTournamentAndMatches(Tournaments tournament, List<RoundRobinMatches> matches, List<BracketMatches> bracketMatches)
    {
        using var context = _dbFactory.CreateDbContext();
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            context.Attach(tournament).State = EntityState.Modified;

            if (tournament.TournamentTypeID == 2)
            {



                // Handle round-robin matches
                foreach (var match in matches)
                {
                    if (match.Team1Score.HasValue && match.Team2Score.HasValue)
                    {
                        if (match.Team1Score < 0 || match.Team2Score < 0)
                        {
                            Console.WriteLine($"Invalid scores for match with ID: {match.MatchID}. Team1Score: {match.Team1Score}, Team2Score: {match.Team2Score}. Scores cannot be negative.");
                            return false;
                        }
                        var trackedMatch = await context.RoundRobinMatches.FindAsync(match.MatchID);
                        if (trackedMatch != null)
                        {
                            trackedMatch.Team1Score = match.Team1Score;
                            trackedMatch.Team2Score = match.Team2Score;
                            context.Entry(trackedMatch).State = EntityState.Modified;

                            await context.SaveChangesAsync();
                            Console.WriteLine("Changes saved to the database.");

                            // Verify the updated match from the database
                            var updatedMatch = await context.RoundRobinMatches.FindAsync(match.MatchID);


                            foreach (var entry in context.ChangeTracker.Entries())
                            {
                                Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");
                            }


                        }
                        else
                        {
                            Console.WriteLine("Not processing");
                        }

                    }
                    else
                    {
                        Console.WriteLine($"Skipping match with ID: {match.MatchID} due to missing score. Team1Score: {match.Team1Score?.ToString() ?? "null"} | Team2Score: {match.Team2Score?.ToString() ?? "null"}");
                    }
                }
            }
            else if (tournament.TournamentTypeID == 1)

            {
                var maxRound = 0;
                var flag = 0;
                bool draw = false;


                foreach (var bracket in bracketMatches)
                {
                    Console.WriteLine($"{flag}--");
                    if (bracket.Round > maxRound)
                    {
                        maxRound = bracket.Round;

                    }
                    if (bracket.Team1Score == bracket.Team2Score) draw = true;
                    if (bracket.Team1Score < 0 || bracket.Team2Score < 0)
                    {
                        Console.WriteLine($"Invalid scores for match with ID: {bracket.MatchID}. Team1Score: {bracket.Team1Score}, Team2Score: {bracket.Team2Score}. Scores cannot be negative.");
                        return false;
                    }
                    flag++;
                }

                if (draw)
                {
                    Console.WriteLine($"Can't Draw");
                    return false;
                }


                Console.WriteLine($"Max Round: {maxRound}");

                // Check if all matches in the current round are completed
                bool allMatchesCompleted = bracketMatches.All(m => m.Team1Score.HasValue && m.Team2Score.HasValue);
                Console.WriteLine($"All matches in the current round completed: {allMatchesCompleted}");

                // Check if a higher round exists in the database
                bool higherRoundExists = await context.BracketMatches
                    .AnyAsync(m => m.TournamentID == tournament.TournamentID && m.Round > maxRound);

                // Prevent modifications if a higher round exists
                if (higherRoundExists)
                {
                    Console.WriteLine($"A higher round already exists. No modifications allowed for round {maxRound}.");
                    return false; // Exit early to prevent modifications
                }



                foreach (var bracket in bracketMatches)
                {
                    if (bracket.Team1Score.HasValue && bracket.Team2Score.HasValue)
                    {
                        context.Attach(bracket).State = EntityState.Modified;
                    }
                    else
                    {
                        Console.WriteLine($"Skipping bracket match with ID: {bracket.MatchID} because one or both scores are null.");
                    }
                }

                await context.SaveChangesAsync();

                // Calculate the total number of rounds
                int totalRounds = (int)Math.Log2(tournament.NumberOfTeams);
                Console.WriteLine($"totalRounds: {totalRounds}");
                if (allMatchesCompleted)
                {
                    if (maxRound < totalRounds)
                    {
                        var matchesInMaxRound = bracketMatches.Where(m => m.Round == maxRound).ToList();
                        await CreateNextRoundMatches(context, tournament, maxRound + 1, matchesInMaxRound);
                    }
                    else
                    {
                        Console.WriteLine("Final match completed. No more rounds to create.");
                    }
                }
            }

            await transaction.CommitAsync();
            Console.WriteLine("Tournament and matches updated successfully.");
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error updating tournament and matches: {ex.Message}");
            return false;
        }
    }

    private async Task CreateNextRoundMatches(tournamentDbContext context, Tournaments tournament, int nextRound, List<BracketMatches> currentRoundMatches)
    {
        List<int> winningTeams = new List<int>();

        // Collect the winning teams from the current round
        foreach (var match in currentRoundMatches)
        {
            Console.WriteLine($"{match.Team2Score}--{match.Team1Score}--{match.Round}");
            int? winnerTeamID = GetWinner(match);
            Console.WriteLine($"{winnerTeamID}");
            if (winnerTeamID.HasValue)
            {
                winningTeams.Add(winnerTeamID.Value);
            }
        }

        // Check if we are at the final match
        if (winningTeams.Count == 2)
        {
            // Create the final match
            var finalMatch = new BracketMatches
            {
                TournamentID = tournament.TournamentID,
                Team1ID = winningTeams[0],
                Team2ID = winningTeams[1],
                Round = nextRound,
                MatchNumber = 1,
                MatchDate = DateTime.Now.AddDays(1)
            };

            context.BracketMatches.Add(finalMatch);
            Console.WriteLine($"Created final match: Team {winningTeams[0]} vs Team {winningTeams[1]} in round {nextRound}");
        }
        else if (winningTeams.Count > 2)
        {
            // Continue with the next round
            for (int i = 0; i < winningTeams.Count; i += 2)
            {
                if (i + 1 >= winningTeams.Count)
                {
                    Console.WriteLine("No second team available for the next match. Tournament is over.");
                    break;
                }

                var nextMatch = new BracketMatches
                {
                    TournamentID = tournament.TournamentID,
                    Team1ID = winningTeams[i],
                    Team2ID = winningTeams[i + 1],
                    Round = nextRound,
                    MatchNumber = (i / 2) + 1,
                    MatchDate = DateTime.Now.AddDays(1)
                };

                context.BracketMatches.Add(nextMatch);
                Console.WriteLine($"Created next round match: Team {winningTeams[i]} vs Team {winningTeams[i + 1]} in round {nextRound}");
            }
        }
        else
        {
            Console.WriteLine("The tournament is complete. Final winner has been determined.");
            return;
        }

        await context.SaveChangesAsync();
    }


    private int? GetWinner(BracketMatches match)
    {
        if (match.Team1Score > match.Team2Score)
        {
            return match.Team1ID;
        }
        else if (match.Team2Score > match.Team1Score)
        {
            return match.Team2ID;
        }
        return null;
    }
}
