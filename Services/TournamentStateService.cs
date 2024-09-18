using tournament_blazor.Models.db_manager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tournament_blazor.Services
{
    public class TournamentStateService
    {
        public Tournaments Tournament { get; set; }
        public List<Teams> Teams { get; set; } = new List<Teams>();
        public List<string> TeamNames { get; set; } = new List<string>();

        public TournamentStateService()
        {
            InitializeNewTournament();
        }

        public void InitializeNewTournament()
        {
            Tournament = new Tournaments
            {
                IsPublic = false,
                ImageUrl = string.Empty,
                TournamentName = "t",
                DisciplineID = Guid.Empty,
                TournamentTypeID = 0,
                NumberOfTeams = 2,
                StartDate = DateTime.Now,
                EndDate = null,
                CreatedAt = DateTime.UtcNow,
                UserID = string.Empty
            };

            Teams = new List<Teams>();
            TeamNames = new List<string>(Enumerable.Repeat(string.Empty, Tournament.NumberOfTeams)); // Initialize with empty team names
        }

        public void ResetTournament()
        {
            InitializeNewTournament();
        }

        public void UpdateNumberOfTeams(int numberOfTeams)
        {
            Tournament.NumberOfTeams = numberOfTeams;

            // Adjust the TeamNames list only when needed
            if (TeamNames.Count < numberOfTeams)
            {
                TeamNames.AddRange(Enumerable.Repeat(string.Empty, numberOfTeams - TeamNames.Count));
            }
            else if (TeamNames.Count > numberOfTeams)
            {
                TeamNames = TeamNames.Take(numberOfTeams).ToList();
            }
        }

        public void AddTeams(List<string> teamNames)
        {
            Teams = teamNames.Select((teamName, index) => new Teams
            {
                TeamName = teamName,
                MatchesPlayed = 0,
                TournamentID = Tournament.TournamentID
            }).ToList();
        }
    }
}
