using Castle.Core.Internal;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Models.Notifications;
using fantasy_hoops.Models.PushNotifications;
using fantasy_hoops.Models.Tournaments;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
        public DbSet<RequestNotification> RequestNotifications { get; set; }
        public DbSet<PushSubscription> PushSubscriptions { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<BestLineup> BestLineups { get; set; }
        public DbSet<PlayersBestLineups> PlayersBestLineups { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentUser> TournamentUsers { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<MatchupPair> TournamentMatchups { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<TournamentInvite> TournamentInvites { get; set; }

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
                .HasKey(matchup => new {matchup.TournamentID, matchup.FirstUserID, matchup.SecondUserID, matchup.ContestId});

            builder.Entity<TournamentUser>()
                .HasKey(tournamentUser => new {tournamentUser.TournamentID, tournamentUser.UserID});

            builder.Entity<Tournament>()
                .HasMany(tournament => tournament.Contests);

            builder.Entity<Contest>()
                .HasMany(contest => contest.Matchups);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = Startup.Configuration.GetConnectionString("DefaultConnection");
            if (!connectionString.IsNullOrEmpty())
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}