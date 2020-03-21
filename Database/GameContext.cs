using System.ComponentModel.DataAnnotations.Schema;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Models.Notifications;
using fantasy_hoops.Models.Tournaments;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace fantasy_hoops.Database
{
    public class GameContext : IdentityDbContext<User>
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Stats> Stats { get; set; }
        public DbSet<Injury> Injuries { get; set; }
        public DbSet<Paragraph> Paragraphs { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<UserLineup> UserLineups { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<GameScoreNotification> GameScoreNotifications { get; set; }
        public DbSet<InjuryNotification> InjuryNotifications { get; set; }
        public DbSet<FriendRequestNotification> FriendRequestNotifications { get; set; }
        public DbSet<PushSubscription> PushSubscriptions { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<BestLineup> BestLineups { get; set; }
        public DbSet<PlayersBestLineups> PlayersBestLineups { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Tournament.TournamentType> TournamentTypes { get; set; }
        public DbSet<TournamentUsers> TournamentUsers { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<MatchupPair> TournamentMatchups { get; set; }

        public GameContext()
        {
        }

        public GameContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Player>()
                .HasOne(player => player.Injury)
                .WithOne(injury => injury.Player)
                .HasForeignKey<Injury>(injury => injury.PlayerID);

            builder.Entity<PlayersBestLineups>()
                .HasKey(playerLineups => new {playerLineups.PlayerID, playerLineups.BestLineupID});

            builder.Entity<PlayersBestLineups>()
                .HasOne(x => x.Player)
                .WithMany(x => x.BestLineups)
                .HasForeignKey(x => x.PlayerID);

            builder.Entity<PlayersBestLineups>()
                .HasOne(x => x.BestLineup)
                .WithMany(x => x.Lineup)
                .HasForeignKey(x => x.BestLineupID);

            builder.Entity<UserAchievement>()
                .HasKey(userAchievement => new {userAchievement.UserID, userAchievement.AchievementID});

            builder.Entity<MatchupPair>()
                .HasKey(matchup => new {matchup.TournamentID, matchup.FirstUserID, matchup.SecondUserID});

            builder.Entity<TournamentUsers>()
                .HasKey(tournamentUser => new {tournamentUser.TournamentID, tournamentUser.UserID});
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Startup.Configuration["fh-connection-string"]);
        }
    }
}