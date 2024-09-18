using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using tournament_blazor.Models.db_manager;

namespace tournament_blazor.Data
{
    public class tournamentDbContext : IdentityDbContext<Users>
    {
        public tournamentDbContext(DbContextOptions<tournamentDbContext> options) : base(options)
        {
        }
        public DbSet<tournament_blazor.Models.db_manager.Disciplines> Disciplines { get; set; } = default!;
        public DbSet<tournament_blazor.Models.db_manager.TournamentTypes> TournamentTypes { get; set; } = default!;

        public DbSet<Tournaments> Tournaments { get; set; } = default!;
        public DbSet<Teams> Teams { get; set; } = default!;
        public DbSet<BracketMatches> BracketMatches { get; set; } = default!;
        public DbSet<RoundRobinMatches> RoundRobinMatches { get; set; } = default!;
        public DbSet<Players> Players { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Disciplines>(entity =>
            {
                entity.ToTable("Disciplines");
                entity.HasKey(d => d.DisciplineID);
                entity.Property(d => d.DisciplineID)
                      .HasColumnType("uniqueidentifier")
                      .HasDefaultValueSql("NEWID()")
                      .IsRequired();
                entity.Property(d => d.DisciplineName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(d => d.Description)
                      .HasMaxLength(1000);
                entity.Property(d => d.PlayersPerTeam)
                      .IsRequired();
            });
            modelBuilder.Entity<TournamentTypes>(entity =>
            {
                entity.ToTable("TournamentTypes");

                entity.HasKey(t => t.TournamentTypeID);

                entity.Property(t => t.TournamentTypeID)
                      .ValueGeneratedOnAdd();

                entity.Property(t => t.TournamentTypeName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(t => t.Description)
                      .HasMaxLength(1000);
            });
            modelBuilder.Entity<Tournaments>(entity =>
            {
                entity.ToTable("Tournaments");
                entity.HasKey(t => t.TournamentID);
                entity.Property(t => t.TournamentName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(t => t.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                // Relationships
                entity.HasOne(t => t.User)
                      .WithMany(u => u.Tournaments)
                      .HasForeignKey(t => t.UserID);

                entity.HasOne(t => t.Discipline)
                      .WithMany()
                      .HasForeignKey(t => t.DisciplineID);

                entity.HasOne(t => t.TournamentType)
                      .WithMany()
                      .HasForeignKey(t => t.TournamentTypeID);

               
            });


            modelBuilder.Entity<Teams>(entity =>
            {
                entity.ToTable("Teams");
                entity.HasKey(t => t.TeamID);
                entity.Property(t => t.TeamName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasOne(t => t.Tournament)
                      .WithMany(tr => tr.Teams)
                      .HasForeignKey(t => t.TournamentID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BracketMatches>(entity =>
            {
                entity.ToTable("BracketMatches");
                entity.HasKey(b => b.MatchID);
                entity.HasOne(b => b.Tournament)
                      .WithMany(t => t.BracketMatches)
                      .HasForeignKey(b => b.TournamentID);

                entity.HasOne(b => b.Team1)
                      .WithMany(t => t.BracketMatchesAsTeam1)
                      .HasForeignKey(b => b.Team1ID)
                      .OnDelete(DeleteBehavior.Cascade); 

                entity.HasOne(b => b.Team2)
                      .WithMany(t => t.BracketMatchesAsTeam2)
                      .HasForeignKey(b => b.Team2ID)
                      .OnDelete(DeleteBehavior.Restrict);

                
            });



            // RoundRobinMatches configuration
            modelBuilder.Entity<RoundRobinMatches>(entity =>
            {
                entity.ToTable("RoundRobinMatches");
                entity.HasKey(r => r.MatchID);

                // Relationships
                entity.HasOne(r => r.Tournament)
                      .WithMany(t => t.RoundRobinMatches)
                      .HasForeignKey(r => r.TournamentID);

                entity.HasOne(r => r.Team1)
                      .WithMany(t => t.RoundRobinMatchesAsTeam1)
                      .HasForeignKey(r => r.Team1ID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Team2)
                      .WithMany(t => t.RoundRobinMatchesAsTeam2)
                      .HasForeignKey(r => r.Team2ID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Team1)
                      .WithMany(t => t.RoundRobinMatchesAsTeam1)
                      .HasForeignKey(r => r.Team1ID);

                entity.HasOne(r => r.Team2)
                      .WithMany(t => t.RoundRobinMatchesAsTeam2)
                      .HasForeignKey(r => r.Team2ID);
            });


            modelBuilder.Entity<Players>(entity =>
            {
                entity.ToTable("Players");
                entity.HasKey(p => p.PlayerID);

                entity.Property(p => p.PlayerName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.HasOne(p => p.Team)
                      .WithMany(t => t.Players)
                      .HasForeignKey(p => p.TeamID);
            });
        }
    }
}
