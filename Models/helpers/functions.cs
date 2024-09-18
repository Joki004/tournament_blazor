using tournament_blazor.Models.db_manager;
using tournament_blazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;




namespace tournament_blazor.Models.helpers
{
    public class Functions
    {
        // Generate balanced Round Robin matches
        public void GenerateBalancedRoundRobinMatches(TournamentStateService tournamentStateService, tournament_blazor.Data.tournamentDbContext context)
        {
            var teams = tournamentStateService.Teams;
            DateTime currentDate = tournamentStateService.Tournament.StartDate;

            Console.WriteLine("Generating Round Robin Matches...");

            // Keep track of last match played by each team to avoid back-to-back matches
            Dictionary<int, DateTime> lastMatchDate = teams.ToDictionary(t => t.TeamID, t => DateTime.MinValue);

            // Iterate through all teams
            for (int i = 0; i < teams.Count; i++)
            {
                for (int j = i + 1; j < teams.Count; j++)
                {
                    Console.WriteLine($"Scheduling match between Team {teams[i].TeamName} (ID: {teams[i].TeamID}) and Team {teams[j].TeamName} (ID: {teams[j].TeamID})");

                    if (lastMatchDate[teams[i].TeamID] == currentDate || lastMatchDate[teams[j].TeamID] == currentDate)
                    {
                        // Skip match if any team played on the current date, go to the next available date
                        currentDate = currentDate.AddDays(1);
                        Console.WriteLine($"Skipping current date. New match date: {currentDate}");
                    }

                    var match = new RoundRobinMatches(
                        tournamentStateService.Tournament.TournamentID, // Removed explicit MatchID assignment
                        teams[i].TeamID,
                        teams[j].TeamID)
                    {
                        MatchDate = currentDate
                    };

                    // Add the match to the context for saving
                    context.RoundRobinMatches.Add(match);
                    Console.WriteLine($"Match scheduled on {match.MatchDate} between Team {teams[i].TeamName} and Team {teams[j].TeamName}");

                    // Update last match played date for both teams
                    lastMatchDate[teams[i].TeamID] = currentDate;
                    lastMatchDate[teams[j].TeamID] = currentDate;

                    // Move to the next day
                    if (!tournamentStateService.Tournament.EndDate.HasValue)
                    {
                        currentDate = currentDate.AddHours(12); // Schedule two matches per day
                        Console.WriteLine($"Next match will be scheduled after 12 hours: {currentDate}");
                    }
                    else
                    {
                        // Spread matches evenly between start and end date
                        currentDate = currentDate.AddDays(1);
                        if (currentDate > tournamentStateService.Tournament.EndDate.Value)
                        {
                            currentDate = tournamentStateService.Tournament.StartDate;
                            Console.WriteLine($"End date reached, resetting match date to start date: {currentDate}");
                        }
                        else
                        {
                            Console.WriteLine($"Next match will be scheduled on {currentDate}");
                        }
                    }
                }
            }

            Console.WriteLine("Round Robin Matches generation completed.");
        }

        // Generate balanced Bracket matches
        public void GenerateBracketMatches(TournamentStateService tournamentStateService, tournament_blazor.Data.tournamentDbContext context)
        {
            var teams = tournamentStateService.Teams;
            DateTime currentDate = tournamentStateService.Tournament.StartDate;

            Console.WriteLine("Generating Bracket Matches for Round 1...");

            List<Teams> remainingTeams = new List<Teams>(teams);

            // Only generate matches for the first round
            int round = 1;

            // Pair teams in round 1
            for (int i = 0; i < remainingTeams.Count - 1; i += 2)
            {
                var match = new BracketMatches(
                     tournamentStateService.Tournament.TournamentID,
                     round,
                     remainingTeams[i].TeamID,
                     remainingTeams[i + 1].TeamID,
                     i / 2 + 1)
                {
                    MatchDate = currentDate
                };


                context.BracketMatches.Add(match);
                Console.WriteLine($"Round {round}: Team {remainingTeams[i].TeamName} vs Team {remainingTeams[i + 1].TeamName}, Date: {match.MatchDate}");

                // Move to the next match date (12 hours later or next day)
                if (!tournamentStateService.Tournament.EndDate.HasValue)
                {
                    currentDate = currentDate.AddHours(12); // Schedule two matches per day
                }
                else
                {
                    currentDate = currentDate.AddDays(1); // Spread matches evenly
                    if (currentDate > tournamentStateService.Tournament.EndDate.Value)
                    {
                        currentDate = tournamentStateService.Tournament.StartDate;
                    }
                }
            }

            Console.WriteLine("Round 1 matches created. Future rounds will be determined by winners.");

            // Placeholder logic for future rounds (round 2 and onwards)
            int matchNumber = 1;
            while (remainingTeams.Count / 2 >= 1)
            {
                Console.WriteLine($"Scheduling matches for future round {round + 1}:");

                // Prepare placeholders for winners of the previous round's matches
                for (int i = 0; i < remainingTeams.Count / 2; i++)
                {
                    Console.WriteLine($"Round {round + 1}: Winner of Match {matchNumber} vs Winner of Match {matchNumber + 1}");
                    matchNumber += 2;
                }

                remainingTeams = remainingTeams.Take(remainingTeams.Count / 2).ToList();
                round++;
            }
        }


        public async Task CalculateWinner(int tournamentID, int tournamentType, tournament_blazor.Data.tournamentDbContext context)
        {
            string winnerTeamName = "Not yet decided";

            if (tournamentType == 1) // Bracket Tournament
            {
                var bracketMatches = await context.BracketMatches
                    .Where(m => m.TournamentID == tournamentID)
                    .ToListAsync();

                if (bracketMatches.All(m => m.Team1Score.HasValue && m.Team2Score.HasValue))
                {
                    // Get the final winner based on the last match played (you might need more detailed logic)
                    var finalMatch = bracketMatches.OrderByDescending(m => m.MatchDate).FirstOrDefault();
                    if (finalMatch != null)
                    {
                        // Determine the winner based on scores
                        if (finalMatch.Team1Score > finalMatch.Team2Score)
                        {
                            winnerTeamName = finalMatch.Team1?.TeamName ?? "Not yet decided";
                        }
                        else if (finalMatch.Team2Score > finalMatch.Team1Score)
                        {
                            winnerTeamName = finalMatch.Team2?.TeamName ?? "Not yet decided";
                        }
                    }
                }
            }
            else if (tournamentType == 2) // Round Robin Tournament
            {
                var roundRobinMatches = await context.RoundRobinMatches
                    .Where(m => m.TournamentID == tournamentID)
                    .ToListAsync();

                if (roundRobinMatches.All(m => m.Team1Score.HasValue && m.Team2Score.HasValue))
                {
                    // Get the team with the highest number of wins or points
                    var teams = await context.Teams
                        .Where(t => t.TournamentID == tournamentID)
                        .ToListAsync();

                    // Assuming each team accumulates points based on wins
                    var teamScores = teams.ToDictionary(t => t.TeamID, t => 0);

                    foreach (var match in roundRobinMatches)
                    {
                        if (match.Team1Score > match.Team2Score)
                        {
                            teamScores[match.Team1ID]++;
                        }
                        else if (match.Team2Score > match.Team1Score)
                        {
                            teamScores[match.Team2ID]++;
                        }
                    }

                    // Determine the team with the highest score
                    var winningTeamID = teamScores.OrderByDescending(t => t.Value).FirstOrDefault().Key;
                    var winner = teams.FirstOrDefault(t => t.TeamID == winningTeamID);

                    if (winner != null)
                    {
                        winnerTeamName = winner.TeamName;
                    }
                }
            }

            Console.WriteLine($"The winner is: {winnerTeamName}");
        }



    }
}
