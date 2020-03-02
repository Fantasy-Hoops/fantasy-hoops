using System;
using fantasy_hoops.Models;
using fantasy_hoops.Models.Achievements;
using fantasy_hoops.Models.Notifications;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

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
                .HasKey(userAchievement => new { userAchievement.UserID, userAchievement.AchievementID });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var keyVaultEndpoint = GetKeyVaultEndpoint();
                var azureServiceTokenProvider = new AzureServiceTokenProvider(
                    "RunAs=App;AppId=9c1be78d-8e41-4887-9be3-1b54eaefb2d1;TenantId=4c6b4065-35b7-4671-b6fe-2f7ca805942b;AppKey=3-Eh7n8[e6TQ:W.-d=RLG-[@PYAMdDnq"
                );
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        azureServiceTokenProvider.KeyVaultTokenCallback));
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .AddAzureKeyVault(
                        keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager())
                    .AddEnvironmentVariables()
                   .Build();
                optionsBuilder.UseSqlServer(configuration["fh-connection-string"]);
            }
        }
        private static string GetKeyVaultEndpoint() => "https://fantasy-hoops.vault.azure.net";
    }
}
